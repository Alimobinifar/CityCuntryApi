using CityCuntryApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace CityCuntryApi.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        [Route("GetLocationData")]
        public async Task<JsonResult> GetLocationData(string Url= "https://countriesnow.space/api/v0.1/countries/states")
        {
            var response = Json(await _locationService.FetchAndSaveLocation(Url));
            return response;
        }
    }
}
