using Microsoft.EntityFrameworkCore;

namespace AutodeskWebApp.Models.Data
{
    public class DriverData : DbContext
    {
        public DriverData(DbContextOptions<DriverData> options) : base(options)
        {
        }

        public DbSet<Driver> Drivers { get; set; } // SQL table name: Drivers

    }
} 