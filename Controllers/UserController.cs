﻿using _PerfectPickUsers_MS.Models.User;
using _PerfectPickUsers_MS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _PerfectPickUsers_MS.Controllers
{
    [Authorize]
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
        public IActionResult GetUsers([FromQuery]string? email)
        {
            if (email != null)
            {
                try
                {
                    if(_userService.UserExists(email))
                    {
                        return Ok(_userService.GetUser(email));
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
                
            } else
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
        public IActionResult CreateUser([FromBody] UserModel user)
        {
            if (_userService.UserExists(user.Email))
            {
                return BadRequest("User already exists");
            }
            try
            {
                var result = _userService.CreateUser(user);
                if (result)
                {
                    return Ok("User created sucessfully!");
                }
                else
                {
                    return BadRequest("User already exists");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] UserDTO user)
        {
            if(user.FirstName == null && user.LastName==null)
            {
                return BadRequest("No fields sent to update");
            }

            if (!_userService.UserExists(user.Email))
            {
                return NotFound("User not found");
            }

            try
            {
                _userService.UpdateUser(user);
                return Ok("User updated successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}
