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
        public async Task<IActionResult> GetStudents()
        {
            var students = await _studentRepository.GetStudents();
            var studentsMap = _mapper.Map<List<StudentDto>>(students);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(studentsMap);
        }

        [HttpGet("{studentId}")]
        [ProducesResponseType(200, Type = typeof(StudentWithUniversitiesDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStudent(int studentId)
        {
            var studentExists = await _studentRepository.StudentExists(studentId);
            if (!studentExists)
                return NotFound();

            var student = await _studentRepository.GetStudent(studentId);
            var universities = await _studentRepository.GetUniversitiesByStudentId(studentId);
            var studentMap = _mapper.Map<StudentWithUniversitiesDto>(student);
            studentMap.Universities = _mapper.Map<List<UniversitiesByStudentDto>>(universities);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(studentMap);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDto studentCreate)
        {
            if (studentCreate == null || studentCreate.UniversityIds.Any(u => u == 0))
                return BadRequest(ModelState);

            var students = await _studentRepository.GetStudents();
            var student = students.Where(s => s.LastName.Trim().ToUpper() == studentCreate.LastName.Trim().ToUpper()).FirstOrDefault();

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

            var addStudentMap = await _studentRepository.CreateStudent(studentMap);
            if (!addStudentMap)
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
        public async Task<IActionResult> UpdateStudent(int studentId, [FromBody] StudentCreateDto updatedStudent)
        {
            if (updatedStudent == null || updatedStudent.UniversityIds.Any(u => u == 0))
                return BadRequest(ModelState);

            var studentExists = await _studentRepository.StudentExists(studentId);
            if (!studentExists)
                return NotFound();

            var student = await _studentRepository.GetStudent(studentId);
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

            var studentUpdate = await _studentRepository.UpdateStudent(student);
            if (!studentUpdate)
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
        public async Task<IActionResult> DeleteStudent(int studentId)
        {
            var studentExists = await _studentRepository.StudentExists(studentId);
            if (!studentExists)
                return NotFound();

            var studentToDelete = await _studentRepository.GetStudent(studentId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentRemove = await _studentRepository.DeleteStudent(studentToDelete);
            if (!studentRemove)
            {
                ModelState.AddModelError("", "Something went wrong while deleting student");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully deleted");
        }
    }
}
