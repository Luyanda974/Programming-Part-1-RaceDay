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
                // Retrieve the participant's enrolments.
                var enrollments =
                    await _apiService.GetWithTokenAsync<List<EnrollmentViewModel>>(
                        "api/Enrollments", token);

                enrollments ??= new List<EnrollmentViewModel>();

                // Retrieve all events from the API.
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                // Retrieve all categories from the API.
                var categories = await _apiService.GetAsync<List<CategoryViewModel>>(
                    "api/Categories");

                // Match the event and category to each enrolment.
                foreach (var enrollment in enrollments)
                {
                    var selectedEvent = events?.FirstOrDefault(
                        e => e.EventID == enrollment.EventID);

                    var selectedCategory = categories?.FirstOrDefault(
                        c => c.CategoryID == enrollment.CategoryID);

                    // Display names instead of database IDs.
                    enrollment.EventName =
                        selectedEvent?.EventName ?? "Unknown Event";

                    enrollment.CategoryName =
                        selectedCategory?.CategoryName ?? "Unknown Category";
                }

                return View(enrollments);
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
        // GET: Display enrolments belonging to the organiser's selected event.
        public async Task<IActionResult> Manage(int? eventID)
        {
            // Retrieve the organiser's session information.
            string? token = HttpContext.Session.GetString("JWToken");
            string? userIDValue = HttpContext.Session.GetString("UserID");
            string? role = HttpContext.Session.GetString("Role");

            // Check whether the user is logged in.
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Only organisers may manage event enrolments.
            if (role != "Organiser")
            {
                return Forbid();
            }

            // Confirm that the organiser ID is valid.
            if (!int.TryParse(userIDValue, out int organiserID))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Retrieve all events from the API.
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                events ??= new List<EventViewModel>();

                // Keep only events belonging to this organiser.
                var myEvents = events
                    .Where(item => item.OrganiserID == organiserID)
                    .ToList();

                // Prepare the page model.
                var model = new ManageEnrollmentsViewModel
                {
                    Events = myEvents,
                    EventID = eventID
                };

                // If an event has been selected, retrieve its enrolments.
                if (eventID.HasValue)
                {
                    // Confirm that the selected event belongs to this organiser.
                    bool ownsEvent = myEvents.Any(
                        item => item.EventID == eventID.Value);

                    if (!ownsEvent)
                    {
                        return Forbid();
                    }

                    // Retrieve enrolments using the authenticated API endpoint.
                    var enrollments =
                        await _apiService.GetWithTokenAsync<List<EnrollmentViewModel>>(
                            $"api/Enrollments/event/{eventID.Value}", token);

                    model.Enrollments =
                        enrollments ?? new List<EnrollmentViewModel>();

                    // Retrieve category and event information for display.
                    foreach (var enrollment in model.Enrollments)
                    {
                        var category =
                            await _apiService.GetAsync<CategoryViewModel>(
                                $"api/Categories/{enrollment.CategoryID}");

                        enrollment.CategoryName =
                            category?.CategoryName ?? "Unknown Category";

                        var selectedEvent = myEvents.FirstOrDefault(
                            item => item.EventID == enrollment.EventID);

                        enrollment.EventName =
                            selectedEvent?.EventName ?? "Unknown Event";
                    }
                }

                return View(model);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to retrieve enrolments. Please try again.";

                return View(new ManageEnrollmentsViewModel());
            }
        }
    }//end of class
}//end of namespace
