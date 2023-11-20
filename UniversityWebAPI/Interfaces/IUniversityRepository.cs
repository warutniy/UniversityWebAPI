using UniversityWebAPI.Models;

namespace UniversityWebAPI.Interfaces
{
    public interface IUniversityRepository
    {
        ICollection<University> GetUniversities();
        University GetUniversity(int universityId);
        ICollection<Student> GetStudentsByUniversityId(int universityId);
        bool UniversityExists(int universityId);
        bool CreateUniversity(University university);
        bool UpdateUniversity(University university);
        bool DeleteUniversity(University university);
        bool Save();
    }
}
