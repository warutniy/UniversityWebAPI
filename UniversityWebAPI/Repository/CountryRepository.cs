using Microsoft.EntityFrameworkCore;
using UniversityWebAPI.Data;
using UniversityWebAPI.Interfaces;
using UniversityWebAPI.Models;

namespace UniversityWebAPI.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;

        public CountryRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> CountryExists(int countryId)
        {
            return await _context.Countries.AnyAsync(c => c.Id == countryId);
        }

        public async Task<bool> CreateCountry(Country country)
        {
            _context.Countries.Add(country);
            return await Save();
        }

        public async Task<bool> DeleteCountry(Country country)
        {
            _context.Countries.Remove(country);
            return await Save();
        }

        public async Task<ICollection<Country>> GetCountries()
        {
            return await _context.Countries.ToListAsync();
        }

        public async Task<Country> GetCountry(int countryId)
        {
            return await _context.Countries.Where(c => c.Id == countryId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<University>> GetUniversitiesByCountryId(int countryId)
        {
            return await _context.Universities.Where(u => u.CountryId == countryId).ToListAsync();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateCountry(Country country)
        {
            _context.Countries.Update(country);
            return await Save();
        }
    }
}
