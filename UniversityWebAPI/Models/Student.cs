namespace UniversityWebAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }

        //Navigations Properties
        public ICollection<StudentUniversity> StudentUniversities { get; set; } = new List<StudentUniversity>();
    }
}
