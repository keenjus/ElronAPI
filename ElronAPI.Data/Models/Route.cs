using System.Collections.Generic;

namespace ElronAPI.Data.Models
{
    public partial class Route
    {
        public Route()
        {
            Trips = new HashSet<Trip>();
        }

        public string RouteId { get; set; }
        public long? AgencyId { get; set; }
        public string RouteShortName { get; set; }
        public string RouteLongName { get; set; }
        public int? RouteType { get; set; }
        public string RouteColor { get; set; }
        public string CompetentAuthority { get; set; }

        public virtual ICollection<Trip> Trips { get; set; }
        public virtual Agency Agency { get; set; }
    }
}
