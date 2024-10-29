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
        public IActionResult GetCities()
        {
            try
            {
                var cities = _cityRepository.GetAll();
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
