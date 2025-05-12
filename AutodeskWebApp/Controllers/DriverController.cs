using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AutodeskWebApp.Models;
using AutodeskWebApp.Models.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace AutodeskWebApp.Controllers;

[Route("Driver")]
public class DriverController : Controller
{
    private readonly ILogger<DriverController> _logger;
    private readonly DriverData dbcontext;

    public DriverController(ILogger<DriverController> logger, DriverData context)
    {
        _logger = logger;
        this.dbcontext = context;
    }

     /// <summary>
    /// API to adds a new driver to the database using POST request.
    /// <param name="driver">The driver object to add.</param>
    /// <returns>Status Code and message.</returns>
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Driver driver)
    {
        try
        {
            var driverExists = await DriverExists(driver.Name, driver.DriverNumber);
            if (driverExists)
            {
                // Driver with same name and DriverNumber already exists
                // return error code 409 : conflict error
                return StatusCode(409);
            }


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
            

            // Confirm addition to database and return status
            return CreatedAtAction(nameof(Find), new { driverId = driverObj.Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new {Message = "Exception occurred: " + ex.Message});
        }
    }

    /// <summary>
    /// GET API to find a driver using their DriverID.
    /// Can access via https://f1driversportal-bpbzhjebbfahe5fj.canadacentral-01.azurewebsites.net/driver/{driverID}
    /// <param name="driverId">The Driver ID to search for.</param>
    /// <returns>Status Code and message.</returns>
    /// </summary>

    [HttpGet("{driverId}")]
    public async Task<IActionResult> Find(Guid driverId)
    {

        try
        {
          if (driverId != Guid.Empty)
            {
                
                var drivers = await dbcontext.Drivers.FirstOrDefaultAsync(driver => driver.Id == driverId);

                if (drivers == null) return NotFound();
            
                return Ok(drivers);
            }

            // Empty or invalid GUID
            // Return error code 422 : Unprocessable Entity
            return StatusCode(422); 
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new {Message = "Exception occurred: " + ex.Message});
        }
    }

    /// <summary>
    /// DELETE API to delete a driver using their DriverID. Also checks if driver exists.
    /// Can access via https://f1driversportal-bpbzhjebbfahe5fj.canadacentral-01.azurewebsites.net/driver/{driverID}
    /// <param name="driverId">The Driver ID to delete.</param>
    /// <returns>Status Code and message.</returns>
    /// </summary>
    /// 
    [HttpDelete("{driverId}")]
    public async Task<IActionResult> Delete(Guid driverId)
    {

        try
        {
            if (driverId != Guid.Empty)
            {
                
                var drivers = await dbcontext.Drivers.FirstOrDefaultAsync(driver => driver.Id == driverId);

                if (drivers == null) return StatusCode(404, new {Message = "Driver ID not found: " + driverId});
            
                dbcontext.Drivers.Remove(drivers);
                await dbcontext.SaveChangesAsync();

                // Confirrm deletion
                var driverExists = await dbcontext.Drivers.FirstOrDefaultAsync(driver => driver.Id == driverId);
                if (driverExists == null)
                {
                    // Driver deleted successfuly
                    return Ok();
                }
            }

            // Empty or invalid GUID
            return StatusCode(422);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new {Message = "Exception occurred: " + ex.Message});
        }
    }

    // Private helper function to check if a driver exists using their name and driver number
    private async Task<bool> DriverExists(string name, int driverNumber)
    {

        try
        {
            if (!name.IsNullOrEmpty())
            {
                var drivers = await dbcontext.Drivers.FirstOrDefaultAsync(driver => driver.Name == name && driver.DriverNumber == driverNumber);
                if (drivers != null) return true;
            }

            return false;
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}
