using ApiConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiConsume.Controllers
{
    public class CountryController : Controller
    {
        private readonly HttpClient _httpClient;
        Uri baseAddress = new Uri("http://localhost:41349/api");

        public CountryController()
        {
            _httpClient=new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        public IActionResult Index()
        {
            return View();
        }

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

        //[HttpDelete("{CountryID}")]
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
    }
}
