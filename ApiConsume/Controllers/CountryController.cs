using ApiConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ApiConsume.Controllers
{
    public class CountryController : Controller
    {
        #region Configuration
        private readonly HttpClient _httpClient;
        Uri baseAddress = new Uri("http://localhost:41349/api");

        public CountryController()
        {
            _httpClient=new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        #endregion

        #region CountryList
        [HttpGet]
        public IActionResult CountryList()
        {
            List<CountryModel> countries = new List<CountryModel>();
            HttpResponseMessage response=_httpClient.GetAsync($"{_httpClient.BaseAddress}/Country").Result;
            if (response.IsSuccessStatusCode) { 
                string data=response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject=JsonConvert.DeserializeObject<dynamic>(data);
                var extractedDataJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

                countries=JsonConvert.DeserializeObject<List<CountryModel>>(extractedDataJson);
            }
            return View("CountryList",countries);
        }
        #endregion

        #region CountryDelete
        public async Task<IActionResult> CountryDelete(int CountryID) {
            try
            {

                HttpResponseMessage response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Country/{CountryID}");
            //HttpResponseMessage response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Country/{CountryID}");

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Country deleted successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Failed to delete the Country. Status Code: {response.StatusCode}";
                    }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("CountryList");
        }
        #endregion

        #region CountryAddEdit
        public async Task<IActionResult> CountryAddEdit(int? CountryID)
        {
            if (CountryID.HasValue)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Country/{CountryID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var country = JsonConvert.DeserializeObject<CountryModel>(data);
                    return View(country);
                }
            }
            return View(new CountryModel());
        }
        #endregion

        #region CountrySave
        [HttpPost]
        public async Task<IActionResult> CountrySave(CountryModel country)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(country);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (country.CountryID == null || country.CountryID==0)
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Country", content);
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Country/{country.CountryID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("CountryList");
            }
            return View("CountryAddEdit", country);
        }
        #endregion
    }
}
