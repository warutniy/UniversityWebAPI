using UniversityWebAPI.Models;

namespace UniversityWebAPI.Interfaces
{
    public interface ICountryRepository
    {
        Task<ICollection<Country>> GetCountries();
        Task<Country> GetCountry(int countryId);
        Task<ICollection<University>> GetUniversitiesByCountryId(int countryId);
        Task<bool> CountryExists(int countryId);
        Task<bool> CreateCountry(Country country);
        Task<bool> UpdateCountry(Country country);
        Task<bool> DeleteCountry(Country country);
        Task<bool> Save();
    }
}
