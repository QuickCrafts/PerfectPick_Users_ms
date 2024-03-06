using _PerfectPickUsers_MS.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using _PerfectPickUsers_MS.Models.User;
using System.Text;
using _PerfectPickUsers_MS.Services;
using Microsoft.AspNetCore.Authorization;

namespace _PerfectPickUsers_MS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly string _SecretKey;
        private readonly UserService _UserAuxiliarService;
        public readonly AESModule _AESModule;

        public AuthController()
        {

            try
            {
                string? tryKey = Environment.GetEnvironmentVariable("secretKey");
                if (string.IsNullOrEmpty(tryKey))
                {
                    throw new Exception("Secret key not found in enviornment");
                }
                _SecretKey = tryKey;
            }
            catch (Exception e)
            {
                throw new Exception("Error while getting secret key: " + e.Message);
            }

            try
            {
                _UserAuxiliarService = new UserService();
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating UserService: " + e.Message);
            }

            try
            {
                _AESModule = new AESModule();
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating AESModule: " + e.Message);
            }

        }

        [HttpPost]
        [Route("GenerateToken")]
        public IActionResult GenerateToken([FromQuery] int userID)
        {
            const bool isRegistered = true;

            if (isRegistered)
            {
                var keyBytes = Encoding.UTF8.GetBytes(_SecretKey);
                var claims = new ClaimsIdentity();
                string ID = Convert.ToString(userID);
                string Encrypted = _AESModule.EncryptString(ID);
                claims.AddClaim(new Claim(ClaimTypes.SerialNumber, Encrypted));


                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string createdToken = tokenHandler.WriteToken(tokenConfig);



                return Ok(new { token = createdToken });
            }

        }

        [HttpPost]
        [Route("VerifyToken")]
        public IActionResult VerifyToken([FromQuery] string token)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_SecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            string userPotentialID = principal.FindFirst(ClaimTypes.SerialNumber).Value;
            int userID = _AESModule.DecryptString(userPotentialID);
            bool exists = _UserAuxiliarService.UserExists(userID);
            if (userPotentialID==null || !exists)
            {
                return Unauthorized();
            }

            var user = _UserAuxiliarService.GetUser(Convert.ToInt16(userID));
            bool isUser = true;
            bool userIsAdmin = user.IsAdmin ? true : false;
            return new OkObjectResult(new { isUser = isUser, isAdmin = userIsAdmin });



        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }
            if (_UserAuxiliarService.UserExists(user.Email))
            {
                return BadRequest("User already exists");
            }

            return Ok("User added successfully");
        }
    }
}
