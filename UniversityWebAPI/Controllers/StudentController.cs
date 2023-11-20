using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UniversityWebAPI.Dto;
using UniversityWebAPI.Interfaces;
using UniversityWebAPI.Models;

namespace UniversityWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentController(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<StudentDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetStudents()
        {
            var students = _mapper.Map<List<StudentDto>>(_studentRepository.GetStudents());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(students);
        }

        [HttpGet("{studentId}")]
        [ProducesResponseType(200, Type = typeof(StudentWithUniversitiesDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetStudent(int studentId)
        {
            if (!_studentRepository.StudentExists(studentId))
                return NotFound();

            var student = _mapper.Map<StudentWithUniversitiesDto>(_studentRepository.GetStudent(studentId));
            student.Universities = _mapper.Map<List<UniversitiesByStudentDto>>(_studentRepository.GetUniversitiesByStudentId(studentId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(student);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreateStudent([FromBody] StudentCreateDto studentCreate)
        {
            if (studentCreate == null || studentCreate.UniversityIds.Any(u => u == 0))
                return BadRequest(ModelState);

            var student = _studentRepository.GetStudents()
                .Where(s => s.LastName.Trim().ToUpper() == studentCreate.LastName.Trim().ToUpper())
                .FirstOrDefault();

            if (student != null)
            {
                ModelState.AddModelError("", "Student already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentMap = _mapper.Map<Student>(studentCreate);

            foreach (int id in studentCreate.UniversityIds)
            {
                studentMap.StudentUniversities.Add(new StudentUniversity()
                {
                    Student = studentMap,
                    UniversityId = id,
                });
            }

            if (!_studentRepository.CreateStudent(studentMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{studentId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult UpdateStudent(int studentId, [FromBody] StudentCreateDto updatedStudent)
        {
            if (updatedStudent == null || updatedStudent.UniversityIds.Any(u => u == 0))
                return BadRequest(ModelState);

            if (!_studentRepository.StudentExists(studentId))
                return NotFound();

            var student = _studentRepository.GetStudent(studentId);
            student.FirstName = updatedStudent.FirstName;
            student.LastName = updatedStudent.LastName;
            student.Nationality = updatedStudent.Nationality;

            var existingIds = student.StudentUniversities.Select(su => su.UniversityId).ToList();
            var selectedIds = updatedStudent.UniversityIds.ToList();
            var toAddIds = selectedIds.Except(existingIds).ToList();
            var toRemoveIds = existingIds.Except(selectedIds).ToList();
            student.StudentUniversities = student.StudentUniversities.Where(su => !toRemoveIds.Contains(su.UniversityId)).ToList();
            foreach (int id in toAddIds)
            {
                student.StudentUniversities.Add(new StudentUniversity()
                {
                    UniversityId = id,
                });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_studentRepository.UpdateStudent(student))
            {
                ModelState.AddModelError("", "Something went wrong while updating student");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{studentId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteStudent(int studentId)
        {
            if (!_studentRepository.StudentExists(studentId))
                return NotFound();

            var studentToDelete = _studentRepository.GetStudent(studentId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_studentRepository.DeleteStudent(studentToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting student");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully deleted");
        }
    }
}
