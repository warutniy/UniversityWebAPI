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
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _countryRepository.GetCountries();
            var countriesMap = _mapper.Map<List<CountryDto>>(countries);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(countriesMap);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(CountryWithUniversitiesDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCountry(int countryId)
        {
            var countryExists = await _countryRepository.CountryExists(countryId);
            if (!countryExists)
                return NotFound();

            var country = await _countryRepository.GetCountry(countryId);
            var universities = await _countryRepository.GetUniversitiesByCountryId(countryId);
            var countryMap = _mapper.Map<CountryWithUniversitiesDto>(country);
            countryMap.Universities = _mapper.Map<List<UniversitiesByCountryDto>>(universities);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(countryMap);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCountry([FromBody] CountryCreateDto countryCreate)
        {
            if (countryCreate == null)
                return BadRequest(ModelState);

            var countries = await _countryRepository.GetCountries();
            var country = countries.Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(countryCreate);

            var addCountryMap = await _countryRepository.CreateCountry(countryMap);
            if (!addCountryMap)
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
        public async Task<IActionResult> UpdateCountry(int countryId, [FromBody] CountryCreateDto updatedCountry)
        {
            if (updatedCountry == null)
                return BadRequest(ModelState);

            var countryExists = await _countryRepository.CountryExists(countryId);
            if (!countryExists)
                return NotFound();

            var country = await _countryRepository.GetCountry(countryId);
            country.Name = updatedCountry.Name;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryUpdate = await _countryRepository.UpdateCountry(country);
            if (!countryUpdate)
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
        public async Task<IActionResult> DeleteCountry(int countryId)
        {
            var countryExists = await _countryRepository.CountryExists(countryId);
            if (!countryExists)
                return NotFound();

            var universitiesToDelete = await _countryRepository.GetUniversitiesByCountryId(countryId);
            var countryToDelete = await _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var universitiesRemove = await _universityRepository.DeleteUniversities(universitiesToDelete.ToList());
            if (!universitiesRemove)
            {
                ModelState.AddModelError("", "Something went wrong while deleting universities");
                return StatusCode(500, ModelState);
            }

            var countryRemove = await _countryRepository.DeleteCountry(countryToDelete);
            if (!countryRemove)
            {
                ModelState.AddModelError("", "Something went wrong while deleting country");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully deleted");
        }
    }
}
