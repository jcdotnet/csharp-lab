using Azure.Core;
using CitiesManager.Core.DTO;
using CitiesManager.Core.Entities;
using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.WebAPI.Controllers;

/// <summary>
///  Represents the Cities Manager API controller class
/// </summary>
[Authorize]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class CitiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    public CitiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Cities
    /// <summary>
    /// Gets a list of cities from the database
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityResponse>>> GetCities()
    {
        return await _context.Cities
        .AsNoTracking()
        .Select(c => new CityResponse(c.Id, c.Name, c.CountryId, null, null))
        .ToListAsync();
    }

    // GET: api/Cities/5
    /// <summary>
    /// Get a city from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CityResponse>> GetCity(Guid id)
    {
        var cityResponse = await _context.Cities
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CityResponse(c.Id, c.Name, c.CountryId, null, null))
            .FirstOrDefaultAsync();

        if (cityResponse == null)
        {
            //return NotFound();
            return Problem(detail: "Invalid City Id", statusCode: 400, title: "Get City");
        }

        return cityResponse;
    }

    // PUT: api/Cities/5
    /// <summary>
    /// Updates a city in the database
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cityUpdateRequest"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCity(Guid id, CityUpdateRequest cityUpdateRequest)
    {
        if (id != cityUpdateRequest.CityId)
        {
            return BadRequest();
        }

        //_context.Entry(city).State = EntityState.Modified;
        var tempCity = await _context.Cities.FindAsync(id);
        if (tempCity == null) return NotFound();
        
        tempCity.Name = cityUpdateRequest.CityName;
        tempCity.CountryId = cityUpdateRequest.CountryId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CityExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Cities
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    /// <summary>
    /// Creates a city in the database
    /// </summary>
    /// <param name="cityAddRequest"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<CityResponse>> PostCity([FromBody]CityAddRequest cityAddRequest)
    {
        var city = new City
        {
            Name = cityAddRequest.CityName.Trim(),
            CountryId = cityAddRequest.CountryId
        };

        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var response = new CityResponse(city.Id, city.Name, city.CountryId, null, null);

        return CreatedAtAction("GetCity", new { id = city.Id }, city);
    }

    // DELETE: api/Cities/5
    /// <summary>
    /// Deletes a city from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCity(Guid id)
    {
        var city = await _context.Cities.FindAsync(id);
        if (city == null)
        {
            return NotFound();
        }

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CityExists(Guid id)
    {
        return _context.Cities.Any(e => e.Id == id);
    }
}
