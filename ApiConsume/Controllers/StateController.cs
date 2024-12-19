using ApiConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ApiConsume.Controllers
{
    public class StateController : Controller
    {
        #region Configiuration
        private readonly HttpClient _httpClient;
        Uri baseAddress = new Uri("http://localhost:41349/api");

        public StateController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        #endregion

        #region StateList
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
        #endregion

        //[HttpDelete("{CountryID}")]
        #region StateDelete
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
        #endregion

        #region StateAddEdit
        public async Task<IActionResult> StateAddEdit(int? StateID)
        {
            await LoadCountryList();
            if (StateID.HasValue)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/State/{StateID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var state = JsonConvert.DeserializeObject<StateModel>(data);
                    return View(state);
                }
            }
            return View(new StateModel());
        }
        #endregion

        #region StateSave
        [HttpPost]
        public async Task<IActionResult> StateSave(StateModel state)
        {
            await LoadCountryList();
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(state);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (state.StateID== null || state.StateID==0)
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/State", content);
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/State/{state.StateID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("StateList");
            }
            return View("StateAddEdit", state);
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


    }
}
