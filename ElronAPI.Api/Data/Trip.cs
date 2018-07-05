using System.Collections.Generic;

namespace ElronAPI.Api.Data
{
    public partial class Trip
    {
        public Trip()
        {
            StopTimes = new HashSet<StopTime>();
        }

        public string RouteId { get; set; }
        public int? ServiceId { get; set; }
        public long TripId { get; set; }
        public string TripHeadsign { get; set; }
        public string TripLongName { get; set; }
        public string DirectionCode { get; set; }
        public int? WheelchairAccessible { get; set; }
        public int? ShapeId { get; set; }

        public virtual ICollection<StopTime> StopTimes { get; set; }
        public virtual Route Route { get; set; }
    }
}
