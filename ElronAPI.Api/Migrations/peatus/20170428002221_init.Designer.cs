using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ElronAPI.Models;

namespace ElronAPI.Migrations.peatus
{
    [DbContext(typeof(PeatusContext))]
    [Migration("20170428002221_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("ElronAPI.Models.Agency", b =>
                {
                    b.Property<long>("AgencyId")
                        .HasColumnName("agency_id");

                    b.Property<string>("AgencyLang")
                        .HasColumnName("agency_lang")
                        .HasColumnType("varchar")
                        .HasMaxLength(255);

                    b.Property<string>("AgencyName")
                        .HasColumnName("agency_name")
                        .HasColumnType("varchar")
                        .HasMaxLength(255);

                    b.Property<string>("AgencyPhone")
                        .HasColumnName("agency_phone")
                        .HasColumnType("varchar")
                        .HasMaxLength(255);

                    b.Property<string>("AgencyTimezone")
                        .HasColumnName("agency_timezone")
                        .HasColumnType("varchar")
                        .HasMaxLength(255);

                    b.Property<string>("AgencyUrl")
                        .HasColumnName("agency_url")
                        .HasColumnType("varchar")
                        .HasMaxLength(255);

                    b.HasKey("AgencyId")
                        .HasName("PK_agencies");

                    b.ToTable("agencies");
                });

            modelBuilder.Entity("ElronAPI.Models.Calendar", b =>
                {
                    b.Property<int>("ServiceId")
                        .HasColumnName("service_id");

                    b.Property<DateTime>("EndDate")
                        .HasColumnName("end_date")
                        .HasColumnType("date");

                    b.Property<bool>("Friday")
                        .HasColumnName("friday");

                    b.Property<bool>("Monday")
                        .HasColumnName("monday");

                    b.Property<bool>("Saturday")
                        .HasColumnName("saturday");

                    b.Property<DateTime>("StartDate")
                        .HasColumnName("start_date")
                        .HasColumnType("date");

                    b.Property<bool>("Sunday")
                        .HasColumnName("sunday");

                    b.Property<bool>("Thursday")
                        .HasColumnName("thursday");

                    b.Property<bool>("Tuesday")
                        .HasColumnName("tuesday");

                    b.Property<bool>("Wednesday")
                        .HasColumnName("wednesday");

                    b.HasKey("ServiceId")
                        .HasName("PK_calendar");

                    b.ToTable("calendar");
                });

            modelBuilder.Entity("ElronAPI.Models.Route", b =>
                {
                    b.Property<string>("RouteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("route_id")
                        .HasColumnType("varchar")
                        .HasMaxLength(32);

                    b.Property<long?>("AgencyId")
                        .HasColumnName("agency_id");

                    b.Property<string>("CompetentAuthority")
                        .HasColumnName("competent_authority")
                        .HasColumnType("varchar")
                        .HasMaxLength(32);

                    b.Property<string>("RouteColor")
                        .HasColumnName("route_color")
                        .HasColumnType("varchar")
                        .HasMaxLength(16);

                    b.Property<string>("RouteLongName")
                        .HasColumnName("route_long_name")
                        .HasColumnType("varchar")
                        .HasMaxLength(256);

                    b.Property<string>("RouteShortName")
                        .HasColumnName("route_short_name")
                        .HasColumnType("varchar")
                        .HasMaxLength(16);

                    b.Property<int?>("RouteType")
                        .HasColumnName("route_type");

                    b.HasKey("RouteId")
                        .HasName("PK_routes");

                    b.HasIndex("AgencyId");

                    b.ToTable("routes");
                });

            modelBuilder.Entity("ElronAPI.Models.Stop", b =>
                {
                    b.Property<int>("StopId")
                        .HasColumnName("stop_id");

                    b.Property<string>("Alias")
                        .HasColumnName("alias")
                        .HasColumnType("varchar")
                        .HasMaxLength(32);

                    b.Property<decimal?>("LestX")
                        .HasColumnName("lest_x");

                    b.Property<decimal?>("LestY")
                        .HasColumnName("lest_y");

                    b.Property<string>("StopArea")
                        .HasColumnName("stop_area")
                        .HasColumnType("varchar")
                        .HasMaxLength(32);

                    b.Property<string>("StopCode")
                        .HasColumnName("stop_code")
                        .HasColumnType("varchar")
                        .HasMaxLength(16);

                    b.Property<string>("StopDesc")
                        .HasColumnName("stop_desc")
                        .HasColumnType("varchar")
                        .HasMaxLength(64);

                    b.Property<double?>("StopLat")
                        .HasColumnName("stop_lat");

                    b.Property<double?>("StopLon")
                        .HasColumnName("stop_lon");

                    b.Property<string>("StopName")
                        .HasColumnName("stop_name")
                        .HasColumnType("varchar")
                        .HasMaxLength(64);

                    b.Property<int?>("ZoneId")
                        .HasColumnName("zone_id");

                    b.Property<string>("ZoneName")
                        .HasColumnName("zone_name")
                        .HasColumnType("varchar")
                        .HasMaxLength(100);

                    b.HasKey("StopId")
                        .HasName("PK_stops");

                    b.ToTable("stops");
                });

            modelBuilder.Entity("ElronAPI.Models.StopTime", b =>
                {
                    b.Property<long>("TripId")
                        .HasColumnName("trip_id");

                    b.Property<int>("StopId")
                        .HasColumnName("stop_id");

                    b.Property<int>("StopSequence")
                        .HasColumnName("stop_sequence");

                    b.Property<string>("ArrivalTime")
                        .HasColumnName("arrival_time")
                        .HasColumnType("varchar")
                        .HasMaxLength(16);

                    b.Property<string>("DepartureTime")
                        .HasColumnName("departure_time")
                        .HasColumnType("varchar")
                        .HasMaxLength(16);

                    b.Property<int?>("DropOffType")
                        .HasColumnName("drop_off_type");

                    b.Property<int?>("PickupType")
                        .HasColumnName("pickup_type");

                    b.HasKey("TripId", "StopId", "StopSequence")
                        .HasName("PK_stop_times");

                    b.HasIndex("StopId");

                    b.ToTable("stop_times");
                });

            modelBuilder.Entity("ElronAPI.Models.Trip", b =>
                {
                    b.Property<long>("TripId")
                        .HasColumnName("trip_id");

                    b.Property<string>("DirectionCode")
                        .HasColumnName("direction_code")
                        .HasColumnType("varchar")
                        .HasMaxLength(32);

                    b.Property<string>("RouteId")
                        .IsRequired()
                        .HasColumnName("route_id")
                        .HasColumnType("varchar")
                        .HasMaxLength(32);

                    b.Property<int?>("ServiceId")
                        .HasColumnName("service_id");

                    b.Property<int?>("ShapeId")
                        .HasColumnName("shape_id");

                    b.Property<string>("TripHeadsign")
                        .HasColumnName("trip_headsign")
                        .HasColumnType("varchar")
                        .HasMaxLength(64);

                    b.Property<string>("TripLongName")
                        .HasColumnName("trip_long_name")
                        .HasColumnType("varchar")
                        .HasMaxLength(256);

                    b.Property<int?>("WheelchairAccessible")
                        .HasColumnName("wheelchair_accessible");

                    b.HasKey("TripId")
                        .HasName("PK_trips");

                    b.HasIndex("RouteId");

                    b.ToTable("trips");
                });

            modelBuilder.Entity("ElronAPI.Models.Route", b =>
                {
                    b.HasOne("ElronAPI.Models.Agency", "Agency")
                        .WithMany("Routes")
                        .HasForeignKey("AgencyId")
                        .HasConstraintName("routes_agencies_fkey");
                });

            modelBuilder.Entity("ElronAPI.Models.StopTime", b =>
                {
                    b.HasOne("ElronAPI.Models.Stop", "Stop")
                        .WithMany("StopTimes")
                        .HasForeignKey("StopId")
                        .HasConstraintName("stop_times_stops_fkey");

                    b.HasOne("ElronAPI.Models.Trip", "Trip")
                        .WithMany("StopTimes")
                        .HasForeignKey("TripId")
                        .HasConstraintName("stop_times_trips_fkey");
                });

            modelBuilder.Entity("ElronAPI.Models.Trip", b =>
                {
                    b.HasOne("ElronAPI.Models.Route", "Route")
                        .WithMany("Trips")
                        .HasForeignKey("RouteId")
                        .HasConstraintName("trips_routes_fkey");
                });
        }
    }
}
