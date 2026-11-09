using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{//start of namespace
    public class CategoriesController : Controller
    {//start of controller
        private readonly RaceDayApiService _apiService;

        // Connect the controller to the API service.
        public CategoriesController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        // Display categories belonging to the logged-in organiser.
        public async Task<IActionResult> Manage()
        {
            // Retrieve the organiser's login information.
            string? token = HttpContext.Session.GetString("JWToken");
            string? userIDValue = HttpContext.Session.GetString("UserID");
            string? role = HttpContext.Session.GetString("Role");

            // Redirect users who are not logged in.
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Only organisers may manage categories.
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
                // Retrieve all categories from the API.
                var categories =
                    await _apiService.GetAsync<List<CategoryViewModel>>(
                        "api/Categories");

                categories ??= new List<CategoryViewModel>();

                // Retrieve all events from the API.
                var events =
                    await _apiService.GetAsync<List<EventViewModel>>(
                        "api/Events");

                events ??= new List<EventViewModel>();

                // Keep only events created by this organiser.
                var myEvents = events
                    .Where(item => item.OrganiserID == organiserID)
                    .ToList();

                // Display only categories belonging to the organiser's events.
                var myCategories = categories
                    .Where(category => myEvents.Any(
                        eventItem => eventItem.EventID == category.EventID))
                    .ToList();

                // Match each category to its event name.
                foreach (var category in myCategories)
                {
                    var selectedEvent = myEvents.FirstOrDefault(
                        item => item.EventID == category.EventID);

                    category.EventName =
                        selectedEvent?.EventName ?? "Unknown Event";
                }

                return View(myCategories);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to retrieve categories. Please try again.";

                return View(new List<CategoryViewModel>());
            }
        }
        // GET: Display the Create Category form.
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Retrieve the organiser's session information.
            string? token = HttpContext.Session.GetString("JWToken");
            string? role = HttpContext.Session.GetString("Role");
            string? userIDValue = HttpContext.Session.GetString("UserID");

            // Check whether the organiser is logged in.
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Only organisers may create categories.
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

                // Only show events belonging to this organiser.
                var myEvents = events
                    .Where(item => item.OrganiserID == organiserID)
                    .ToList();

                // Pass the organiser's events to the Create view.
                ViewBag.Events = myEvents;

                return View(new CreateCategoryViewModel());
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Unable to load your events.";
                ViewBag.Events = new List<EventViewModel>();

                return View(new CreateCategoryViewModel());
            }
        }
        // POST: Send the new category to the API.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            // Retrieve the organiser's session information.
            string? token = HttpContext.Session.GetString("JWToken");
            string? role = HttpContext.Session.GetString("Role");
            string? userIDValue = HttpContext.Session.GetString("UserID");

            // Check whether the organiser is logged in.
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Only organisers may create categories.
            if (role != "Organiser")
            {
                return Forbid();
            }

            // Confirm that the organiser ID is valid.
            if (!int.TryParse(userIDValue, out int organiserID))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Retrieve the organiser's events for the dropdown.
            List<EventViewModel> myEvents;

            try
            {
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                events ??= new List<EventViewModel>();

                myEvents = events
                    .Where(item => item.OrganiserID == organiserID)
                    .ToList();

                ViewBag.Events = myEvents;
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Unable to load your events.";
                ViewBag.Events = new List<EventViewModel>();

                return View(model);
            }

            // Check the submitted form values.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Confirm that the selected event belongs to this organiser.
            bool selectedEventExists = myEvents.Any(
                item => item.EventID == model.EventID);

            if (!selectedEventExists)
            {
                ModelState.AddModelError(
                    "EventID", "Please select one of your own events.");

                return View(model);
            }

            try
            {
                // Send only the fields expected by the API.
                var request = new
                {
                    model.CategoryName,
                    model.CategoryType,
                    model.EventID
                };

                // Create the category through the protected API endpoint.
                await _apiService.PostWithTokenAsync<object, CategoryViewModel>(
                    "api/Categories", request, token);

                // Display a success message.
                TempData["SuccessMessage"] =
                    "Category created successfully.";

                return RedirectToAction("Manage");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the category. Please try again.");

                return View(model);
            }
        }

    }//emd of controller
}//end of namespace
