namespace ElronAPI.Domain.Models
{
    public class TrainTimeModel
    {
        public int? ServiceId { get; set; }
        public string RouteLongName { get; set; }
        public string RouteShortName { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}