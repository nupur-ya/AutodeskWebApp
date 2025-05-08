using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AutodeskWebApp.Models;
using AutodeskWebApp.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http.Extensions;
using System.Globalization;

namespace AutodeskWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DriverData dbcontext;
    public List<Race> race;
    private Dictionary<Guid, string> driverHeadhsotGuids = new Dictionary<Guid, string>();
    private HttpClient client = new HttpClient();

    private readonly string driversUrl = "https://api.openf1.org/v1/drivers";
    private readonly string raceSessionsUrl = "https://api.openf1.org/v1/sessions";

    public HomeController(DriverData context)
    {
        this.dbcontext = context;
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
    
    [HttpGet]
    public IActionResult AddDriver()
    {
        return View();
    }

    /// <summary>
    /// Adds a new driver to the database using POST request.
    /// <param name="driver">The driver object to add.</param>
    /// </summary>
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
                IsRacingThisYear = driver.IsRacingThisYear,
                ImageUrl = driver.ImageUrl,
                DriverNumber = driver.DriverNumber
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

    /// <summary>
    /// Gets a list from drivers from the database using GET request.
    /// Gets all drivers if no driver number or ID is provided.
    /// <param name="driverNumber">Optional: The Driver Number to search for.</param>
    /// <param name="driverId">Optional: The Driver ID to search for.</param>
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> DriverList(int? driverNumber, Guid? driverId)
    {

        List<Driver> drivers = new List<Driver>();

        try
        {
            // Initialize the dictionary to cache the driver headshot URLs so we don't fetch them every time
            if (driverHeadhsotGuids.Count == 0)
            {
                foreach (var driver in await dbcontext.Drivers.ToListAsync())
                {
                    driverHeadhsotGuids[driver.Id] = driver.ImageUrl;
                }
            }

            // Get driver by driver number
            if (driverNumber != null)
            {
                drivers = await dbcontext.Drivers.Where(driver => driver.DriverNumber == driverNumber).ToListAsync();
                // If driver number is not found, return all drivers
                if (driverNumber == 0) 
                {
                    drivers = await dbcontext.Drivers.ToListAsync();
                }
            }
            // Get driver by driver ID
            else if (driverId != null)
            {
                drivers = await dbcontext.Drivers.Where(driver => driver.Id == driverId).ToListAsync();
                // If driver number is not found, return all drivers
                if (driverId == Guid.Empty) 
                {
                    drivers = await dbcontext.Drivers.ToListAsync();
                }
            }
            else
            {
                // Get all drivers
                drivers = await dbcontext.Drivers.ToListAsync();
            }
            
            // Sort by driver number
            drivers = drivers.OrderBy(driver => driver.DriverNumber).ToList();
            return View(drivers);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    /// <summary>
    /// Gets a list of teams from the database using GET request.
    /// Gets all teams if no team rank or '0' is provided.
    /// <param name="teamRank">Optional: The Team Rank to search for.</param>
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> TeamList(int teamRank)
    {
        try
        {
            // Cache the driver headshot URLs if not already cached
            if (driverHeadhsotGuids.Count == 0) // TODO: fix for possible bug - if a new driver is added to the db and team after first caching
            {
                foreach (var driver in await dbcontext.Drivers.ToListAsync())
                {
                    driverHeadhsotGuids[driver.Id] = driver.ImageUrl;
                }
            }

            List<TeamView> teamViews = new List<TeamView>();
            List<Team> teams = new List<Team>();

            // Get all teams if teamRank is 0 or no teamRank provided, otherwise filter by teamRank
            teams = teamRank == 0 ? await dbcontext.Teams.ToListAsync() : await dbcontext.Teams.Where(team => team.Rank == teamRank).ToListAsync();

            // Initialize TeamView objects
            foreach (var team in teams)
            {
                var teamView = new TeamView
                {
                    Id = team.Id,
                    Name = team.Name,
                    Driver1 = driverHeadhsotGuids.TryGetValue(team.Driver1, out string? value) ? value : "Unknown",
                    Driver1Number = team.Driver1,
                    Driver2 = driverHeadhsotGuids.TryGetValue(team.Driver2, out string? value1) ? value1 : "Unknown",
                    Driver2Number = team.Driver2,
                    TeamChief = team.TeamChief,
                    CarImageUrl = team.CarImageUrl,
                    Rank = team.Rank,
                    PolePositions = team.PolePositions
                };
                Console.WriteLine($"Team: {team.CarImageUrl}");
                teamViews.Add(teamView);
            }

            // Sort by team ranking
            teamViews = teamViews.OrderBy(team => team.Rank).ToList();

            return View(teamViews);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    /// <summary>
    /// Gets the driver to be edited from the database using GET request.
    /// Redirects to error page if driver not found.
    /// <param name="id">The DriverID to search for.</param>
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EditDriver(Guid id)
    {
        try
        {
            var driver = await dbcontext.Drivers.FindAsync(id);
            // Return to error page if driver not found
            if (driver == null)
            {
                return RedirectToErrorPage("Driver not found. Make sure the driver exists");
            }

            // Return the driver object to the view for editing
            return View(driver);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    /// <summary>
    /// Sends the updated driver object to the database using POST request.
    /// <param name="driver">The Driver object to send.</param>
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> EditDriver(Driver driver)
    {
        try
        {
            var existingDriver = await dbcontext.Drivers.FindAsync(driver.Id);
            // Check if the driver exists
            if (existingDriver == null)
            {
                // Return to error page if driver not found
                return RedirectToErrorPage("Driver not found. Make sure the Driver ID is correct.");
            }

            // Update the existing driver properties with the new values
            existingDriver.Name = driver.Name;
            existingDriver.Email = driver.Email;
            existingDriver.Phone = driver.Phone;
            existingDriver.Team = driver.Team;
            existingDriver.HomeCountry = driver.HomeCountry;
            existingDriver.IsRacingThisYear = driver.IsRacingThisYear;
            existingDriver.ImageUrl = driver.ImageUrl;
            existingDriver.DriverNumber = driver.DriverNumber;

            // Save the changes to the database
            await dbcontext.SaveChangesAsync();

            // Redirect to the driver list page after editing
            return RedirectToAction("DriverList", "Home");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    /// <summary>
    /// Deletes the driver from the database using POST request.
    /// Redirects to DriverList page after deletion.
    /// <param name="id">The DriverID to delete.</param>
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DeleteDriver(Guid id)
    {
        try
        {
            var driver = await dbcontext.Drivers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (driver == null)
            {
                // Return to error page if driver not found
                return RedirectToErrorPage("Driver doesn't exist.");
            }

            // Delete the driver from the database and save changes
            dbcontext.Drivers.Remove(driver);
            await dbcontext.SaveChangesAsync();

            // Return to the driver list page after deletion
            return RedirectToAction("DriverList", "Home");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


    /// <summary>
    /// Gets the race list from a public API using GET request.
    /// Redirects to PreviousRaces page to display the table.
    /// <param name="id">The DriverID to delete.</param>
    /// </summary> 
    [HttpGet]
   public async Task<IActionResult> RaceList(DateOnly? startDate, DateOnly? endDate)
   {   
        try
        {
            // Validate date
            if (startDate == null) startDate = DateOnly.MinValue;
            if (endDate == null) endDate = DateOnly.FromDateTime(DateTime.Now);

            // Validate date range
            if (startDate > endDate || startDate < DateOnly.MinValue || endDate > DateOnly.MaxValue)
            {
                // Return to error page if date range is invalid
                return RedirectToErrorPage("Invalid date range.");
            }
            
            // Setup URL
            // TODO: create URL using QueryBuilder
            // var qb = new QueryBuilder();
            // qb.Add("date_start%3E%3D", startDate?.ToString("yyyy-MM-dd"));
            // qb.Add("date_end%3C%3D", endDate?.ToString("yyyy-MM-dd"));
            
            var raceListUrl = raceSessionsUrl + $"?date_start%3E%3D{startDate}&date_end%3C%3D{endDate}";
            
            HttpResponseMessage response = await client.GetAsync(raceListUrl);
        
            // Validate response
            if (response.IsSuccessStatusCode)
            {
                race = await response.Content.ReadAsAsync<List<Race>>();
            }
            else
            {
                return RedirectToErrorPage(response.StatusCode.ToString());
            }
            
            // Redirect to the RaceList View to display the table
            return View(race);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
   }

    /// <summary>
    /// Initializes the database with driver data from a public API using GET request.
    /// Redirects to DriverList page after initialization.
    /// </summary> 
    [HttpGet]
    private async Task<IActionResult> InitDatabase()
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(driversUrl);

            // Validate response
            if (response.IsSuccessStatusCode)
            {
                var drivers = await response.Content.ReadAsAsync<List<JsonDriverData>>();

                foreach (var driver in drivers)
                {
                    // The API has a few drivers with null headshot URLs, so we need to validate if it's null
                    if (driver.headshot_url != null)
                    {
                        // Popoulate the driver object with data from the API
                        var driverObj = new Driver
                        {
                            Id = Guid.NewGuid(), // Generate a new GUID for the driver ID
                            Name = driver.full_name,
                            Email = driver.name_acronym + "@" + driver.team_name + ".com",
                            Phone = driver.session_key.ToString(),
                            Team = driver.team_name,
                            HomeCountry = driver.country_code,
                            IsRacingThisYear = true,
                            ImageUrl = driver.headshot_url,
                            DriverNumber = driver.driver_number
                        };

                        // Add driver to the database and save changes
                        await dbcontext.Drivers.AddAsync(driverObj);
                        await dbcontext.SaveChangesAsync();
                    }
                }

                // Redirect to the driver list page after editing
                return RedirectToAction("DriverList", "Home");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return View("Error", new ErrorViewModel { ErrorMessage = $"Error: {response.StatusCode}" });
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

    /// <summary>
    /// Redirects to Error page and logs the error message.
    /// <param name="errorMessage">The error message to log.</param>
    /// </summary> 
    private ViewResult RedirectToErrorPage(string errorMessage)
    {
        Console.WriteLine(errorMessage);
        return View("Error", new ErrorViewModel { ErrorMessage = errorMessage });
    }
}
