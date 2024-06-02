using _PerfectPickUsers_MS.DB;
using _PerfectPickUsers_MS.Models.Countries;
using Newtonsoft.Json.Linq;

namespace _PerfectPickUsers_MS.Repositories
{
    public class CountryRepository
    {
        private readonly HttpClient _httpClient;

        public CountryRepository()
        {
            _httpClient = new()
            {
                BaseAddress = new Uri(Environment.GetEnvironmentVariable("countriesURI") ?? throw new Exception("Countri URI not found"))
            };
        }
        public CountryModel GetCountry(int countryID)
        {
            using (var _context = new PerfectPickUsersDbSecondContext())
            {
                Country? country = _context.Countries.Find(countryID);
                if (country == null)
                {
                    throw new Exception("Country not found");
                }
                return new CountryModel
                {
                    Id_country = country.IdCountry,
                    Name = country.Name,
                    Code_2 = country.Code2,
                    Code_3 = country.Code3
                };
            }
        }

        public List<CountryModel> GetAllCountries()
        {
            using (var _context = new PerfectPickUsersDbSecondContext())
            {
                var countries = _context.Set<Country>().ToList();
                var countriesList = new List<CountryModel>();
                foreach (var country in countries)
                {
                    countriesList.Add(new CountryModel
                    {
                        Id_country = country.IdCountry,
                        Name = country.Name,
                        Code_2 = country.Code2,
                        Code_3 = country.Code3
                    });
                }
                return countriesList;
            }
        }

        public void AddCountry(CountryModel country)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                _context.Countries.Add(new Country
                {
                    Name = country.Name,
                    Code2 = country.Code_2,
                    Code3 = country.Code_3
                });
                _context.SaveChanges();
            }
            using (var _context = new PerfectPickUsersDbSecondContext())
            {
                _context.Countries.Add(new Country
                {
                    Name = country.Name,
                    Code2 = country.Code_2,
                    Code3 = country.Code_3
                });
                _context.SaveChanges();
            }
        }

        public void UpdateCountry(CountryModel country, int countryID)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                Country? countryToUpdate = _context.Countries.Find(countryID);
                if (countryToUpdate == null)
                {
                    throw new Exception("Country not found");
                }
                countryToUpdate.Name = country.Name;
                countryToUpdate.Code2 = country.Code_2;
                countryToUpdate.Code3 = country.Code_3;
                _context.SaveChanges();
            }
            using (var _context = new PerfectPickUsersDbSecondContext())
            {
                Country? countryToUpdate = _context.Countries.Find(countryID);
                if (countryToUpdate == null)
                {
                    throw new Exception("Country not found");
                }
                countryToUpdate.Name = country.Name;
                countryToUpdate.Code2 = country.Code_2;
                countryToUpdate.Code3 = country.Code_3;
                _context.SaveChanges();
            }
        }

        public void DeleteCountry(int countryID)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                Country? country = _context.Countries.Find(countryID);
                if (country == null)
                {
                    throw new Exception("Country not found");
                }
                _context.Countries.Remove(country);
                _context.SaveChanges();
            }
            using (var _context = new PerfectPickUsersDbSecondContext())
            {
                Country? country = _context.Countries.Find(countryID);
                if (country == null)
                {
                    throw new Exception("Country not found");
                }
                _context.Countries.Remove(country);
                _context.SaveChanges();
            }
        }

        public void UpdateDatabase()
        {
            var countries = _httpClient.GetAsync("v3.1/all?fields=name,cca2,cca3").Result;
            if (countries.IsSuccessStatusCode)
            {
                var countriesList = countries.Content.ReadAsStringAsync().Result;
                var countriesArray = JArray.Parse(countriesList);
                using (var _context = new PerfectPickUsersDbContext())
                {
                    foreach (var country in countriesArray)
                    {
                        var Name = JObject.Parse(country["name"].ToString());
                        var CountryName = Name["common"];
                        var countryToAdd = new Country
                        {
                            Name = CountryName.ToString(),
                            Code2 = country["cca2"].ToString(),
                            Code3 = country["cca3"].ToString()
                        };
                        _context.Countries.Add(countryToAdd);
                    }
                    _context.SaveChanges();
                }
                using (var _context = new PerfectPickUsersDbSecondContext())
                {
                    foreach (var country in countriesArray)
                    {
                        var Name = JObject.Parse(country["name"].ToString());
                        var CountryName = Name["common"];
                        var countryToAdd = new Country
                        {
                            Name = CountryName.ToString(),
                            Code2 = country["cca2"].ToString(),
                            Code3 = country["cca3"].ToString()
                        };
                        _context.Countries.Add(countryToAdd);
                    }
                    _context.SaveChanges();
                }
            }
        }
    }
}
