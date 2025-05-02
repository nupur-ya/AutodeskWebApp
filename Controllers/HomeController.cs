using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AutodeskWebApp.Models;
using AutodeskWebApp.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace AutodeskWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DriverData dbcontext;

    public HomeController(DriverData context)
    {
        this.dbcontext = context;
    }
    
    [HttpGet]
    public IActionResult AddDriver()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddDriver(Driver driver)
    {
        try
        {
            var driverObj = new Driver
            {
                Id = Guid.NewGuid(), // Generate a new GUID for the driver ID
                Name = driver.Name,
                Email = driver.Email,
                Phone = driver.Phone,
                Team = driver.Team,
                HomeCountry = driver.HomeCountry,
                IsRacingThisYear = driver.IsRacingThisYear
            };

            await dbcontext.Drivers.AddAsync(driverObj);
            await dbcontext.SaveChangesAsync(); // Save changes to the database

            Console.WriteLine(driver.Name);
            return View();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
   

    public IActionResult Index()
    {
        return View();
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
