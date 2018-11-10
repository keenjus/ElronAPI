using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PostgreSQLCopyHelper;

namespace ElronAPI.Api.Hangfire
{
    public class GtfsImport : IJob
    {
        private readonly HttpClient _client;
        private readonly string _tempFolderPath;
        private readonly string _connectionString;

        private readonly PostgreSQLCopyHelper<Agency> _agencyCopyHelper =
            new PostgreSQLCopyHelper<Agency>("public", "agencies")
                .MapBigInt("agency_id", x => x.agency_id)
                .MapVarchar("agency_name", x => x.agency_name)
                .MapVarchar("agency_url", x => x.agency_url)
                .MapVarchar("agency_timezone", x => x.agency_timezone)
                .MapVarchar("agency_phone", x => x.agency_phone)
                .MapVarchar("agency_lang", x => x.agency_lang);

        private readonly PostgreSQLCopyHelper<Calendar> _calendarCopyHelper =
            new PostgreSQLCopyHelper<Calendar>("public", "calendar")
                .MapInteger("service_id", x => x.service_id)
                .MapBoolean("monday", x => x.monday)
                .MapBoolean("tuesday", x => x.tuesday)
                .MapBoolean("wednesday", x => x.wednesday)
                .MapBoolean("thursday", x => x.thursday)
                .MapBoolean("friday", x => x.friday)
                .MapBoolean("saturday", x => x.saturday)
                .MapBoolean("sunday", x => x.sunday)
                .MapDate("start_date", x => x.start_date)
                .MapDate("end_date", x => x.end_date);

        private readonly PostgreSQLCopyHelper<Route> _routeCopyHelper =
            new PostgreSQLCopyHelper<Route>("public", "routes")
                .MapVarchar("route_id", x => x.route_id)
                .MapBigInt("agency_id", x => x.agency_id)
                .MapVarchar("route_short_name", x => x.route_short_name)
                .MapVarchar("route_long_name", x => x.route_long_name)
                .MapInteger("route_type", x => x.route_type)
                .MapVarchar("route_color", x => x.route_color)
                .MapVarchar("competent_authority", x => x.competent_authority);

        private readonly PostgreSQLCopyHelper<Stop> _stopCopyHelper =
            new PostgreSQLCopyHelper<Stop>("public", "stops")
                .MapInteger("stop_id", x => x.stop_id)
                .MapVarchar("stop_code", x => x.stop_code)
                .MapVarchar("stop_name", x => x.stop_name)
                .MapDouble("stop_lat", x => x.stop_lat)
                .MapDouble("stop_lon", x => x.stop_lon)
                .MapInteger("zone_id", x => x.zone_id)
                .MapVarchar("alias", x => x.alias)
                .MapVarchar("stop_area", x => x.stop_area)
                .MapVarchar("stop_desc", x => x.stop_desc)
                .MapNumeric("lest_x", x => x.lest_x)
                .MapNumeric("lest_y", x => x.lest_y)
                .MapVarchar("zone_name", x => x.zone_name);

        private readonly PostgreSQLCopyHelper<Trip> _tripCopyHelper =
            new PostgreSQLCopyHelper<Trip>("public", "trips")
                .MapVarchar("route_id", x => x.route_id)
                .MapInteger("service_id", x => x.service_id)
                .MapBigInt("trip_id", x => x.trip_id)
                .MapVarchar("trip_headsign", x => x.trip_headsign)
                .MapVarchar("trip_long_name", x => x.trip_long_name)
                .MapVarchar("direction_code", x => x.direction_code)
                .MapInteger("wheelchair_accessible", x => x.wheelchair_accessible)
                .MapInteger("shape_id", x => x.shape_id);

        private readonly PostgreSQLCopyHelper<StopTime> _stopTimeCopyHelper =
            new PostgreSQLCopyHelper<StopTime>("public", "stop_times")
                .MapBigInt("trip_id", x => x.trip_id)
                .MapVarchar("arrival_time", x => x.arrival_time)
                .MapVarchar("departure_time", x => x.departure_time)
                .MapInteger("stop_id", x => x.stop_id)
                .MapInteger("stop_sequence", x => x.stop_sequence)
                .MapInteger("pickup_type", x => x.pickup_type)
                .MapInteger("drop_off_type", x => x.drop_off_type);

        public GtfsImport(IHttpClientFactory factory, IConfiguration configuration)
        {
            _client = factory.CreateClient();
            _tempFolderPath = configuration.GetValue<string>("GtfsImportPath");
            _connectionString = configuration.GetConnectionString("Peatus");
        }

        public async Task WorkAsync()
        {
            if (string.IsNullOrWhiteSpace(_tempFolderPath)) return;

            var tempFolder = Directory.CreateDirectory(_tempFolderPath);
            if (!tempFolder.Exists) return;

            string zipPath = Path.Combine(tempFolder.FullName, "gtfs.zip");
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            string extractPath = Path.Combine(tempFolder.FullName, "gtfs");
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }

            var fileBytes = await _client.GetByteArrayAsync("http://peatus.ee/gtfs/gtfs.zip");
            File.WriteAllBytes(zipPath, fileBytes);

            ZipFile.ExtractToDirectory(zipPath, extractPath, true);

            var agenciesFilePath = Path.Combine(extractPath, "agency.txt");
            await TruncateTableAsync("agencies");
            await ImportFileAsync(agenciesFilePath, _agencyCopyHelper);

            var calendarFilePath = Path.Combine(extractPath, "calendar.txt");
            await TruncateTableAsync("calendar");
            await ImportFileAsync(calendarFilePath, _calendarCopyHelper,
                reader => { reader.Configuration.RegisterClassMap<CalendarMapping>(); });

            var routesFilePath = Path.Combine(extractPath, "routes.txt");
            await TruncateTableAsync("routes");
            await ImportFileAsync(routesFilePath, _routeCopyHelper);

            var stopsFilePath = Path.Combine(extractPath, "stops.txt");
            await TruncateTableAsync("stops");
            await ImportFileAsync(stopsFilePath, _stopCopyHelper);

            var tripsFilePath = Path.Combine(extractPath, "trips.txt");
            await TruncateTableAsync("trips");
            await ImportFileAsync(tripsFilePath, _tripCopyHelper);

            var stopTimesFilePath = Path.Combine(extractPath, "stop_times.txt");
            await TruncateTableAsync("stop_times");
            await ImportFileAsync(stopTimesFilePath, _stopTimeCopyHelper);

            await LogImportAsync();
        }

        private async Task LogImportAsync()
        {
            const string sql = @"insert into public.""import_logs""(""import_date"") values(now());";
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task TruncateTableAsync(string table)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand($"truncate {table} cascade;", connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task ImportFileAsync<T>(string filePath, IPostgreSQLCopyHelper<T> copyHelper,
            Action<CsvReader> configureReader = null)
        {
            using (var textReader = File.OpenText(filePath))
            {
                using (var csvReader = new CsvReader(textReader))
                {
                    configureReader?.Invoke(csvReader);

                    var items = csvReader.GetRecords<T>();
                    await InsertItemsAsync(items, copyHelper);
                }
            }
        }

        private async Task InsertItemsAsync<T>(IEnumerable<T> items, IPostgreSQLCopyHelper<T> copyHelper)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                copyHelper.SaveAll(connection, items);
            }
        }

        private class Agency
        {
            public long agency_id { get; set; }
            public string agency_name { get; set; }
            public string agency_url { get; set; }
            public string agency_timezone { get; set; }
            public string agency_phone { get; set; }
            public string agency_lang { get; set; }
        }

        private class Route
        {
            public string route_id { get; set; }
            public long? agency_id { get; set; }
            public string route_short_name { get; set; }
            public string route_long_name { get; set; }
            public int? route_type { get; set; }
            public string route_color { get; set; }
            public string competent_authority { get; set; }
        }

        private class Stop
        {
            public int stop_id { get; set; }
            public string stop_code { get; set; }
            public string stop_name { get; set; }
            public double? stop_lat { get; set; }
            public double? stop_lon { get; set; }
            public int? zone_id { get; set; }
            public string alias { get; set; }
            public string stop_area { get; set; }
            public string stop_desc { get; set; }
            public decimal? lest_x { get; set; }
            public decimal? lest_y { get; set; }
            public string zone_name { get; set; }
        }

        private class Trip
        {
            public string route_id { get; set; }
            public int? service_id { get; set; }
            public long trip_id { get; set; }
            public string trip_headsign { get; set; }
            public string trip_long_name { get; set; }
            public string direction_code { get; set; }
            public int? wheelchair_accessible { get; set; }
            public int? shape_id { get; set; }
        }

        private class StopTime
        {
            public long trip_id { get; set; }
            public string arrival_time { get; set; }
            public string departure_time { get; set; }
            public int stop_id { get; set; }
            public int stop_sequence { get; set; }
            public int? pickup_type { get; set; }
            public int? drop_off_type { get; set; }
        }

        private class Calendar
        {
            public int service_id { get; set; }
            public bool monday { get; set; }
            public bool tuesday { get; set; }
            public bool wednesday { get; set; }
            public bool thursday { get; set; }
            public bool friday { get; set; }
            public bool saturday { get; set; }
            public bool sunday { get; set; }
            public DateTime start_date { get; set; }
            public DateTime end_date { get; set; }
        }

        private sealed class CalendarMapping : ClassMap<Calendar>
        {
            public CalendarMapping()
            {
                AutoMap();
                Map(m => m.start_date).TypeConverterOption.Format("yyyyMMdd");
                Map(m => m.end_date).TypeConverterOption.Format("yyyyMMdd");
            }
        }
    }
}