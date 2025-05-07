namespace AutodeskWebApp.Models.Data
{
    public class Driver
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Team { get; set; }
        public string HomeCountry { get; set; }
        public bool IsRacingThisYear { get; set; }
        public string ImageUrl { get; set; }
        public int DriverNumber { get; set; }
    }

    public class JsonDriverData{
        public int driver_number { get; set; }
        public string broadcast_name { get; set; }
        public string full_name { get; set; }
        public string name_acronym { get; set; }
        public string team_name { get; set; }
        public string team_colour { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string headshot_url { get; set; }
        public string country_code { get; set; }
        public int session_key { get; set; }
        public int meeting_key { get; set; }
    }
}