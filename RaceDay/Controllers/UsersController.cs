using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers
{//start of namespacce
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {//start of class

        private readonly RaceDayContext _context;

        // Get the database context through dependency injection.
        public UsersController(RaceDayContext context)
        {
            _context = context;
        }

        // Reads the logged-in user's ID from their JWT token.
        private int? GetCurrentUserId()
        {
            string? userIdValue =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Return null if the token does not contain a valid user ID.
            if (!int.TryParse(userIdValue, out int userId))
            {
                return null;
            }

            return userId;
        }

        // GET: api/Users/5
        // Returns a user's profile, but only if it belongs to the logged-in user.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            int? currentUserId = GetCurrentUserId();

            // Require a valid user ID in the authentication token.
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            // Prevent users from viewing another user's profile.
            if (currentUserId.Value != id)
            {
                return Forbid();
            }

            // Find the user in the database.
            User? user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            // Return safe profile details only.
            // Never return the password hash.
            return Ok(new
            {
                user.UserID,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role
            });
        }

        // PUT: api/Users/5
        // Updates the logged-in user's first name, surname, and email.
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(
            int id,
            UpdateProfileRequest request)
        {
            int? currentUserId = GetCurrentUserId();

            // Require a valid user ID in the authentication token.
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            // Users may update only their own profiles.
            if (currentUserId.Value != id)
            {
                return Forbid();
            }

            // Find the existing user record.
            User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            // Remove extra spaces from the supplied values.
            string email = request.Email.Trim();
            string firstName = request.FirstName.Trim();
            string lastName = request.LastName.Trim();

            // Do not accept values that become empty after trimming.
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName))
            {
                return BadRequest(new
                {
                    message = "First name, last name, and email are required."
                });
            }

            // Check whether another user already uses this email.
            bool emailAlreadyExists = await _context.Users.AnyAsync(
                u => u.Email == email && u.UserID != id);

            if (emailAlreadyExists)
            {
                return Conflict(new
                {
                    message = "That email address is already in use."
                });
            }

            // Update only the permitted profile fields.
            // The user ID, role, and password hash remain unchanged.
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;

            // Save the changes to the database.
            await _context.SaveChangesAsync();

            // Return the updated profile without sensitive information.
            return Ok(new
            {
                message = "Profile updated successfully.",
                user.UserID,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role
            });
        }

    }//end of class
}//end of namespace
