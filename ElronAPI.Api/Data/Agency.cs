using System.Collections.Generic;

namespace ElronAPI.Api.Data
{
    public partial class Agency
    {
        public Agency()
        {
            Routes = new HashSet<Route>();
        }

        public long AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string AgencyUrl { get; set; }
        public string AgencyTimezone { get; set; }
        public string AgencyPhone { get; set; }
        public string AgencyLang { get; set; }

        public virtual ICollection<Route> Routes { get; set; }
    }
}
