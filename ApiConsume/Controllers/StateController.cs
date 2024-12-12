using ApiConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiConsume.Controllers
{
    public class StateController : Controller
    {
        private readonly HttpClient _httpClient;
        Uri baseAddress = new Uri("http://localhost:41349/api");

        public StateController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult StateList()
        {
            List<StateModel> states = new List<StateModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/State").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                var extractedDataJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

                states = JsonConvert.DeserializeObject<List<StateModel>>(extractedDataJson);
            }
            return View("StateList", states);
        }

        //[HttpDelete("{CountryID}")]
        public async Task<IActionResult> StateDelete(int StateID)
        {
            try
            {

                HttpResponseMessage response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/State/{StateID}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "State deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to delete the State. Status Code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("StateList");
        }
    }
}
