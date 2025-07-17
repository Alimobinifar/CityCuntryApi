

using CityCuntryApi.Models;

namespace CityCuntryApi.ViewModel

{
    public class StateOnly
    {
        public string name { get; set; }
    }
    public class CountryWithStates
    {
        public string name { get; set; }
        public string iso3 { get; set; }
        public List<StateOnly> states { get; set; }
    }
    public class FullResponse
    {
        public bool error { get; set; }
        public string msg { get; set; }
        public List<CountryWithStates> data { get; set; }
    }

    public class CitiesResponse
    {
        public bool Error { get; set; }
        public string Msg { get; set; }
        public List<string> Data { get; set; }
    }

}
