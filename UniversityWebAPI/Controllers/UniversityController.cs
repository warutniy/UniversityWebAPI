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
        public IActionResult GetUniversities()
        {
            var universities = _mapper.Map<List<UniversityDto>>(_universityRepository.GetUniversities());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(universities);
        }

        [HttpGet("{universityId}")]
        [ProducesResponseType(200, Type = typeof(UniversityWithStudentsDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUniversity(int universityId)
        {
            if (!_universityRepository.UniversityExists(universityId))
                return NotFound();

            var university = _mapper.Map<UniversityWithStudentsDto>(_universityRepository.GetUniversity(universityId));
            university.Students = _mapper.Map<List<StudentsByUniversityDto>>(_universityRepository.GetStudentsByUniversityId(universityId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(university);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateUniversity([FromBody] UniversityCreateDto universityCreate)
        {
            if (universityCreate == null || universityCreate.CountryId <= 0)
                return BadRequest(ModelState);

            var university = _universityRepository.GetUniversities()
                .Where(u => u.Name.Trim().ToUpper() == universityCreate.Name.Trim().ToUpper())
                .FirstOrDefault();

            if (university != null)
            {
                ModelState.AddModelError("", "University already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var universityMap = _mapper.Map<University>(universityCreate);

            if (!_universityRepository.CreateUniversity(universityMap))
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
        public IActionResult UpdateUniversity(int universityId, [FromBody] UniversityCreateDto updatedUniversity)
        {
            if (updatedUniversity == null || updatedUniversity.CountryId <= 0)
                return BadRequest(ModelState);

            if (!_universityRepository.UniversityExists(universityId))
                return NotFound();

            var university = _universityRepository.GetUniversity(universityId);
            university.Name = updatedUniversity.Name;
            university.CountryId = updatedUniversity.CountryId;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_universityRepository.UpdateUniversity(university))
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
        public IActionResult DeleteUniversity(int universityId)
        {
            if (!_universityRepository.UniversityExists(universityId))
                return NotFound();

            var universityToDelete = _universityRepository.GetUniversity(universityId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_universityRepository.DeleteUniversity(universityToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting university");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully deleted");
        }
    }
}
