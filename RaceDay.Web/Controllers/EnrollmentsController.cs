using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{//start of namespace
    public class EnrollmentsController : Controller
    {//strat of class
        private readonly RaceDayApiService _apiService;

        // Connect the controller to the API service.
        public EnrollmentsController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        // Display the participant's existing enrolments.
        public async Task<IActionResult> Index()
        {
            string? token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var enrollments =
                    await _apiService.GetWithTokenAsync<List<EnrollmentViewModel>>(
                        "api/Enrollments", token);

                return View(enrollments ?? new List<EnrollmentViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to retrieve your enrolments. Please try again.";

                return View(new List<EnrollmentViewModel>());
            }
        }

        // Display the form used to enrol in an event.
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            string? token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Retrieve available events from the API.
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var model = new EnrollmentFormViewModel
                {
                    Events = events ?? new List<EventViewModel>()
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Unable to load available events.";

                return View(new EnrollmentFormViewModel());
            }
        }

        // Retrieve categories for the event selected by the participant.
        [HttpGet]
        public async Task<IActionResult> GetCategories(int eventID)
        {
            if (eventID <= 0)
            {
                return BadRequest();
            }

            try
            {
                var categories =
                    await _apiService.GetAsync<List<CategoryViewModel>>(
                        $"api/Categories/event/{eventID}");

                return Json(categories ?? new List<CategoryViewModel>());
            }
            catch (HttpRequestException)
            {
                return StatusCode(500, "Unable to load categories.");
            }
        }

        // Submit the participant's enrolment to the API.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnrollmentFormViewModel model)
        {
            string? token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (model.EventID <= 0 || model.CategoryID <= 0)
            {
                ModelState.AddModelError("", "Please select an event and category.");
            }

            if (!ModelState.IsValid)
            {
                await LoadEvents(model);
                return View(model);
            }

            try
            {
                // Send only the two fields required by the API.
                var request = new CreateEnrollmentViewModel
                {
                    EventID = model.EventID,
                    CategoryID = model.CategoryID
                };

                await _apiService.PostWithTokenAsync<CreateEnrollmentViewModel, EnrollmentViewModel>(
                    "api/Enrollments", request, token);

                TempData["Success"] = "You have successfully enrolled in the event!";

                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Unable to complete your enrolment. You may already be enrolled in this event, or the selected category may be invalid.");

                await LoadEvents(model);
                return View(model);
            }
        }

        // Reload the event dropdown when the form needs to be displayed again.
        private async Task LoadEvents(EnrollmentFormViewModel model)
        {
            model.Events = await _apiService.GetAsync<List<EventViewModel>>(
                "api/Events") ?? new List<EventViewModel>();

            // Reload the categories for the previously selected event.
            if (model.EventID > 0)
            {
                model.Categories =
                    await _apiService.GetAsync<List<CategoryViewModel>>(
                        $"api/Categories/event/{model.EventID}")
                    ?? new List<CategoryViewModel>();
            }
        }
    }//end of class
}//end of namespace
