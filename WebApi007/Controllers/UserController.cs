using BL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi007.Data;

namespace WebApi007.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AppDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Helper method for logging
        private void LogAction(string action, int id)
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation("{Action} performed on User {Id} by IP {IPAddress}", action, id, ipAddress);
        }

        // GET: api/users
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();

            // Log user retrieval
            LogAction("Users retrieved", 0);  // No specific user ID for retrieving all users

            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                _logger.LogWarning("User {Id} not found", id);
                return NotFound();
            }

            // Log user retrieval
            LogAction("User retrieved", id);

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("User object is null during creation.");
                return BadRequest("Invalid user object.");
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            // Log user creation
            LogAction("User created", user.Id);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User user)
        {
            var existing = _context.Users.Find(id);
            if (existing == null)
            {
                _logger.LogWarning("User {Id} not found during update", id);
                return NotFound();
            }

            existing.Username = user.Username;
            existing.Password = user.Password;
            existing.JobTitle = user.JobTitle;
            _context.SaveChanges();

            // Log user update
            LogAction("User updated", id);

            return Ok(existing);
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                _logger.LogWarning("User {Id} not found during deletion", id);
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            // Log user deletion
            LogAction("User deleted", id);

            return Ok();
        }
    }
}
