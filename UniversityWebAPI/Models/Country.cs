namespace UniversityWebAPI.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Navigations Properties
        public ICollection<University> Universities { get; set; } = new List<University>();
    }
}
