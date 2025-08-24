using HandMade_Store_api.DTOs.User;
using HandMade_Store_api.Interfaces;
using HandMade_Store_api.Services;
using HandMade_Store_api.DTOs.User;
using HandMade_Store_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HandMade_Store_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // GET: api/user/search?keyword=xxx
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> Search([FromQuery] string keyword)
        {
            var users = await _userService.SearchUsersAsync(keyword);
            return Ok(users);
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create([FromBody] UserCreateRequest request)
        {
            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UserUpdateRequest request)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, request);
            if (updatedUser == null) return NotFound();
            return Ok(updatedUser);
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "User deleted successfully" });
        }
        [HttpPatch("{id}/change-active")]
        public async Task<ActionResult<UserResponse>> ChangeUserStatus(int id)
        {
            var updatedUser = await _userService.ChangeUserStatusAsync(id);
            if (updatedUser == null) return NotFound();
            return Ok(updatedUser);
        }

    }

}