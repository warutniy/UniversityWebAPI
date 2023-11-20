using Microsoft.EntityFrameworkCore;
using UniversityWebAPI.Data;
using UniversityWebAPI.Interfaces;
using UniversityWebAPI.Models;

namespace UniversityWebAPI.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DataContext _context;

        public StudentRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateStudent(Student student)
        {
            _context.Students.Add(student);
            return Save();
        }

        public bool DeleteStudent(Student student)
        {
            _context.Students.Remove(student);
            return Save();
        }

        public Student GetStudent(int studentId)
        {
            return _context.Students.Where(s => s.Id == studentId).Include(su => su.StudentUniversities)
                .ThenInclude(u => u.University).FirstOrDefault();
        }

        public ICollection<Student> GetStudents()
        {
            return _context.Students.ToList();
        }

        public ICollection<University> GetUniversitiesByStudentId(int studentId)
        {
            return _context.StudentUniversities.Where(su => su.StudentId == studentId).Select(su => new University
            {
                Id = su.University.Id,
                Name = su.University.Name,
                Country = su.University.Country,
            }).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool StudentExists(int studentId)
        {
            return _context.Students.Any(s => s.Id == studentId);
        }

        public bool UpdateStudent(Student student)
        {
            _context.Students.Update(student);
            return Save();
        }
    }
}
