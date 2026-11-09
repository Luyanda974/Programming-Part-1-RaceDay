using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{//start of nmaepsce
    public class ResultsController : Controller
    {//start of class
        private readonly RaceDayApiService _apiService;

        // Connect the controller to the API service.
        public ResultsController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        // Display results belonging to the logged-in participant.
        public async Task<IActionResult> Index()
        {
            // Retrieve the participant's login token from the session.
            string? token = HttpContext.Session.GetString("JWToken");

            // Redirect to login if the participant is not logged in.
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Retrieve the participant's own enrolments.
                var enrollments =
                    await _apiService.GetWithTokenAsync<List<EnrollmentViewModel>>(
                        "api/Enrollments", token);

                enrollments ??= new List<EnrollmentViewModel>();

                // Retrieve all recorded results from the API.
                var allResults =
                    await _apiService.GetAsync<List<ResultViewModel>>(
                        "api/Results");

                allResults ??= new List<ResultViewModel>();

                // Create a list to hold only this participant's results.
                var myResults = new List<ResultViewModel>();

                // Check each result against the participant's enrolments.
                foreach (var result in allResults)
                {
                    var enrollment = enrollments.FirstOrDefault(
                        item => item.EnrollmentID == result.EnrollmentID);

                    // Only include results linked to this participant.
                    if (enrollment != null)
                    {
                        // Retrieve the event details.
                        var selectedEvent =
                            await _apiService.GetAsync<EventViewModel>(
                                $"api/Events/{enrollment.EventID}");

                        // Retrieve the category details.
                        var selectedCategory =
                            await _apiService.GetAsync<CategoryViewModel>(
                                $"api/Categories/{enrollment.CategoryID}");

                        // Add the event and category names for display.
                        result.EventName =
                            selectedEvent?.EventName ?? "Unknown Event";

                        result.CategoryName =
                            selectedCategory?.CategoryName ?? "Unknown Category";

                        myResults.Add(result);
                    }
                }

                return View(myResults);
            }
            catch (HttpRequestException)
            {
                // Display a friendly message if the API cannot be reached.
                ViewBag.ErrorMessage =
                    "Unable to retrieve your results. Please try again.";

                return View(new List<ResultViewModel>());
            }
        }
    }//end of class
}//end of namepsace
