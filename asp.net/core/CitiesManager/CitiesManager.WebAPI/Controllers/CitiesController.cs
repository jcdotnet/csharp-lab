using CitiesManager.Core.DTO;
using CitiesManager.Core.Entities;
using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CitiesManager.WebAPI.Controllers;

/// <summary>
///  Represents the Cities Manager API controller class for cities
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
            if (!await CityExists(id))
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
    public async Task<ActionResult<CityResponse>> PostCity([FromBody] CityAddRequest cityAddRequest)
    {
        string cityName = cityAddRequest.CityName.Trim();

        bool cityExists = await _context.Cities.AnyAsync(c =>
            c.Name == cityName && c.CountryId == cityAddRequest.CountryId
        );

        if (cityExists) return Problem(detail: "City Exists", statusCode: 409, title: "Post City");

        var city = new City
        {
            Name = cityName,
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

    #region Citizens

    // REST routes: citizens routes go here because citizen are dependent on city
    // GET: api/Cities/5/Citizens
    /// <summary>
    /// Gets a list of citizens who lives in a specific city
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}/Citizens")]
    public async Task<ActionResult<IEnumerable<CitizenResponse>>> GetCitizens(Guid id)
    {
        if (!await CityExists(id))
        {
            return Problem(detail: "City Not Found", statusCode: 404, title: "Get City Citizens");
        }

        var citizens = await _context.Citizens
            .AsNoTracking()
            .Where(c => c.CityId == id)
            .Select(c => new CitizenResponse(
                c.Id,
                c.FullName,
                c.DateOfBirth,
                c.Address,
                c.CityId,
                c.City != null ? c.City.Name : null
            ))
            .ToListAsync();

        return Ok(citizens);
    }

    // GET: api/Cities/5/Citizens/123
    /// <summary>
    /// Gets a specific citizen from a specific city
    /// </summary>
    /// <param name="id"></param>
    /// <param name="citizenId"></param>
    /// <returns></returns>
    [HttpGet("{id}/Citizens/{citizenId}")]
    public async Task<ActionResult<CitizenResponse>> GetCitizen(Guid id, Guid citizenId)
    {
        var citizenResponse = await _context.Citizens
            .Where(c => c.CityId == id && c.Id == citizenId)
            .Select(c => new CitizenResponse(
                c.Id,
                c.FullName,
                c.DateOfBirth,
                c.Address,
                c.CityId,
                null
            ))
            .FirstOrDefaultAsync();

        if (citizenResponse == null)
        {
            return Problem(detail: "Citizen or City not found", statusCode: 404, title: "Get Citizen");
        }

        return citizenResponse;
    }

    // POST: api/Cities/5/Citizens
    /// <summary>
    /// Creates a new citizen inside a specific city
    /// </summary>
    /// <param name="id"></param>
    /// <param name="citizenAddRequest"></param>
    /// <returns></returns>
    [HttpPost("{id}/Citizens")]
    public async Task<ActionResult<CitizenResponse>> PostCitizen(Guid id,
        [FromBody] CitizenAddRequest citizenAddRequest)
    {

        if (!await CityExists(id))
        {
            return Problem(detail: "City Not Found", statusCode: 404, title: "Register Citizen");
        }

        string citizenName = citizenAddRequest.FullName.Trim();

        bool citizenExists = await _context.Citizens.AnyAsync(c =>
            c.FullName == citizenName && c.CityId == id
        );

        if (citizenExists) return Problem(detail: "Citizen Exists", statusCode: 409, title: "Register Citizen");

        var citizen = new Citizen
        {
            FullName = citizenName,
            DateOfBirth = citizenAddRequest.DateOfBirth,
            Address = citizenAddRequest.Address.Trim(),
            CityId = id
        };

        _context.Citizens.Add(citizen);
        await _context.SaveChangesAsync();

        var response = new CitizenResponse(
            citizen.Id,
            citizen.FullName,
            citizen.DateOfBirth,
            citizen.Address,
            citizen.CityId,
            null
        );

        return CreatedAtAction(nameof(GetCitizen), new { id = id, citizenId = response.CitizenId }, response);
    }
    // PUT: api/Cities/5/Citizens/123
    /// <summary>
    /// Updates a citizen
    /// </summary>
    /// <param name="id"></param>
    /// <param name="citizenId"></param>
    /// <param name="citizenUpdateRequest"></param>
    /// <returns></returns>
    [HttpPut("{id}/Citizens/{citizenId}")]
    public async Task<IActionResult> PutCitizen(Guid id, Guid citizenId,
        [FromBody] CitizenUpdateRequest citizenUpdateRequest)
    {
        if (citizenId != citizenUpdateRequest.CitizenId)
        {
            return BadRequest();
        }

        var tempCitizen = await _context.Citizens.FindAsync(citizenId);
        if (tempCitizen == null)
        {
            return Problem(detail: "Citizen Not Found", statusCode: 404, title: "Update Citizen");
        }

        if (tempCitizen.CityId != citizenUpdateRequest.CityId && !await CityExists(citizenUpdateRequest.CityId))
        {
            return Problem(detail: "City Not Found", statusCode: 404, title: "Update Citizen");
        }

        tempCitizen.FullName = citizenUpdateRequest.FullName.Trim();
        tempCitizen.DateOfBirth = citizenUpdateRequest.DateOfBirth;
        tempCitizen.Address = citizenUpdateRequest.Address.Trim();
        tempCitizen.CityId = citizenUpdateRequest.CityId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Citizens.Any(c => c.Id == citizenId))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Cities/5/Citizens/123
    /// <summary>
    /// Deletes a citizen from a specific city
    /// </summary>
    /// <param name="id"></param>
    /// <param name="citizenId"></param>
    /// <returns></returns>
    [HttpDelete("{id}/Citizens/{citizenId}")]
    public async Task<IActionResult> DeleteCitizen(Guid id, Guid citizenId)
    {
        var citizen = await _context.Citizens
            .FirstOrDefaultAsync(c => c.Id == citizenId && c.CityId == id);

        if (citizen == null)
        {
            return Problem(detail: "Citizen Not Found", statusCode: 404, title: "Delete Citizen");
        }

        _context.Citizens.Remove(citizen);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region private methods

    private async Task<bool> CityExists(Guid id)
    {
        return await _context.Cities.AnyAsync(e => e.Id == id);
    }

    #endregion
}
