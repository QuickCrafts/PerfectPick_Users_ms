using _PerfectPickUsers_MS.Models.Countries;
using _PerfectPickUsers_MS.Services;
using Microsoft.AspNetCore.Mvc;


namespace _PerfectPickUsers_MS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryController : Controller
    {
        private readonly CountryService _countryService;


        public CountryController()
        {
            try
            {
                _countryService = new CountryService();
            }
            catch (Exception e)
            {
                throw new Exception("CountryService not found", e);
            }
        }

        [HttpGet]
        public IActionResult GetAllCountries()
        {
            try
            {
                return Ok(_countryService.GetAllCountries());
            }
            catch (Exception e)
            {
                return StatusCode(500,new { Message = e.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetCountryById(int id)
        {
            try
            {
                return Ok(_countryService.GetCountry(id));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult AddCountry([FromBody] CountryModel country)
        {
            try
            {
                _countryService.AddCountry(country);
                return Ok(new { Message = "Country added" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCountry(int id)
        {
            try
            {
                _countryService.DeleteCountry(id);
                return Ok(new
                {
                    Message = "Country deleted"
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    Message = e.Message
                });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCountry([FromBody] CountryModel country, int id)
        {
            try
            {
                _countryService.UpdateCountry(country, id);
                return Ok( new { Message = "Country updated" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpPut]
        public IActionResult UpdateDatabase()
        {
            try
            {
                _countryService.UpdateDatabase();
                return Ok(new { Message = "Database updated" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

    }

}

