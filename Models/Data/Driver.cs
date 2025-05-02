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
    }
    public class Vehicle
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
    }
}