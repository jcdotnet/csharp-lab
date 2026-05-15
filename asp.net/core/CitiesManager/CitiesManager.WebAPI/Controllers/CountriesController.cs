using CitiesManager.Core.DTO;
using CitiesManager.Core.Entities;
using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CitiesManager.WebAPI.Controllers
{
    /// <summary>
    /// Represents the Cities Manager API controller class for countries
    /// </summary>
    /// <param name="context"></param>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class CountriesController(ApplicationDbContext context) : ControllerBase
    {
        //  GET: api/countries
        /// <summary>
        /// Get Countries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryResponse>>> GetCountries()
        {
            return await context.Countries
                .AsNoTracking()
                .Include(c => c.Cities)
                .Select(c => new CountryResponse(
                    c.Id,
                    c.Name,
                    c.Cities.Select(city => city.Name).ToList()
                ))
                .ToListAsync(); // not OK(..) // Implicit HTTP 200 OK conversion 
        }

        // GET: api/countries/1
        /// <summary>
        /// Get country
        /// </summary>
        /// <param name="id">Country Id</param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CountryResponse>> GetCountry(Guid id)
        {
            var country = await context.Countries
                .AsNoTracking()
                .Include(c => c.Cities)
                .Where(c => c.Id == id)
                .Select(c => new CountryResponse(
                    c.Id,
                    c.Name,
                    c.Cities.Select(city => city.Name).ToList()
                ))
                .FirstOrDefaultAsync();

            if (country == null) return Problem(detail: "Country not found.", statusCode: 404, title: "Get Country");

            return country; // Ok(country) // Implicit HTTP 200 OK conversion 
        }

        //  POST: api/countries
        /// <summary>
        /// Creates a new country
        /// </summary>
        /// <param name="countryAddRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CountryResponse>> PostCountry([FromBody] CountryAddRequest countryAddRequest)
        {
            string countryName = countryAddRequest.CountryName.Trim();

            bool countryExists = await context.Countries.AnyAsync(c => c.Name == countryName);

            if (countryExists)
            {
                return Problem(detail: "Country Exists", statusCode: 409, title: "Register Country");
            }

            var country = new Country
            {
                Id = Guid.NewGuid(),
                Name = countryName
            };

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            var response = new CountryResponse(country.Id, country.Name, []);

            return CreatedAtAction("GetCountry", new { id = country.Id }, response);
        }

        //  REST routes: GET cities goes here because cities are dependent on countries
        //  GET: api/countries/1/cities
        /// <summary>
        /// Get all cities from a specific country 
        /// </summary>
        /// <param name="id"> The country Id</param>
        /// <returns></returns>
        [HttpGet("{id:guid}/cities")]
        public async Task<ActionResult<IEnumerable<CityResponse>>> GetCitiesByCountry(Guid id)
        {
            bool countryExists = await context.Countries.AnyAsync(c => c.Id == id);

            if (!countryExists) return Problem(detail: "Country Not Found", statusCode: 404, title: "Get Cities");

            return await context.Cities
                .AsNoTracking()
                .Where(c => c.CountryId == id)
                .Select(c => new CityResponse(c.Id, c.Name, c.CountryId, null, null))
                .ToListAsync();
        }

    }
}
