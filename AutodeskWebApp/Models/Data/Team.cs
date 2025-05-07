namespace AutodeskWebApp.Models.Data
{
    public class Team
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid Driver1 { get; set; }
        public Guid Driver2 { get; set; }
        public required string TeamChief { get; set; }
        public required string CarImageUrl { get; set; }
        public int Rank { get; set; }
        public int PolePositions { get; set; }
    }

    public class TeamView
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Driver1 { get; set; }
        public Guid Driver1Number { get; set; }
        public required string Driver2 { get; set; }
        public Guid Driver2Number { get; set; }
        public required string TeamChief { get; set; }
        public required string CarImageUrl { get; set; }
        public int Rank { get; set; }
        public int PolePositions { get; set; }
    }
}