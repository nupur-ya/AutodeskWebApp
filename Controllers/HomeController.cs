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
            
            // Redirect to the driver list page after editing
            return RedirectToAction("DriverList", "Home");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    public async Task<IActionResult> DriverList()
    {
        var drivers = await dbcontext.Drivers.ToListAsync();
        return View(drivers);
    }

    [HttpGet]
    public async Task<IActionResult> EditDriver(Guid id)
    {
        var driver = await dbcontext.Drivers.FindAsync(id);
        Debug.Assert(driver != null, "Driver not found in the database.");
        if (driver == null)
        {
            return NotFound();
        }
        return View(driver);
    }
    [HttpPost]
    public async Task<IActionResult> EditDriver(Driver driver)
    {
        var existingDriver = await dbcontext.Drivers.FindAsync(driver.Id);
        if (existingDriver == null)
        {
            return NotFound();
        }
        // Update the existing driver properties with the new values
        existingDriver.Name = driver.Name;
        existingDriver.Email = driver.Email;
        existingDriver.Phone = driver.Phone;
        existingDriver.Team = driver.Team;
        existingDriver.HomeCountry = driver.HomeCountry;
        existingDriver.IsRacingThisYear = driver.IsRacingThisYear;

        // Save the changes to the database
        await dbcontext.SaveChangesAsync();

        // Redirect to the driver list page after editing
        return RedirectToAction("DriverList", "Home");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteDriver(Guid id)
    {
        var driver = await dbcontext.Drivers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (driver == null)
        {
            return NotFound();
        }

        dbcontext.Drivers.Remove(driver);
        await dbcontext.SaveChangesAsync();

        return RedirectToAction("DriverList", "Home");
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
