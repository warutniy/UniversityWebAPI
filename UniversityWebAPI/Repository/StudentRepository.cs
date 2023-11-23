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

        public async Task<bool> CreateStudent(Student student)
        {
            _context.Students.Add(student);
            return await Save();
        }

        public async Task<bool> DeleteStudent(Student student)
        {
            _context.Students.Remove(student);
            return await Save();
        }

        public async Task<Student> GetStudent(int studentId)
        {
            return await _context.Students.Where(s => s.Id == studentId).Include(su => su.StudentUniversities)
                .ThenInclude(u => u.University).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Student>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<ICollection<University>> GetUniversitiesByStudentId(int studentId)
        {
            return await _context.StudentUniversities.Where(su => su.StudentId == studentId).Select(su => new University
            {
                Id = su.University.Id,
                Name = su.University.Name,
                Country = su.University.Country,
            }).ToListAsync();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> StudentExists(int studentId)
        {
            return await _context.Students.AnyAsync(s => s.Id == studentId);
        }

        public async Task<bool> UpdateStudent(Student student)
        {
            _context.Students.Update(student);
            return await Save();
        }
    }
}
