namespace UniversityWebAPI.Dto
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CountryWithUniversitiesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UniversitiesByCountryDto> Universities { get; set; }
    }

    public class CountryCreateDto
    {
        public string Name { get; set; }
    }
}
