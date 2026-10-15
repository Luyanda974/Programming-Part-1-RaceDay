using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;


namespace RaceDay.Controllers
{//start of namespace
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {//start of class

        // Provides access to the RaceDay database.
        private readonly RaceDayContext _context;

        // Receives the database context through dependency injection.
        public EnrollmentsController(RaceDayContext context)
        {
            _context = context;
        }

        // Reads the logged-in user's ID from their verified JWT token.
        private bool TryGetCurrentUserID(out int userID)
        {
            string? userIDValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIDValue, out userID);
        }

        // Checks whether the logged-in organiser owns a particular event.
        private async Task<bool> OrganiserOwnsEvent(
            int eventID, int organiserID)
        {
            return await _context.Events.AnyAsync(eventItem =>
                eventItem.Id == eventID &&
                eventItem.OrganiserID == organiserID);
        }

        // GET: api/Enrollments
        // Participants can view only their own enrolments.
        [HttpGet]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            // Get the participant's ID from the authenticated token.
            if (!TryGetCurrentUserID(out int participantID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Retrieve only enrolments belonging to the logged-in participant.
            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(item => item.ParticipantID == participantID)
                .ToListAsync();

            // Return the participant's enrolments.
            return Ok(enrollments);
        }

        // GET: api/Enrollments/5
        // A participant can view their own enrolment.
        // An organiser can view an enrolment for an event they own.
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Participant,Organiser")]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            // Find the requested enrolment.
            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.EnrollmentID == id);

            // Return HTTP 404 if the enrolment does not exist.
            if (enrollment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            // Read the ID of the authenticated user.
            if (!TryGetCurrentUserID(out int currentUserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Participants may access only their own enrolments.
            if (User.IsInRole("Participant"))
            {
                if (enrollment.ParticipantID != currentUserID)
                {
                    return Forbid();
                }

                return Ok(enrollment);
            }

            // Organisers may access enrolments only for their own events.
            bool ownsEvent = await OrganiserOwnsEvent(
                enrollment.EventID, currentUserID);

            if (!ownsEvent)
            {
                return Forbid();
            }

            return Ok(enrollment);
        }

        // GET: api/Enrollments/event/5
        // Allows an organiser to view enrolments for their own event.
        [HttpGet("event/{eventID:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetEnrollmentsForEvent(int eventID)
        {
            // Read the organiser's ID from the verified JWT token.
            if (!TryGetCurrentUserID(out int organiserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Confirm that the requested event belongs to this organiser.
            bool ownsEvent = await OrganiserOwnsEvent(
                eventID, organiserID);

            if (!ownsEvent)
            {
                bool eventExists = await _context.Events
                    .AnyAsync(item => item.Id == eventID);

                if (!eventExists)
                {
                    return NotFound(new
                    {
                        message = "Event not found."
                    });
                }

                return Forbid();
            }

            // Retrieve the enrolments for the organiser's event.
            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(item => item.EventID == eventID)
                .ToListAsync();

            // Return the list, even if the event has no enrolments yet.
            return Ok(enrollments);
        }

        // POST: api/Enrollments
        // Allows a participant to enrol themselves in an event category.
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> CreateEnrollment(
            [FromBody] CreateEnrollmentRequest request)
        {
            // Obtain the participant's ID from the authenticated JWT.
            if (!TryGetCurrentUserID(out int participantID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Confirm that the selected event exists.
            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(item =>
                    item.Id == request.EventID);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            // Confirm that the selected category exists and belongs
            // to the event the participant is entering.
            bool categoryMatchesEvent = await _context.Categories
                .AnyAsync(category =>
                    category.CategoryID == request.CategoryID &&
                    category.EventID == request.EventID);

            if (!categoryMatchesEvent)
            {
                return BadRequest(new
                {
                    message =
                        "The selected category does not belong to this event."
                });
            }

            // Prevent the same participant from entering the same event twice.
            bool alreadyEnrolled = await _context.Enrollments
                .AnyAsync(item =>
                    item.ParticipantID == participantID &&
                    item.EventID == request.EventID);

            if (alreadyEnrolled)
            {
                return Conflict(new
                {
                    message =
                        "You are already enrolled in this event."
                });
            }

            // Create the enrolment using the authenticated participant's ID.
            var enrollment = new Enrollment
            {
                EnrollmentDate = DateTime.Today,
                ParticipantID = participantID,
                EventID = request.EventID,
                CategoryID = request.CategoryID
            };

            // Save the enrolment to the database.
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Return HTTP 201 Created and the saved enrolment.
            return CreatedAtAction(
                nameof(GetEnrollment),
                new { id = enrollment.EnrollmentID },
                enrollment);
        }

        // DELETE: api/Enrollments/5
        // A participant may cancel their own enrolment if no result exists.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> CancelEnrollment(int id)
        {
            // Read the participant's ID from their verified JWT.
            if (!TryGetCurrentUserID(out int participantID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Find the requested enrolment.
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(item => item.EnrollmentID == id);

            if (enrollment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            // Prevent participants from cancelling another person's entry.
            if (enrollment.ParticipantID != participantID)
            {
                return Forbid();
            }

            // Keep the enrolment if a result has already been recorded.
            bool hasResult = await _context.Results
                .AnyAsync(result => result.EnrollmentID == id);

            if (hasResult)
            {
                return Conflict(new
                {
                    message =
                        "This enrolment cannot be cancelled because a result has been recorded."
                });
            }

            // Remove the enrolment and save the deletion.
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            // Return HTTP 204 when cancellation succeeds.
            return NoContent();
        }

    }//end of class
}//end of namespace
