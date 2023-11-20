namespace UniversityWebAPI.Models
{
    public class University
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Navigations Properties
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public ICollection<StudentUniversity> StudentUniversities { get; set; } = new List<StudentUniversity>();
    }
}
