using UniversityWebAPI.Models;

namespace UniversityWebAPI.Interfaces
{
    public interface IStudentRepository
    {
        ICollection<Student> GetStudents();
        Student GetStudent(int studentId);
        ICollection<University> GetUniversitiesByStudentId(int studentId);
        bool StudentExists(int studentId);
        bool CreateStudent(Student student);
        bool UpdateStudent(Student student);
        bool DeleteStudent(Student student);
        bool Save();
    }
}
