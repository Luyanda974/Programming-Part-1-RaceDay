using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using RaceDay.Services;

namespace RaceDay.Controllers
{//start of namespace

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {//start of class

        // for access to the database
        private readonly RaceDayContext _context;
     //for password hashing and verification
        private readonly PasswordService _passwordService;

        private readonly TokenServices _tokenServices;

        //recieved the required services
        public AuthController(RaceDayContext context, PasswordService passwordService, TokenServices tokenServices)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenServices=tokenServices;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            bool emailExists = await _context.Users.AnyAsync(user => user.Email == request.Email);
            if (emailExists)
            {
                return Conflict(new { message = "An account already exists." });
            }
            //create new user with registration details provided
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                Role = request.Role

            };

            //add user to the database
            _context.Users.Add(user);

            //save new account
            await _context.SaveChangesAsync();

            //return only the safe account info and never the password hash
            return Created($"/api/users/{user.UserID}", new { user.UserID, user.FirstName, user.LastName, user.Email,user.Role });

        }
        [HttpPost("login")]
        public async Task<IActionResult>Login([FromBody] LoginRequest request)
        {
            //find account using the submitted email address
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == request.Email);

            //Reject the login if the account does not exist or if the password does not match the stored

            if (user == null || !_passwordService.VerifyPassword(user.PasswordHash, request.Password))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            //Generate a JWT token for the authenticated user
            string token = _tokenServices.CreateToken(user.UserID, user.Email, user.Role);
            //return the token and safe account information
            return Ok(new {token,user.UserID, user.FirstName, user.LastName,user.Email,user.Role});

          
            }
        [HttpGet("protected")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Protected()
        {
            // Return a message if authentication succeeds.
            return Ok(new
            {
                message = "You have successfully accessed a protected endpoint."
            });
        }

        }//end of class
}//end of namespace
