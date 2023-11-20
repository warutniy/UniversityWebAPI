namespace UniversityWebAPI.Models
{
    public class StudentUniversity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int UniversityId { get; set; }
        public University University { get; set; }
    }
}
