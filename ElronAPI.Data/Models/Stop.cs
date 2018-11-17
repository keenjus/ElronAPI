using System.Collections.Generic;

namespace ElronAPI.Data.Models
{
    public partial class Stop
    {
        public Stop()
        {
            StopTimes = new HashSet<StopTime>();
        }

        public int StopId { get; set; }
        public string StopCode { get; set; }
        public string StopName { get; set; }
        public double? StopLat { get; set; }
        public double? StopLon { get; set; }
        public int? ZoneId { get; set; }
        public string Alias { get; set; }
        public string StopArea { get; set; }
        public string StopDesc { get; set; }
        public decimal? LestX { get; set; }
        public decimal? LestY { get; set; }
        public string ZoneName { get; set; }

        public virtual ICollection<StopTime> StopTimes { get; set; }
    }
}
