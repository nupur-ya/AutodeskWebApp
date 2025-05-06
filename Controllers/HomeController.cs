using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AutodeskWebApp.Models;
using AutodeskWebApp.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Formatting;

namespace AutodeskWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DriverData dbcontext;
    public List<Race> race;
    private HttpClient client = new HttpClient();

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
   
   [HttpGet]
   public async Task<IActionResult> RaceList(DateOnly? startDate, DateOnly? endDate)
   {   
        try
        {
            // Validate date
            if (startDate == null) startDate = DateOnly.MinValue;
            if (endDate == null) endDate = DateOnly.FromDateTime(DateTime.Now);

            if (startDate > endDate)
            {
                Console.WriteLine("Start date must be before end date.");
                return View("Error", new ErrorViewModel { ErrorMessage = "Start date must be before end date." });
            }
            
            // Setup URL
            var raceListUrl = $"https://api.openf1.org/v1/sessions?date_start%3E%3D{startDate}&date_end%3C%3D{endDate}";
            
            HttpResponseMessage response = await client.GetAsync(raceListUrl);
        
            // Validate response
            if (response.IsSuccessStatusCode)
            {
                race = await response.Content.ReadAsAsync<List<Race>>();
            }
            else{
                Console.WriteLine($"Error: {response.StatusCode}");
                return View("Error", new ErrorViewModel { ErrorMessage = $"Error: {response.StatusCode}" });
            }
            
            return View(race);
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
