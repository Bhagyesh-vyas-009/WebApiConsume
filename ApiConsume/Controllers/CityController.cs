using ApiConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace ApiConsume.Controllers
{
    public class CityController : Controller
    {
        private readonly HttpClient _httpClient;
        Uri baseAddress = new Uri("http://localhost:41349/api");

        public CityController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CityList()
        {
            List<CityModel> cities = new List<CityModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/City").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                var extractedDataJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

                cities = JsonConvert.DeserializeObject<List<CityModel>>(extractedDataJson);
            }
            return View("CityList", cities);
        }

        //[HttpDelete("{CountryID}")]
        public async Task<IActionResult> CityDelete(int CityID)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/City/{CityID}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "City deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to delete the City. Status Code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("CityList");
        }
    }
}
