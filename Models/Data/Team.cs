namespace AutodeskWebApp.Models.Data
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Driver> Drivers{ get; set; }
        public string TeamChief { get; set; }
        public string CarImageUrl { get; set; }
        public int Rank { get; set; }
        public int PolePositions { get; set; }
    }
}