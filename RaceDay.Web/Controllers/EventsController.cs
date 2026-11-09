using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{//start of namespace
    public class EventsController : Controller
    {//start of class
        private readonly RaceDayApiService _apiService;

        public EventsController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        // Display all available events to participants.
        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                return View(events ?? new List<EventViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to load events. Please try again later.";

                return View(new List<EventViewModel>());
            }
        }

        // Display only the events belonging to the logged-in organiser.
        public async Task<IActionResult> Manage()
        {
            string? token = HttpContext.Session.GetString("JWToken");
            string? userID = HttpContext.Session.GetString("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token) ||
                string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (role != "Organiser")
            {
                return Forbid();
            }

            if (!int.TryParse(userID, out int organiserID))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var allEvents =
                    await _apiService.GetAsync<List<EventViewModel>>(
                        "api/Events");

                allEvents ??= new List<EventViewModel>();

                var organiserEvents = allEvents
                    .Where(eventItem =>
                        eventItem.OrganiserID == organiserID)
                    .ToList();

                return View(organiserEvents);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to retrieve your events. Please try again.";

                return View(new List<EventViewModel>());
            }
        }

        // Display the Create Event form.
        public async Task<IActionResult> Create()
        {
            // Check that the user is logged in.
            string? token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Only organisers may create events.
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return Forbid();
            }

            try
            {
                // Retrieve event types from the API.
                var eventTypes =
                    await _apiService.GetAsync<List<EventTypeViewModel>>(
                        "api/Events/eventtypes");

                // Prepare the form and its dropdown options.
                var model = new CreateEventViewModel
                {
                    EventTypes = eventTypes ?? new List<EventTypeViewModel>()
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to load event types. Please try again.";

                return View(new CreateEventViewModel());
            }
        }

        // Receive the completed Create Event form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            string? token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return Forbid();
            }

            // Retrieve event types again for the dropdown if validation fails.
            try
            {
                var eventTypes =
                    await _apiService.GetAsync<List<EventTypeViewModel>>(
                        "api/Events/eventtypes");

                model.EventTypes =
                    eventTypes ?? new List<EventTypeViewModel>();
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to load event types. Please try again.";
            }

            // Check the form fields before sending them to the API.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Send the event details using the organiser's JWT.
                var createdEvent =
                    await _apiService.PostWithTokenAsync<
                        CreateEventViewModel, EventViewModel>(
                            "api/Events", model, token);

                TempData["SuccessMessage"] =
                    "Your event was created successfully.";

                return RedirectToAction("Manage");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the event. Please check your details and try again.");

                return View(model);
            }
        }
        // Display the Edit Event form.
public async Task<IActionResult> Edit(int id)
{
    // Retrieve the organiser's login details.
    string? token = HttpContext.Session.GetString("JWToken");
    string? userID = HttpContext.Session.GetString("UserID");

    // Redirect users who are not logged in.
    if (string.IsNullOrEmpty(token) ||
        string.IsNullOrEmpty(userID))
    {
        return RedirectToAction("Login", "Auth");
    }

    // Only organisers may edit events.
    if (HttpContext.Session.GetString("Role") != "Organiser")
    {
        return Forbid();
    }

    if (!int.TryParse(userID, out int organiserID))
    {
        return RedirectToAction("Login", "Auth");
    }

    try
    {
        // Retrieve the selected event from the API.
        var eventItem = await _apiService.GetAsync<EventViewModel>(
            $"api/Events/{id}");

        if (eventItem == null)
        {
            return NotFound();
        }

        // Make sure this event belongs to the logged-in organiser.
        if (eventItem.OrganiserID != organiserID)
        {
            return Forbid();
        }

        // Retrieve the available event types.
        var eventTypes =
            await _apiService.GetAsync<List<EventTypeViewModel>>(
                "api/Events/eventtypes");

        // Transfer the existing event details into the edit form.
        var model = new CreateEventViewModel
        {
            EventName = eventItem.EventName,
            Description = eventItem.Description,
            EventDate = eventItem.EventDate,
            Location = eventItem.Location,
            Distance = eventItem.Distance,
            EventTypeID = eventItem.EventTypeID,
            EventTypes = eventTypes ?? new List<EventTypeViewModel>()
        };

        return View(model);
    }
    catch (HttpRequestException)
    {
        return RedirectToAction("Manage");
    }
}

// Receive the updated event details.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(
    int id, CreateEventViewModel model)
{
    string? token = HttpContext.Session.GetString("JWToken");

    if (string.IsNullOrEmpty(token))
    {
        return RedirectToAction("Login", "Auth");
    }

    if (HttpContext.Session.GetString("Role") != "Organiser")
    {
        return Forbid();
    }

    try
    {
        // Reload event types if the form needs to be displayed again.
        var eventTypes =
            await _apiService.GetAsync<List<EventTypeViewModel>>(
                "api/Events/eventtypes");

        model.EventTypes =
            eventTypes ?? new List<EventTypeViewModel>();
    }
    catch (HttpRequestException)
    {
        ViewBag.ErrorMessage =
            "Unable to load event types. Please try again.";
    }

    if (!ModelState.IsValid)
    {
        return View(model);
    }

    try
    {
        // Send the updated details to the API.
        // The API checks that this organiser owns the event.
        await _apiService.PutWithTokenAsync(
            $"api/Events/{id}", model, token);

        TempData["SuccessMessage"] =
            "Your event was updated successfully.";

        return RedirectToAction("Manage");
    }
    catch (HttpRequestException)
    {
        ModelState.AddModelError(
            string.Empty,
            "Unable to update the event. Please check your details and try again.");

        return View(model);
    }
}
        // POST: Delete an event.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve the organiser's JWT token from the session.
            string? token = HttpContext.Session.GetString("JWToken");

            // Check whether the user is logged in.
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Only organisers may delete events.
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return Forbid();
            }

            try
            {
                // Send the authenticated DELETE request to the API.
                await _apiService.DeleteWithTokenAsync(
                    $"api/Events/{id}", token);

                // Display a success message after deletion.
                TempData["SuccessMessage"] = "Event deleted successfully.";

                return RedirectToAction("Manage");
            }
            catch (HttpRequestException)
            {
                // Display an error if the API refuses the deletion.
                TempData["ErrorMessage"] =
                    "Unable to delete this event. It may have existing categories or enrolments.";

                return RedirectToAction("Manage");
            }
        }

    }//end of class
}//end of namespace
