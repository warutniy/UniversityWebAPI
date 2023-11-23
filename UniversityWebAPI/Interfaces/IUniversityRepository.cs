using UniversityWebAPI.Models;

namespace UniversityWebAPI.Interfaces
{
    public interface IUniversityRepository
    {
        Task<ICollection<University>> GetUniversities();
        Task<University> GetUniversity(int universityId);
        Task<ICollection<Student>> GetStudentsByUniversityId(int universityId);
        Task<bool> UniversityExists(int universityId);
        Task<bool> CreateUniversity(University university);
        Task<bool> UpdateUniversity(University university);
        Task<bool> DeleteUniversity(University university);
        Task<bool> DeleteUniversities(List<University> universities);
        Task<bool> Save();
    }
}
