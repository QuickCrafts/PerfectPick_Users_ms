﻿
using _PerfectPickUsers_MS.Exceptions;
using _PerfectPickUsers_MS.Models.Login;
using _PerfectPickUsers_MS.Models.User;
using _PerfectPickUsers_MS.Models.Contact;
using _PerfectPickUsers_MS.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using _PerfectPickUsers_MS.Functions;


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
                    return new BadRequestObjectResult(new {Message = "User already exists" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500,new { Message = e.Message });
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
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("GoogleLogin")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = "Users/signin-google" };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [Route("signin-google")]
        public async Task<IActionResult> GoogleAuth()
        {
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (response.Principal == null)
            {
                return new BadRequestObjectResult(new { Message = "Google authentication failed" });
            }

            string email = response.Principal.FindFirst(ClaimTypes.Email).Value;

            if (_userService.UserEmailExists(email))
            {
                int userID = _userService.GetUserIDFromEmail(email);
                if (_userService.UserIsAdmin(userID))
                {
                    return BadRequest("Admins must use standard login!");
                }
                return Ok(_TokenModule.GenerateToken(userID, true));
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
                return Ok(new { Message = "User verified successfully" });
            }
            catch (UserNotFoundException e)
            {
                return NotFound(new {Message = "User not found"});
            }
            catch (Exception e)
            {
                return StatusCode(500,new { Message = e.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromBody] UserDTO user, int id)
        {

            if (!_userService.UserIDExists(id))
            {
                return new NotFoundObjectResult(new { Message = "User not found" });
            }

            try
            {
                _userService.UpdateUser(user, id);
                return StatusCode(201, new { Message = "User updated successfully" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
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
        [Route("verify/{token}")]
        public IActionResult VerifyToken(string token)
        {
            try
            {
                int? userID = _TokenModule.ValidateToken(token, true);
                if (!userID.HasValue)
                {
                    return new UnauthorizedObjectResult(new { Message = "Unauthorized" });
                }

                if (!_userService.UserIDExists(userID.Value))
                {
                    return new UnauthorizedObjectResult(new { Message = "Unauthorized" });
                }

                bool isAdmin = _userService.UserIsAdmin(userID.Value);

                var user = _userService.GetUser(Convert.ToInt16(userID));
                if (user == null)
                {
                    return new UnauthorizedObjectResult(new { Message = "Unauthorized" });
                }
                return new OkObjectResult(new { Message = "Valid Token", ID = userID , Admin = isAdmin});
            } catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
            



        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginModel login)
        {

            try
            {
                if (!_userService.UserEmailExists(login.Email))
                {
                    return new NotFoundObjectResult( new { Message = "User doesn't exist"});
                }

                var token = _userService.Login(login);
                return new OkObjectResult(new { Token = token });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (!_userService.UserIDExists(id))
            {
                return new NotFoundObjectResult(new { Message = "User not found" });
            }

            try
            {
                _userService.DeleteUser(id);
                return StatusCode(204, new { Message = "User removed" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message =  e.Message });
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
                        return new OkObjectResult(_userService.GetUser(userID.Value));
                    }
                    else
                    {
                        return new NotFoundObjectResult( new { Message = "User not found" });
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { Message = e.Message });
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
                    return StatusCode(500, new { Message = e.Message });
                }
            }

        }

        [HttpGet]
        [Route("Email/{email}")]
        public IActionResult GetEmailUser(string email)
        {
            try
            {
                if (_userService.UserEmailExists(email))
                {
                    int userID = _userService.GetUserIDFromEmail(email);
                    return Ok(_userService.GetUser(userID));
                }
                else
                {
                    return new NotFoundObjectResult(new { Message = "User not found" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }


        [HttpPost]
        [Route("auth/forgot/{email}")]
        public IActionResult ForgotPassword(string email)
        {
            try
            {
                if (!_userService.UserEmailExists(email))
                {
                    return new NotFoundObjectResult(new { Message = "User not found" });
                }
                _userService.RequestPasswordReset(email);
                return new OkObjectResult(new { Message = "Password reset email sent" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpPost]
        [Route("auth/recover")]
        public IActionResult RecoverPassword([FromQuery] string verificationToken, [FromBody] PasswordResetModel PasswordChangeData)
        {
            try
            {
                int? userID = _TokenModule.ValidateToken(verificationToken, false);
                if (!userID.HasValue)
                {
                    return new UnauthorizedObjectResult(new { Message = "Unauthorized" });
                }
                string newPassword = PasswordChangeData.NewPassword;
                _userService.SetNewPassword(userID.Value, newPassword);
                return new OkObjectResult(new { Message = "Password changed successfully" });

            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpPost]
        [Route("auth/change")]
        public IActionResult ChangePassword([FromBody] PasswordChangeModel PasswordChangeData)
        {
            try
            {
                if(!_userService.UserEmailExists(PasswordChangeData.Email))
                {
                    return new NotFoundObjectResult(new { Message = "User not found" });
                }   
                int userID = _userService.GetUserIDFromEmail(PasswordChangeData.Email);
                string currentPassword = PasswordChangeData.OldPassword;
                string newPassword = PasswordChangeData.NewPassword;
                _userService.ChangePassword(userID, currentPassword, newPassword);
                return new OkObjectResult(new { Message = "Password changed successfully" });

            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpGet]
        [Route("verify/role/{id}")]
        public IActionResult VerifyRole(int id)
        {
            try
            {
                if (!_userService.UserIDExists(id))
                {
                    return new NotFoundObjectResult(new { Message = "User not found" });
                }

                if (_userService.UserIsAdmin(id))
                {
                    return new OkObjectResult(new { isAdmin = true });
                }
                else
                {
                    return new OkObjectResult(new { isAdmin = false });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpPost]
        [Route("contact")]
        public IActionResult Contact([FromBody] ContactModel contact)
        {
            try
            {
                _userService.Contact(contact);
                return new OkObjectResult(new { Message = "Message sent" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }
    }

}
