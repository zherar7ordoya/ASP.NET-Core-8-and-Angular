using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;
using WorldCities.Server.Data;
using WorldCities.Server.Data.Models;

namespace WorldCities.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SeedController
(
    ApplicationDbContext context,
    IWebHostEnvironment env
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Import()
    {
        // prevents non-development environments from running this method 
        if (!env.IsDevelopment()) throw new SecurityException("Not allowed");

        // ensures EPPlus can read the Excel file
        var path = Path.Combine(env.ContentRootPath, "Data/Source/worldcities.xlsx");
        using var stream = System.IO.File.OpenRead(path);
        using var excelPackage = new ExcelPackage(stream);

        // get the first worksheet 
        var worksheet = excelPackage.Workbook.Worksheets[0];

        // define how many rows we want to process 
        var nEndRow = worksheet.Dimension.End.Row;

        // initialize the record counters 
        var numberOfCountriesAdded = 0;
        var numberOfCitiesAdded = 0;

        // create a lookup dictionary containing all the countries already existing 
        // into the Database (it will be empty on first run). 
        var countriesByName = context.Countries
                .AsNoTracking()
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        // iterates through all rows, skipping the first one 
        for (int nRow = 2; nRow <= nEndRow; nRow++)
        {
            var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
            var countryName = row[nRow, 5].GetValue<string>();
            var iso2 = row[nRow, 6].GetValue<string>();
            var iso3 = row[nRow, 7].GetValue<string>();

            // skip this country if it already exists in the database 
            if (countriesByName.ContainsKey(countryName)) continue;

            // create the Country entity and fill it with xlsx data 
            var country = new Country
            {
                Name = countryName,
                ISO2 = iso2,
                ISO3 = iso3
            };

            // add the new country to the DB context 
            await context.Countries.AddAsync(country);

            // store the country in our lookup to retrieve its Id later on 
            countriesByName.Add(countryName, country);

            // increment the counter 
            numberOfCountriesAdded++;
        }

        // save all the countries into the Database 
        if (numberOfCountriesAdded > 0) await context.SaveChangesAsync();

        // create a lookup dictionary containing all the cities already existing 
        // into the Database (it will be empty on first run). 
        var cities = context.Cities
                .AsNoTracking()
                .ToDictionary(x => (
                    Name: x.Name,
                    Lat: x.Lat,
                    Lon: x.Lon,
                    CountryId: x.CountryId));

        // iterates through all rows, skipping the first one 
        for (int nRow = 2; nRow <= nEndRow; nRow++)
        {
            var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
            var name = row[nRow, 1].GetValue<string>();
            var lat = row[nRow, 3].GetValue<decimal>();
            var lon = row[nRow, 4].GetValue<decimal>();
            var countryName = row[nRow, 5].GetValue<string>();

            // retrieve country Id by countryName
            var countryId = countriesByName[countryName].Id;

            // skip this city if it already exists in the database 
            if (cities.ContainsKey((
                Name: name,
                Lat: lat,
                Lon: lon,
                CountryId: countryId))) continue;

            // create the City entity and fill it with xlsx data 
            var city = new City
            {
                Name = name,
                Lat = lat,
                Lon = lon,
                CountryId = countryId
            };

            // add the new city to the DB context 
            context.Cities.Add(city);

            // increment the counter 
            numberOfCitiesAdded++;
        }

        // save all the cities into the Database 
        if (numberOfCitiesAdded > 0) await context.SaveChangesAsync();
        return new JsonResult(new
        {
            Cities = numberOfCitiesAdded,
            Countries = numberOfCountriesAdded
        });
    }
}