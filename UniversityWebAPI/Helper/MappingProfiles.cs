using AutoMapper;
using UniversityWebAPI.Dto;
using UniversityWebAPI.Models;

namespace UniversityWebAPI.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Country, CountryWithUniversitiesDto>().ReverseMap();
            CreateMap<Country, CountryCreateDto>().ReverseMap();
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Student, StudentsByUniversityDto>().ReverseMap();
            CreateMap<Student, StudentWithUniversitiesDto>().ReverseMap();
            CreateMap<Student, StudentCreateDto>().ReverseMap();
            CreateMap<University, UniversityDto>().ReverseMap();
            CreateMap<University, UniversitiesByStudentDto>().ReverseMap();
            CreateMap<University, UniversitiesByCountryDto>().ReverseMap();
            CreateMap<University, UniversityWithStudentsDto>().ReverseMap();
            CreateMap<University, UniversityCreateDto>().ReverseMap();
        }
    }
}
