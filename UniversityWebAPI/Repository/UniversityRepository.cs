using Microsoft.EntityFrameworkCore;
using UniversityWebAPI.Data;
using UniversityWebAPI.Interfaces;
using UniversityWebAPI.Models;

namespace UniversityWebAPI.Repository
{
    public class UniversityRepository : IUniversityRepository
    {
        private readonly DataContext _context;

        public UniversityRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateUniversity(University university)
        {
            _context.Universities.Add(university);
            return Save();
        }

        public bool DeleteUniversity(University university)
        {
            _context.Universities.Remove(university);
            return Save();
        }

        public ICollection<Student> GetStudentsByUniversityId(int universityId)
        {
            return _context.StudentUniversities.Where(su => su.UniversityId == universityId).Select(s => s.Student).ToList();
        }

        public ICollection<University> GetUniversities()
        {
            return _context.Universities.Include(u => u.Country).ToList();
        }

        public University GetUniversity(int universityId)
        {
            return _context.Universities.Where(u => u.Id == universityId).Include(u => u.Country).FirstOrDefault();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UniversityExists(int universityId)
        {
            return _context.Universities.Any(u => u.Id == universityId);
        }

        public bool UpdateUniversity(University university)
        {
            _context.Universities.Update(university);
            return Save();
        }
    }
}
