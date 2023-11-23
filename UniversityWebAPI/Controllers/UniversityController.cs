using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UniversityWebAPI.Dto;
using UniversityWebAPI.Interfaces;
using UniversityWebAPI.Models;

namespace UniversityWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityController : Controller
    {
        private readonly IUniversityRepository _universityRepository;
        private readonly IMapper _mapper;

        public UniversityController(IUniversityRepository universityRepository, IMapper mapper)
        {
            _universityRepository = universityRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UniversityDto>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetUniversities()
        {
            var universities = await _universityRepository.GetUniversities();
            var universitiesMap = _mapper.Map<List<UniversityDto>>(universities);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(universitiesMap);
        }
        
        [HttpGet("{universityId}")]
        [ProducesResponseType(200, Type = typeof(UniversityWithStudentsDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUniversity(int universityId)
        {
            var universityExists = await _universityRepository.UniversityExists(universityId);
            if (!universityExists)
                return NotFound();

            var university = await _universityRepository.GetUniversity(universityId);
            var students = await _universityRepository.GetStudentsByUniversityId(universityId);
            var universityMap = _mapper.Map<UniversityWithStudentsDto>(university);
            universityMap.Students = _mapper.Map<List<StudentsByUniversityDto>>(students);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(universityMap);
        }
        
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUniversity([FromBody] UniversityCreateDto universityCreate)
        {
            if (universityCreate == null || universityCreate.CountryId <= 0)
                return BadRequest(ModelState);

            var universities = await _universityRepository.GetUniversities();
            var university = universities.Where(u => u.Name.Trim().ToUpper() == universityCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if (university != null)
            {
                ModelState.AddModelError("", "University already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var universityMap = _mapper.Map<University>(universityCreate);

            var addUniversityMap = await _universityRepository.CreateUniversity(universityMap);
            if (!addUniversityMap)
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }
        
        [HttpPut("{universityId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUniversity(int universityId, [FromBody] UniversityCreateDto updatedUniversity)
        {
            if (updatedUniversity == null || updatedUniversity.CountryId <= 0)
                return BadRequest(ModelState);

            var universityExists = await _universityRepository.UniversityExists(universityId);
            if (!universityExists)
                return NotFound();

            var university = await _universityRepository.GetUniversity(universityId);
            university.Name = updatedUniversity.Name;
            university.CountryId = updatedUniversity.CountryId;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var universityUpdate = await _universityRepository.UpdateUniversity(university);
            if (!universityUpdate)
            {
                ModelState.AddModelError("", "Something went wrong while updating university");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }
        
        [HttpDelete("{universityId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUniversity(int universityId)
        {
            var universityExists = await _universityRepository.UniversityExists(universityId);
            if (!universityExists)
                return NotFound();

            var universityToDelete = await _universityRepository.GetUniversity(universityId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var universityRemove = await _universityRepository.DeleteUniversity(universityToDelete);
            if (!universityRemove)
            {
                ModelState.AddModelError("", "Something went wrong while deleting university");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully deleted");
        }
    }
}
