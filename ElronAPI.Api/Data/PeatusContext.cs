using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ElronAPI.Api.Data
{
    public partial class PeatusContext : DbContext
    {
        public PeatusContext()
        {
        }

        public PeatusContext(DbContextOptions<PeatusContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agency> Agencies { get; set; }
        public virtual DbSet<Calendar> Calendar { get; set; }
        public virtual DbSet<ImportLog> ImportLogs { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<Stop> Stops { get; set; }
        public virtual DbSet<StopTime> StopTimes { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agency>(entity =>
            {
                entity.HasKey(e => e.AgencyId);

                entity.ToTable("agencies");

                entity.Property(e => e.AgencyId)
                    .HasColumnName("agency_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.AgencyLang)
                    .HasColumnName("agency_lang")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.AgencyName)
                    .HasColumnName("agency_name")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.AgencyPhone)
                    .HasColumnName("agency_phone")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.AgencyTimezone)
                    .HasColumnName("agency_timezone")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.AgencyUrl)
                    .HasColumnName("agency_url")
                    .HasColumnType("character varying(255)");
            });

            modelBuilder.Entity<Calendar>(entity =>
            {
                entity.HasKey(e => e.ServiceId);

                entity.ToTable("calendar");

                entity.Property(e => e.ServiceId)
                    .HasColumnName("service_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("date");

                entity.Property(e => e.Friday).HasColumnName("friday");

                entity.Property(e => e.Monday).HasColumnName("monday");

                entity.Property(e => e.Saturday).HasColumnName("saturday");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("date");

                entity.Property(e => e.Sunday).HasColumnName("sunday");

                entity.Property(e => e.Thursday).HasColumnName("thursday");

                entity.Property(e => e.Tuesday).HasColumnName("tuesday");

                entity.Property(e => e.Wednesday).HasColumnName("wednesday");
            });

            modelBuilder.Entity<ImportLog>(entity =>
            {
                entity.ToTable("import_logs");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('importlogs_id_seq'::regclass)");

                entity.Property(e => e.ImportDate)
                    .HasColumnName("import_date")
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.HasKey(e => e.RouteId);

                entity.ToTable("routes");

                entity.Property(e => e.RouteId)
                    .HasColumnName("route_id")
                    .HasColumnType("character varying(32)")
                    .ValueGeneratedNever();

                entity.Property(e => e.AgencyId).HasColumnName("agency_id");

                entity.Property(e => e.CompetentAuthority)
                    .HasColumnName("competent_authority")
                    .HasColumnType("character varying(32)");

                entity.Property(e => e.RouteColor)
                    .HasColumnName("route_color")
                    .HasColumnType("character varying(16)");

                entity.Property(e => e.RouteLongName)
                    .HasColumnName("route_long_name")
                    .HasColumnType("character varying(256)");

                entity.Property(e => e.RouteShortName)
                    .HasColumnName("route_short_name")
                    .HasColumnType("character varying(16)");

                entity.Property(e => e.RouteType).HasColumnName("route_type");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.Routes)
                    .HasForeignKey(d => d.AgencyId)
                    .HasConstraintName("routes_agencies_fkey");
            });

            modelBuilder.Entity<Stop>(entity =>
            {
                entity.HasKey(e => e.StopId);

                entity.ToTable("stops");

                entity.Property(e => e.StopId)
                    .HasColumnName("stop_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Alias)
                    .HasColumnName("alias")
                    .HasColumnType("character varying(32)");

                entity.Property(e => e.LestX)
                    .HasColumnName("lest_x")
                    .HasColumnType("numeric(19,0)");

                entity.Property(e => e.LestY)
                    .HasColumnName("lest_y")
                    .HasColumnType("numeric(19,0)");

                entity.Property(e => e.StopArea)
                    .HasColumnName("stop_area")
                    .HasColumnType("character varying(32)");

                entity.Property(e => e.StopCode)
                    .HasColumnName("stop_code")
                    .HasColumnType("character varying(16)");

                entity.Property(e => e.StopDesc)
                    .HasColumnName("stop_desc")
                    .HasColumnType("character varying(64)");

                entity.Property(e => e.StopLat).HasColumnName("stop_lat");

                entity.Property(e => e.StopLon).HasColumnName("stop_lon");

                entity.Property(e => e.StopName)
                    .HasColumnName("stop_name")
                    .HasColumnType("character varying(64)");

                entity.Property(e => e.ZoneId).HasColumnName("zone_id");

                entity.Property(e => e.ZoneName)
                    .HasColumnName("zone_name")
                    .HasColumnType("character varying(100)");
            });

            modelBuilder.Entity<StopTime>(entity =>
            {
                entity.HasKey(e => new { e.TripId, e.StopId, e.StopSequence });

                entity.ToTable("stop_times");

                entity.Property(e => e.TripId).HasColumnName("trip_id");

                entity.Property(e => e.StopId).HasColumnName("stop_id");

                entity.Property(e => e.StopSequence).HasColumnName("stop_sequence");

                entity.Property(e => e.ArrivalTime)
                    .HasColumnName("arrival_time")
                    .HasColumnType("character varying(16)");

                entity.Property(e => e.DepartureTime)
                    .HasColumnName("departure_time")
                    .HasColumnType("character varying(16)");

                entity.Property(e => e.DropOffType).HasColumnName("drop_off_type");

                entity.Property(e => e.PickupType).HasColumnName("pickup_type");

                entity.HasOne(d => d.Stop)
                    .WithMany(p => p.StopTimes)
                    .HasForeignKey(d => d.StopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("stop_times_stops_fkey");

                entity.HasOne(d => d.Trip)
                    .WithMany(p => p.StopTimes)
                    .HasForeignKey(d => d.TripId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("stop_times_trips_fkey");
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.TripId);

                entity.ToTable("trips");

                entity.Property(e => e.TripId)
                    .HasColumnName("trip_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.DirectionCode)
                    .HasColumnName("direction_code")
                    .HasColumnType("character varying(32)");

                entity.Property(e => e.RouteId)
                    .IsRequired()
                    .HasColumnName("route_id")
                    .HasColumnType("character varying(32)");

                entity.Property(e => e.ServiceId).HasColumnName("service_id");

                entity.Property(e => e.ShapeId).HasColumnName("shape_id");

                entity.Property(e => e.TripHeadsign)
                    .HasColumnName("trip_headsign")
                    .HasColumnType("character varying(64)");

                entity.Property(e => e.TripLongName)
                    .HasColumnName("trip_long_name")
                    .HasColumnType("character varying(256)");

                entity.Property(e => e.WheelchairAccessible).HasColumnName("wheelchair_accessible");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.Trips)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("trips_routes_fkey");
            });

            modelBuilder.HasSequence("importlogs_id_seq");
        }
    }
}
