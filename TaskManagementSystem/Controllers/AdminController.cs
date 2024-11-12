using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpPost("assign-admin")]
        public async Task<IActionResult> AssignAdminRole([FromBody] AssignAdminRequest model)
        {
            var success = await _userService.AssignAdminRoleAsync(model.UserEmail);
            if (!success)
                return BadRequest(new { message = "User not found or already an admin." });

            return Ok(new { message = "User has been assigned the Admin role." });
        }

        [HttpPost("remove-admin")]
        public async Task<IActionResult> RemoveAdminRole([FromBody] AssignAdminRequest model)
        {
            var success = await _userService.RemoveAdminRoleAsync(model.UserEmail);
            if (!success)
                return BadRequest(new { message = "User not found or is not an admin." });

            return Ok(new { message = "Admin role has been removed from the user." });
        }

        [HttpDelete("remove-user")]
        public async Task<IActionResult> RemoveUser([FromBody] AssignAdminRequest model)
        {
            var success = await _userService.DeleteUserAsync(model.UserEmail);
            if (!success)
                return BadRequest(new { message = "User not found or failed to delete user." });

            return Ok(new { message = "User has been removed from the system." });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }
    }
    public class AssignAdminRequest
    {
        public string UserEmail { get; set; }
    }
}
