using NUnit.Framework;
using AutodeskWebApp.Controllers;
using AutodeskWebApp.Models.Data;
using AutodeskWebApp.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AutodeskWebApp.Tests;
public class Tests
{
    private HomeController homeController;
    private DriverData driverData;
    private DbContextOptions<DriverData> dbContextOptions;
    private Guid sampleDriverId;

    [SetUp]
    public void Setup()
    {
        dbContextOptions = new DbContextOptionsBuilder<DriverData>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

        driverData = new DriverData(dbContextOptions);

        homeController = new HomeController(driverData);
        InitDatabase();
    }

    void InitDatabase()
    {
        sampleDriverId = Guid.NewGuid();
        driverData.Drivers.Add(new Driver
        {
            Id = sampleDriverId,
            Name = "Test Driver",
            Email = "Test@Email.com",
            Phone = "1234567890",
            Team = "Test Team",
            HomeCountry = "Test Country",
            IsRacingThisYear = true,
            ImageUrl = "http://example.com/image.jpg",
            DriverNumber = 1
        });

        driverData.SaveChanges();
    }

    /*** Get DriverList Tests ***/
    [Test]
    public async Task TestDriverListWithCorrectId()
    {
        var result = await homeController.DriverList(1, sampleDriverId);
        var viewResult = result as ViewResult;

        Assert.That(result, Is.InstanceOf<ViewResult>());
        Assert.That(viewResult, Is.Not.Null);
        Assert.That(viewResult.Model, Is.InstanceOf<List<Driver>>());
        Assert.That(viewResult.Model, Is.Not.InstanceOf<ErrorViewModel>());

    }

    [Test]
    public async Task TestDriverListWithIncorrectId()
    {
        // Null input ID should return all drivers
        var result = await homeController.DriverList(null, null);
        var viewResult = result as ViewResult;

        Assert.That(result, Is.InstanceOf<ViewResult>());
        Assert.That(viewResult.Model, Is.InstanceOf<List<Driver>>());

        // Invalid input IDs should return an empty drivers list
        var result2 = await homeController.DriverList(11, Guid.Empty);
        var viewResult2 = result2 as ViewResult;
        var driverList = viewResult2.Model as List<Driver>;

        Assert.That(result2, Is.InstanceOf<ViewResult>());
        Assert.That(driverList?.Count, Is.EqualTo(0));
    }

    /*** Edit Driver Tests ***/
    [Test]
    public async Task TestEditDriverWithCorrectId()
    {
        var result = await homeController.EditDriver(sampleDriverId);
        var viewResult = result as ViewResult;

        Assert.That(result, Is.InstanceOf<ViewResult>());
        Assert.That(viewResult, Is.Not.Null);
        Assert.That(viewResult.Model, Is.InstanceOf<Driver>());

    }

    [Test]
    public async Task TestEditDriverWithIncorrectId()
    {
        var result = await homeController.EditDriver(Guid.NewGuid());
        var viewResult = result as ViewResult;

        Assert.That(result, Is.InstanceOf<ViewResult>());
        Assert.That(viewResult.Model, Is.InstanceOf<ErrorViewModel>());
    }

    [TearDownAttribute]
    public void TearDown()
    {
        driverData?.Dispose();
        homeController?.Dispose();
    }

}
