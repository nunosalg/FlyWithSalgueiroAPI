using FlyWithSalgueiroAPI.Data.Entities;
using FlyWithSalgueiroAPI.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FlyWithSalgueiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;

        public CitiesController(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCities()
        {
            try
            {
                var cities = (IEnumerable<City>)_cityRepository.GetAll();
                if (cities == null)
                {
                    return NotFound("No cities found at the moment.");
                }

                return Ok(cities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
