using ApiConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;


namespace ApiConsume.Controllers
{
    public class CityController : Controller
    {
        #region Configuation
        private readonly HttpClient _httpClient;
        Uri baseAddress = new Uri("http://localhost:41349/api");

        public CityController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        #endregion


        #region CityList
        public  List<CityModel> GetAllCities(int? filterID)
        {
            List<CityModel> cities = new List<CityModel>();
            int filterby = filterID ?? -999;
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/City/cities/{filterby}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                cities = JsonConvert.DeserializeObject<List<CityModel>>(data);
            }
            return cities;
        }
        #endregion

        public IActionResult CityList(int? filterID) {

            var data = GetAllCities(filterID);
            ViewBag.Data = data;
            if(filterID == null)
            {
                ViewBag.Cstae = "All";
            }
            else
            {
                if (data.Count != 0)
                    ViewBag.cstae = data[0].StateName;
                else
                    ViewBag.cstae = "No Data";
            }
            return View();
        }

        #region CityAdd
        public async Task<IActionResult> CityAddEdit(int? CityID)
        {
            await LoadCountryList();
            if (CityID.HasValue)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/City/{CityID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var city = JsonConvert.DeserializeObject<CityModel>(data);
                    ViewBag.StateList = await GetStatesByCountryID(city.CountryID);
                    return View(city);
                }
            }
            return View(new CityModel());
        }
        #endregion

        #region CitySave
        [HttpPost]
        public async Task<IActionResult> CitySave(CityModel city)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(city);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (city.CityID == null || city.CityID == 0)
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/City", content);
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/City/{city.CityID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("CityList");
            }
            await LoadCountryList();
            return View("CityAddEdit", city);
        }
        #endregion

        #region CityDelete
        public async Task<IActionResult> CityDelete(int CityID)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/City/{CityID}");
            return RedirectToAction("CityList");
        }
        #endregion

        #region LoadCountryList
        private async Task LoadCountryList()
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Country/countries");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<CountryDropDownModel>>(data);
                ViewBag.CountryList = countries;
            }
        }
        #endregion

        #region GetStatesByCountry
        [HttpPost]
        public async Task<JsonResult> GetStatesByCountry(int CountryID)
        {
            var states = await GetStatesByCountryID(CountryID);
            return Json(states);
        }
        #endregion

        #region GetStatesByCountryID
        private async Task<List<StateDropDownModel>> GetStatesByCountryID(int CountryID)
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Country/States/{CountryID}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<StateDropDownModel>>(data);
            }
            return new List<StateDropDownModel>();
        }
        #endregion
    }
}
