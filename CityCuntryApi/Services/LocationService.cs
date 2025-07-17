using Azure;
using CityCuntryApi.Data;
using CityCuntryApi.Models;
using CityCuntryApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace CityCuntryApi.Services
{
    public interface ILocationService
    {
        Task<AppResponse> FetchAndSaveLocation(string Url, CancellationToken cancellationToken = default);
    }

    public class LocationService : ILocationService
    {
        private readonly AppDbContext _appDbContext;
        private readonly HttpClient _httpClient;

        // HttpClient رو از بیرون تزریق می‌کنیم (بهتر برای تست و بهینه‌سازی)
        public LocationService(AppDbContext appDbContext, HttpClient httpClient)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<AppResponse> FetchAndSaveLocation(string url, CancellationToken cancellationToken = default)
        {
            AppResponse result = new AppResponse();
            if (string.IsNullOrEmpty(url))
            {
                result.Msg = "Url can not be empty";
                result.Error = true;
                return result;
            }
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    result.Error = true;
                    result.Msg = "Fetching data from server had error ! ";
                    return result;
                }
                var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponse = JsonSerializer.Deserialize<FullResponse>(jsonString, options);
                if (apiResponse?.data == null || apiResponse.data.Count == 0)
                {
                    result.Msg = "Data is not in correct format .... ";
                    result.Error = true;
                    return result;
                }
                // start using navigation property
                List<Country> _countryLst = new List<Country>();
                List<State> _stateLst = new List<State>();
                List<City> _cityLst = new List<City>();
                //strat foreach and save on obj.
                foreach (var _country in apiResponse.data)
                {
                    var newCountry = new Country
                    {
                        CountryName = _country.name,
                        CountryCode = _country.iso3,
                    };
                    _countryLst.Add(newCountry);
                    if (_country.states != null)
                    {
                        foreach (var _state in _country.states)
                        {
                            var newState = new State
                            {
                                StateName = _state.name,
                                Country = newCountry,
                            };
                            _stateLst.Add(newState);
                            var cityList = await FetchCities("https://countriesnow.space/api/v0.1/countries/state/cities", newCountry.CountryName, newState.StateName);
                            if (cityList.Object != null)
                            {
                                foreach(var _city in (List<string>)cityList.Object)
                                {
                                    var newCity = new City
                                    {
                                        CityName = _city,
                                        State = newState
                                    };
                                    _cityLst.Add(newCity);
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                await _appDbContext.Countries.AddRangeAsync(_countryLst, cancellationToken);
                await _appDbContext.States.AddRangeAsync(_stateLst, cancellationToken);
                await _appDbContext.Cities.AddRangeAsync(_cityLst, cancellationToken);
                await _appDbContext.SaveChangesAsync(cancellationToken);

                result.Msg = "Data has been registred on db successfully";
                result.Result = 2;

            }
            catch (OperationCanceledException)
            {
                result.Msg = "Opertaion has been cancelled ! ";
                result.Result = 0;
            }
            catch (Exception ex)
            {
                result.Msg = "Error in fetch or register data: " + ex.Message;
                result.Error = true;
                result.Result = 0;
            }
            return result;

        }
        
        public async Task<AppResponse> FetchCities(string url, string country, string state, CancellationToken cancellationToken = default)
        {
            AppResponse result = new AppResponse();

            try
            {
                var payload = new { country, state };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    result.Error = true;
                    result.Msg = $"Fetching data from server failed with status code: {response.StatusCode}";
                    return result;
                }

                var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponse = JsonSerializer.Deserialize<CitiesResponse>(jsonString, options);

                if (apiResponse == null || apiResponse.Error || apiResponse.Data == null || apiResponse.Data.Count == 0)
                {
                    result.Error = true;
                    result.Msg = "Data is not in correct format or no cities found.";
                    return result;
                }

                result.Result = 1;
                result.Msg = "Cities fetched successfully.";
                result.Object = apiResponse.Data;  

                return result;
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.Msg = $"Exception occurred: {ex.Message}";
                return result;
            }
        }

    }

}

