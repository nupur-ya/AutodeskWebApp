namespace AutodeskWebApp.Models.Data
{
    public class Driver
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Team { get; set; }
        public required string HomeCountry { get; set; }
        public bool IsRacingThisYear { get; set; }
        public required string ImageUrl { get; set; }
        public int DriverNumber { get; set; }
    }

    public class JsonDriverData{
        public int driver_number { get; set; }
        public required string broadcast_name { get; set; }
        public required string full_name { get; set; }
        public required string name_acronym { get; set; }
        public required string team_name { get; set; }
        public required string team_colour { get; set; }
        public required string first_name { get; set; }
        public required string last_name { get; set; }
        public required string headshot_url { get; set; }
        public required string country_code { get; set; }
        public int session_key { get; set; }
        public int meeting_key { get; set; }
    }
}