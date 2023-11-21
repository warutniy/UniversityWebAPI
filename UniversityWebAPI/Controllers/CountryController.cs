using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UniversityWebAPI.Dto;
using UniversityWebAPI.Interfaces;
using UniversityWebAPI.Models;

namespace UniversityWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IUniversityRepository _universityRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, IUniversityRepository universityRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _universityRepository = universityRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(CountryWithUniversitiesDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var country = _mapper.Map<CountryWithUniversitiesDto>(_countryRepository.GetCountry(countryId));
            country.Universities = _mapper.Map<List<UniversitiesByCountryDto>>(_countryRepository.GetUniversitiesByCountryId(countryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryCreateDto countryCreate)
        {
            if (countryCreate == null)
                return BadRequest(ModelState);

            var countries = _countryRepository.GetCountries()
                .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.Trim().ToUpper())
                .FirstOrDefault();

            if (countries != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(countryCreate);

            if (!_countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryCreateDto updatedCountry)
        {
            if (updatedCountry == null)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var country = _countryRepository.GetCountry(countryId);
            country.Name = updatedCountry.Name;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.UpdateCountry(country))
            {
                ModelState.AddModelError("", "Something went wrong while updating country");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var universitiesToDelete = _countryRepository.GetUniversitiesByCountryId(countryId);
            var countryToDelete = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_universityRepository.DeleteUniversities(universitiesToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong while deleting universities");
                return StatusCode(500, ModelState);
            }

            if (!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting country");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully deleted");
        }
    }
}
