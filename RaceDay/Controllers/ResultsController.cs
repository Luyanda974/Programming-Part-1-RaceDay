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
    [Route("api/controller")]
    public class ResultsController : ControllerBase
    {//start of class

        // Provides access to results, enrolments and events.
        private readonly RaceDayContext _context;

        // Receives the database context through dependency injection.
        public ResultsController(RaceDayContext context)
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

        // Checks whether an organiser owns the event linked to an enrolment.
        private async Task<bool> OrganiserOwnsEnrollment(
            int enrollmentID, int organiserID)
        {
            return await _context.Enrollments
                .AnyAsync(enrollment =>
                    enrollment.EnrollmentID == enrollmentID &&
                    _context.Events.Any(eventItem =>
                        eventItem.Id == enrollment.EventID &&
                        eventItem.OrganiserID == organiserID));
        }

        // GET: api/Results
        // Allows anyone to view recorded results.
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetResults()
        {
            // Retrieve all recorded results without tracking them for editing.
            var results = await _context.Results
                .AsNoTracking()
                .ToListAsync();

            // Return HTTP 200 with the results.
            return Ok(results);
        }

        // GET: api/Results/5
        // Returns one result by its ID.
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetResult(int id)
        {
            // Find the result matching the supplied ID.
            var result = await _context.Results
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.ResultID == id);

            // Return HTTP 404 if the result does not exist.
            if (result == null)
            {
                return NotFound(new
                {
                    message = "Result not found."
                });
            }

            // Return the matching result.
            return Ok(result);
        }

        // GET: api/Results/event/5
        // Returns all results associated with enrolments in an event.
        [HttpGet("event/{eventID:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetResultsForEvent(int eventID)
        {
            // Confirm that the requested event exists.
            bool eventExists = await _context.Events
                .AnyAsync(item => item.Id == eventID);

            if (!eventExists)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            // Find results whose enrolments belong to this event.
            var results = await _context.Results
                .AsNoTracking()
                .Where(result => _context.Enrollments.Any(enrollment =>
                    enrollment.EnrollmentID == result.EnrollmentID &&
                    enrollment.EventID == eventID))
                .ToListAsync();

            // Return HTTP 200, even if no results have been recorded yet.
            return Ok(results);
        }

        // POST: api/Results
        // Only an organiser can record results for an event they own.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateResult(
            [FromBody] CreateResultRequest request)
        {
            // Read the organiser's ID from the authenticated JWT token.
            if (!TryGetCurrentUserID(out int organiserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // A finishing time must be greater than zero.
            if (request.FinishTime <= TimeSpan.Zero)
            {
                return BadRequest(new
                {
                    message = "The finishing time must be greater than zero."
                });
            }

            // Confirm that the enrolment exists.
            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(item =>
                    item.EnrollmentID == request.EnrollmentID);

            if (enrollment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            // Confirm that the enrolment belongs to an event owned
            // by the currently authenticated organiser.
            bool ownsEnrollment = await OrganiserOwnsEnrollment(
                request.EnrollmentID, organiserID);

            if (!ownsEnrollment)
            {
                return Forbid();
            }

            // Only one result should be recorded for each enrolment.
            bool resultAlreadyExists = await _context.Results
                .AnyAsync(item =>
                    item.EnrollmentID == request.EnrollmentID);

            if (resultAlreadyExists)
            {
                return Conflict(new
                {
                    message =
                        "A result has already been recorded for this enrolment."
                });
            }

            // Create the result using the validated request details.
            var result = new Result
            {
                EnrollmentID = request.EnrollmentID,
                FinishTime = request.FinishTime,
                FinishPosition = request.FinishPosition
            };

            // Save the result to the database.
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            // Return HTTP 201 Created and the new result.
            return CreatedAtAction(
                nameof(GetResult),
                new { id = result.ResultID },
                result);
        }

        // PUT: api/Results/5
        // Only the organiser who owns the event can update its result.
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateResult(
            int id,
            [FromBody] UpdateResultRequest request)
        {
            // Read the organiser's ID from the verified JWT.
            if (!TryGetCurrentUserID(out int organiserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Check that the submitted finishing time is valid.
            if (request.FinishTime <= TimeSpan.Zero)
            {
                return BadRequest(new
                {
                    message = "The finishing time must be greater than zero."
                });
            }

            // Find the existing result.
            var result = await _context.Results
                .FirstOrDefaultAsync(item => item.ResultID == id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Result not found."
                });
            }

            // Confirm that this organiser owns the event for this result.
            bool ownsEnrollment = await OrganiserOwnsEnrollment(
                result.EnrollmentID, organiserID);

            if (!ownsEnrollment)
            {
                return Forbid();
            }

            // Update the finishing time and position.
            result.FinishTime = request.FinishTime;
            result.FinishPosition = request.FinishPosition;

            // Save the changes to SQL Server.
            await _context.SaveChangesAsync();

            // Return the updated result.
            return Ok(result);
        }

        // DELETE: api/Results/5
        // Only the organiser who owns the event can delete its result.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            // Read the organiser's ID from the verified JWT.
            if (!TryGetCurrentUserID(out int organiserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Find the result that needs to be deleted.
            var result = await _context.Results
                .FirstOrDefaultAsync(item => item.ResultID == id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Result not found."
                });
            }

            // Confirm that the organiser owns the event for this result.
            bool ownsEnrollment = await OrganiserOwnsEnrollment(
                result.EnrollmentID, organiserID);

            if (!ownsEnrollment)
            {
                return Forbid();
            }

            // Remove the result and save the deletion.
            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            // Return HTTP 204 when deletion succeeds.
            return NoContent();
        }

    }//end of class
}//end of namespace
