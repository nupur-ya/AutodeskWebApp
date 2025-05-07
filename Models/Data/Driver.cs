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
}