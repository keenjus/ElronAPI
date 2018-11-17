namespace ElronAPI.Domain.Classifiers
{
    public class AgencyType
    {
        public int Id { get; }

        private AgencyType(int id)
        {
            Id = id;
        }

        public static readonly AgencyType Elron = new AgencyType(82);
    }
}