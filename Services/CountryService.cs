using _PerfectPickUsers_MS.Models.Countries;
using _PerfectPickUsers_MS.Repositories;

namespace _PerfectPickUsers_MS.Services
{
    public class CountryService
    {
        private readonly CountryRepository _countryRepository;

        public CountryService()
        {
            _countryRepository = new CountryRepository();
        }

        public CountryModel GetCountry(int countryID)
        {
            return _countryRepository.GetCountry(countryID);
        }

        public List<CountryModel> GetAllCountries()
        {
            return _countryRepository.GetAllCountries();
        }

        public void AddCountry(CountryModel country)
        {
            _countryRepository.AddCountry(country);
        }

        public void UpdateCountry(CountryModel country, int countryID)
        {
            _countryRepository.UpdateCountry(country, countryID);
        }

        public void DeleteCountry(int countryID)
        {
            _countryRepository.DeleteCountry(countryID);
        }

        public void UpdateDatabase()
        {
            _countryRepository.UpdateDatabase();
        }
    }
}
