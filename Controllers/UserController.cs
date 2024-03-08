using _PerfectPickUsers_MS.Models.User;
using _PerfectPickUsers_MS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _PerfectPickUsers_MS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController()
        {
            _userService = new UserService();
        }

        [HttpGet]
        public IActionResult GetUser([FromQuery] int? userID)
        {
            if (userID.HasValue)
            {
                try
                {
                    if (_userService.UserIDExists(userID.Value))
                    {
                        return Ok(_userService.GetUser(userID.Value));
                    }
                    else
                    {
                        return NotFound("User not found");
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }

            }
            else
            {
                try
                {
                    return Ok(_userService.GetAllUsers());
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }

        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            try
            {
                if (_userService.UserEmailExists(user.Email))
                {
                    return BadRequest("User already exists");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            try
            {
                var result = _userService.CreateUser(user);
                return Ok("User created sucessfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] UserDTO user, [FromQuery] int userID)
        {
            if (user.FirstName == null && user.LastName == null)
            {
                return BadRequest("No fields sent to update");
            }

            if (!_userService.UserIDExists(userID))
            {
                return NotFound("User not found");
            }

            try
            {
                _userService.UpdateUser(user, userID);
                return Ok("User updated successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}
