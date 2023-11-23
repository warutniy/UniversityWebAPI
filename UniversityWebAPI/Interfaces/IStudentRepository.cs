using UniversityWebAPI.Models;

namespace UniversityWebAPI.Interfaces
{
    public interface IStudentRepository
    {
        Task<ICollection<Student>> GetStudents();
        Task<Student> GetStudent(int studentId);
        Task<ICollection<University>> GetUniversitiesByStudentId(int studentId);
        Task<bool> StudentExists(int studentId);
        Task<bool> CreateStudent(Student student);
        Task<bool> UpdateStudent(Student student);
        Task<bool> DeleteStudent(Student student);
        Task<bool> Save();
    }
}
