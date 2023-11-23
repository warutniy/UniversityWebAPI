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

        public async Task<bool> CreateUniversity(University university)
        {
            _context.Universities.Add(university);
            return await Save();
        }

        public async Task<bool> DeleteUniversities(List<University> universities)
        {
            _context.Universities.RemoveRange(universities);
            return await Save();
        }

        public async Task<bool> DeleteUniversity(University university)
        {
            _context.Universities.Remove(university);
            return await Save();
        }

        public async Task<ICollection<Student>> GetStudentsByUniversityId(int universityId)
        {
            return await _context.StudentUniversities.Where(su => su.UniversityId == universityId).Select(s => s.Student).ToListAsync();
        }

        public async Task<ICollection<University>> GetUniversities()
        {
            return await _context.Universities.Include(u => u.Country).ToListAsync();
        }

        public async Task<University> GetUniversity(int universityId)
        {
            return await _context.Universities.Where(u => u.Id == universityId).Include(u => u.Country).FirstOrDefaultAsync();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UniversityExists(int universityId)
        {
            return await _context.Universities.AnyAsync(u => u.Id == universityId);
        }

        public async Task<bool> UpdateUniversity(University university)
        {
            _context.Universities.Update(university);
            return await Save();
        }
    }
}
