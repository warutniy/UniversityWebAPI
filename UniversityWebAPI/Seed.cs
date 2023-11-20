using UniversityWebAPI.Data;
using UniversityWebAPI.Models;

namespace UniversityWebAPI
{
    public class Seed
    {
        private readonly DataContext _context;

        public Seed(DataContext context)
        {
            _context = context;
        }

        public void SeedDataContext()
        {
            if (!_context.Students.Any())
            {
                var students = new List<Student>()
                {
                    new Student()
                    {
                        FirstName = "John",
                        LastName = "Smith",
                        Nationality = "English",
                        StudentUniversities = new List<StudentUniversity>()
                        {
                            new StudentUniversity()
                            {
                                University = new University()
                                {
                                    Name = "University of Cambridge",
                                    Country = new Country()
                                    {
                                        Name = "England"
                                    }
                                }
                            },
                            new StudentUniversity()
                            {
                                University = new University()
                                {
                                    Name = "Stanford University",
                                    Country = new Country()
                                    {
                                        Name = "United States"
                                    }
                                }
                            }
                        }
                    },
                    new Student()
                    {
                        FirstName = "Mark",
                        LastName = "Twain",
                        Nationality = "French",
                        StudentUniversities = new List<StudentUniversity>()
                        {
                            new StudentUniversity()
                            {
                                University = new University()
                                {
                                    Name = "University of Paris",
                                    Country = new Country()
                                    {
                                        Name = "France"
                                    }
                                }
                            },
                            new StudentUniversity()
                            {
                                University = new University()
                                {
                                    Name = "University of Milan",
                                    Country = new Country()
                                    {
                                        Name = "Italy"
                                    }
                                }
                            }
                        }
                    },
                    new Student()
                    {
                        FirstName = "Sarah",
                        LastName = "Lee",
                        Nationality = "Korean",
                        StudentUniversities = new List<StudentUniversity>()
                        {
                            new StudentUniversity()
                            {
                                University = new University()
                                {
                                    Name = "Korea University",
                                    Country = new Country()
                                    {
                                        Name = "South Korea"
                                    }
                                }
                            }
                        }
                    }
                };
                _context.Students.AddRange(students);
                _context.SaveChanges();
            }
        }
    }
}
