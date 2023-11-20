namespace UniversityWebAPI.Dto
{
    public class UniversityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CountryName { get; set; }
    }

    public class UniversitiesByStudentDto
    {
        public string Name { get; set; }
        public string CountryName { get; set; }
    }

    public class UniversitiesByCountryDto
    {
        public string Name { get; set; }
    }

    public class UniversityWithStudentsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CountryName { get; set; }
        public ICollection<StudentsByUniversityDto> Students { get; set; }
    }

    public class UniversityCreateDto
    {
        public string Name { get; set; }
        public int CountryId { get; set; }
    }
}
