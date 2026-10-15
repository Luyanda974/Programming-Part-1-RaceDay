using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using System.Security.Claims;

namespace RaceDay.Controllers
{//start of namespce

    // Provides endpoints for viewing and managing RaceDay events.
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        // Provides access to the RaceDay database.
        private readonly RaceDayContext _context;

        // Receives the database context through dependency injection.
        public EventsController(RaceDayContext context)
        {
            _context = context;
        }

        // Reads the logged-in user's ID from their verified JWT token.
        // This prevents the client from choosing another organiser's ID.
        private bool TryGetCurrentUserID(out int userID)
        {
            string? userIDValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIDValue, out userID);
        }

        // GET: api/Events
        // Allows anyone to browse available events.
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents()
        {
            // Read all events without tracking them for editing.
            var events = await _context.Events
                .AsNoTracking()
                .ToListAsync();

            // Return HTTP 200 OK and the list of events.
            return Ok(events);
        }

        // GET: api/Events/5
        // Allows anyone to view one event by its ID.
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvent(int id)
        {
            // Find the event matching the supplied ID.
            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);

            // Return HTTP 404 if the event does not exist.
            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            // Return HTTP 200 with the event details.
            return Ok(eventItem);
        }

        // POST: api/Events
        // Only an authenticated organiser can create an event.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateEvent(
            [FromBody] EventRequest request)
        {
            // Get the organiser's ID from the authenticated token.
            if (!TryGetCurrentUserID(out int organiserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Confirm that the selected event type exists.
            bool eventTypeExists = await _context.EventTypes
                .AnyAsync(type => type.EventTypeID == request.EventTypeID);

            if (!eventTypeExists)
            {
                return BadRequest(new
                {
                    message = "The selected event type does not exist."
                });
            }

            // Build the event using the validated request details.
            var eventItem = new Event
            {
                EventName = request.EventName,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                Distance = request.Distance,

                // The organiser ID comes from the JWT, not the request.
                OrganiserID = organiserID,

                EventTypeID = request.EventTypeID
            };

            // Add the event to the database.
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            // Return HTTP 201 Created and a link to the new event.
            return CreatedAtAction(
                nameof(GetEvent),
                new { id = eventItem.Id },
                eventItem);
        }

        // PUT: api/Events/5
        // Only the organiser who owns the event can update it.
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] EventRequest request)
        {
            // Get the logged-in organiser's ID from their JWT.
            if (!TryGetCurrentUserID(out int organiserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Find the existing event.
            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(item => item.Id == id);

            // Return HTTP 404 if no matching event exists.
            if (existingEvent == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            // Prevent an organiser from changing another organiser's event.
            if (existingEvent.OrganiserID != organiserID)
            {
                return Forbid();
            }

            // Confirm that the selected event type exists.
            bool eventTypeExists = await _context.EventTypes
                .AnyAsync(type => type.EventTypeID == request.EventTypeID);

            if (!eventTypeExists)
            {
                return BadRequest(new
                {
                    message = "The selected event type does not exist."
                });
            }

            // Update only the fields organisers are allowed to change.
            existingEvent.EventName = request.EventName;
            existingEvent.Description = request.Description;
            existingEvent.EventDate = request.EventDate;
            existingEvent.Location = request.Location;
            existingEvent.Distance = request.Distance;
            existingEvent.EventTypeID = request.EventTypeID;

            // Keep the original OrganiserID unchanged.
            await _context.SaveChangesAsync();

            // Return the updated event.
            return Ok(existingEvent);
        }

        // DELETE: api/Events/5
        // Only the organiser who owns the event can delete it.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            // Get the logged-in organiser's ID from their JWT.
            if (!TryGetCurrentUserID(out int organiserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Find the event that the organiser wants to delete.
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(item => item.Id == id);

            // Return HTTP 404 if the event does not exist.
            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            // Prevent an organiser from deleting another organiser's event.
            if (eventItem.OrganiserID != organiserID)
            {
                return Forbid();
            }

            // Check whether the event has categories or enrolments.
            // These records depend on the event, so deleting it could
            // break the relationships stored in the database.
            bool hasCategories = await _context.Categories
                .AnyAsync(category => category.EventID == id);

            bool hasEnrollments = await _context.Enrollments
                .AnyAsync(enrollment => enrollment.EventID == id);

            // Ask the organiser to deal with dependent records first.
            if (hasCategories || hasEnrollments)
            {
                return Conflict(new
                {
                    message =
                        "This event cannot be deleted while it has categories or enrolments."
                });
            }

            // Remove the event and save the deletion.
            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            // HTTP 204 means the deletion succeeded with no response body.
            return NoContent();
        }
    }


}//end of namespace
