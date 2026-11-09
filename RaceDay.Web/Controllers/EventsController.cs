using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{//start of namespace
    public class EventsController : Controller
    {//start of class
        private readonly RaceDayApiService _apiService;

        // Connect the controller to the API service.
        public EventsController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        // Display all available events.
        public async Task<IActionResult> Index()
        {
            try
            {
                // Request the list of events from the Part 2 API.
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                // Send the retrieved events to the view.
                return View(events ?? new List<EventViewModel>());
            }
            catch (HttpRequestException)
            {
                // Display an empty list if the API request fails.
                ViewBag.ErrorMessage = "Unable to load events. Please try again later.";

                return View(new List<EventViewModel>());
            }
        }
    }//end of class
}//end of namespace
