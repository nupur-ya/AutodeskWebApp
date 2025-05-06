using Microsoft.EntityFrameworkCore;

namespace AutodeskWebApp.Models.Data
{
    public class Race
    {
        public int circuit_key { get; set; }
        public string circuit_short_name { get; set; }
        public string country_code { get; set; }
        public int country_key { get; set; }
        public string country_name { get; set; }
        public DateTime date_end { get; set; }
        public DateTime date_start { get; set; }
        public TimeSpan gmt_offset{ get; set; }
        public string location { get; set; }
        public int meeting_key { get; set; }
        public int session_key { get; set; }
        public string session_name { get; set; }
        public string session_type { get; set; }
        public int year { get; set; }
    }
}
