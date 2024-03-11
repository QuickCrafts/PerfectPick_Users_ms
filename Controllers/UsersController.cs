using _PerfectPickUsers_MS.DB;
using _PerfectPickUsers_MS.Exceptions;
using _PerfectPickUsers_MS.Models.Login;
using _PerfectPickUsers_MS.Models.User;
using _PerfectPickUsers_MS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using _PerfectPickUsers_MS.Functions;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Http.HttpResults;

namespace _PerfectPickUsers_MS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly UserService _userService;
        private readonly TokenModule _TokenModule;

        public UsersController()
        {
            _userService = new UserService();
            _TokenModule = new TokenModule();
        }

        [HttpPost]
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
                var userID = _userService.GetUserIDFromEmail(user.Email);
                object response = new { id = userID };
                return new CreatedResult("User created successfully", response);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        [Route("GoogleLogin")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = "api/User/signin-google" };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [Route("signin-google")]
        public async Task<IActionResult> GoogleAuth()
        {
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (response.Principal == null)
            {
                return BadRequest("Google authentication failed");
            }

            string email = response.Principal.FindFirst(ClaimTypes.Email).Value;

            if (_userService.UserEmailExists(email))
            {
                int userID = _userService.GetUserIDFromEmail(email);
                if (_userService.UserIsAdmin(userID))
                {
                    return BadRequest("Admins must use standard login!");
                }
                return Ok(_TokenModule.GenerateToken(userID, true, false));
            }

            string givenName = response.Principal.FindFirst(ClaimTypes.GivenName).Value;
            string surName = "";
            var trying = response.Principal.FindFirst(ClaimTypes.Surname);
            if (trying != null)
            {
                surName = trying.Value;
            }



            UserModel user = new UserModel
            {
                Email = email,
                FirstName = givenName,
                LastName = surName,
                Password = Environment.GetEnvironmentVariable("SafeGooglePassword"),
                Birthdate = ""
            };

            try
            {
                _userService.CreateUser(user);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            var userid = _userService.GetUserIDFromEmail(user.Email);
            object responseID = new { id = userid };
            return new CreatedResult("User created successfully", responseID);
        }

        [HttpPost]
        [Route("verify")]
        public IActionResult Verify([FromQuery] string userToken)
        {
            try
            {
                _userService.VerifyUser(userToken);
                return Ok("User verified successfully");
            }
            catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromBody] UserDTO user, int id)
        {

            if (!_userService.UserIDExists(id))
            {
                return NotFound("User not found");
            }

            try
            {
                _userService.UpdateUser(user, id);
                return StatusCode(201, "User updated successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        [Route("setup")]
        public IActionResult Setup([FromQuery]int id)
        {
            try
            {
                if (!_userService.UserIDExists(id))
                {
                    return NotFound("User not found");
                }
                _userService.SetupUser(id);
                return Ok("User setup successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        [Route("setup")]
        public IActionResult VerifyToken([FromQuery] string token)
        {
            int? userID = _TokenModule.ValidateToken(token, true);
            if (!userID.HasValue)
            {
                return Unauthorized("Invalid token.");
            }

            if (!_userService.UserIDExists(userID.Value))
            {
                return Unauthorized("Invalid token.");
            }

            var user = _userService.GetUser(Convert.ToInt16(userID));
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok("Valid token.");



        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginModel login)
        {

            try
            {
                if (!_userService.UserEmailExists(login.Email))
                {
                    return NotFound("User doesn't exist");
                }

                var token = _userService.Login(login);
                return Ok(token);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (!_userService.UserIDExists(id))
            {
                return NotFound("User not found");
            }

            try
            {
                _userService.DeleteUser(id);
                return StatusCode(204);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
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
        [Route("ChangePassword")]
        public IActionResult ChangePassword([FromQuery] int userID, [FromQuery] string currentPassword, [FromQuery] string newPassword)
        {
            try
            {
                if (!_userService.UserIDExists(userID))
                {
                    return NotFound("User not found");
                }

                _userService.ChangePassword(userID, currentPassword, newPassword);
                return Ok("Password changed successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }













    }
}
