using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using System.Security.Claims;
using static RaceDay.DTOs.UpdateCategory;

namespace RaceDay.Controllers
{//start of namepsace
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {//start of class

        // Provides access to categories, events and enrolments.
        private readonly RaceDayContext _context;

        // Receives the database context through dependency injection.
        public CategoriesController(RaceDayContext context)
        {
            _context = context;
        }

        // Reads the logged-in user's ID from the verified JWT token.
        private bool TryGetCurrentUserID(out int userID)
        {
            string? userIDValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIDValue, out userID);
        }

        // Checks whether the logged-in organiser owns the specified event.
        private async Task<bool> OrganiserOwnsEvent(
            int eventID, int organiserID)
        {
            return await _context.Events.AnyAsync(eventItem =>
                eventItem.Id == eventID &&
                eventItem.OrganiserID == organiserID);
        }

        // GET: api/Categories
        // Allows anyone to browse event categories.
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            // Retrieve all categories without tracking them for editing.
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            // Return HTTP 200 and the category list.
            return Ok(categories);
        }

        // GET: api/Categories/5
        // Returns one category using its ID.
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(int id)
        {
            // Find the requested category.
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.CategoryID == id);

            // Return HTTP 404 if the category does not exist.
            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            // Return the matching category.
            return Ok(category);
        }

        // GET: api/Categories/event/5
        // Returns all categories belonging to a specific event.
        [HttpGet("event/{eventID:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoriesForEvent(int eventID)
        {
            // Check that the event exists.
            bool eventExists = await _context.Events
                .AnyAsync(item => item.Id == eventID);

            if (!eventExists)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            // Retrieve categories linked to the requested event.
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(item => item.EventID == eventID)
                .ToListAsync();

            // Return HTTP 200, including an empty list if none exist.
            return Ok(categories);
        }

        // POST: api/Categories
        // Only the organiser who owns the event can add a category.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CreateCategoryRequest request)
        {
            // Read the organiser's ID from the verified JWT token.
            if (!TryGetCurrentUserID(out int organiserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Confirm that the organiser owns the selected event.
            bool ownsEvent = await OrganiserOwnsEvent(
                request.EventID, organiserID);

            if (!ownsEvent)
            {
                // Distinguish a missing event from an event owned by someone else.
                bool eventExists = await _context.Events
                    .AnyAsync(item => item.Id == request.EventID);

                if (!eventExists)
                {
                    return NotFound(new
                    {
                        message = "Event not found."
                    });
                }

                return Forbid();
            }

            // Create the category using the validated request.
            var category = new Categorys
            {
                CategoryName = request.CategoryName,
                CategoryType = request.CategoryType,
                EventID = request.EventID
            };

            // Add the category to the database.
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Return HTTP 201 and the newly created category.
            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryID },
                category);
        }

        // PUT: api/Categories/5
        // Only the organiser who owns the category's event can update it.
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] UpdateCategoryRequest request)
        {
            // Read the logged-in organiser's ID from their JWT.
            if (!TryGetCurrentUserID(out int organiserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Find the category that needs to be updated.
            var category = await _context.Categories
                .FirstOrDefaultAsync(item => item.CategoryID == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            // Check that the organiser owns the event linked to this category.
            bool ownsEvent = await OrganiserOwnsEvent(
                category.EventID, organiserID);

            if (!ownsEvent)
            {
                return Forbid();
            }

            // Update the category's editable fields.
            category.CategoryName = request.CategoryName;
            category.CategoryType = request.CategoryType;

            // Save the changes to SQL Server.
            await _context.SaveChangesAsync();

            // Return the updated category.
            return Ok(category);
        }

        // DELETE: api/Categories/5
        // Only the organiser who owns the category's event can delete it.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Read the organiser's ID from the verified JWT.
            if (!TryGetCurrentUserID(out int organiserID))
            {
                return Unauthorized(new
                {
                    message = "A valid user ID was not found in the token."
                });
            }

            // Find the category that needs to be deleted.
            var category = await _context.Categories
                .FirstOrDefaultAsync(item => item.CategoryID == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            // Confirm that the logged-in organiser owns the event.
            bool ownsEvent = await OrganiserOwnsEvent(
                category.EventID, organiserID);

            if (!ownsEvent)
            {
                return Forbid();
            }

            // Do not delete a category while enrolments still refer to it.
            bool hasEnrollments = await _context.Enrollments
                .AnyAsync(item => item.CategoryID == id);

            if (hasEnrollments)
            {
                return Conflict(new
                {
                    message =
                        "This category cannot be deleted while enrolments refer to it."
                });
            }

            // Remove the category and save the deletion.
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            // Return HTTP 204 when deletion succeeds.
            return NoContent();
        }

    }//end of class
}//end of namespace
