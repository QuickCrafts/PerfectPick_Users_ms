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
        private readonly string? _SecretKey;
        private readonly UserService _UserAuxiliarService;

        public AuthController(IConfiguration config)
        {

            try
            {
                _SecretKey = config.GetSection("Settings").GetSection("secretKey").ToString();
                if (string.IsNullOrEmpty(_SecretKey))
                {
                    throw new Exception("Secret key not found in enviornment");
                }
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

        }

        [Authorize]
        [HttpPost]
        [Route("GenerateToken")]
        public IActionResult GenerateToken([FromBody] string user_email)
        {
            const bool isRegistered = true;

            if (isRegistered)
            {
                var keyBytes = Encoding.UTF8.GetBytes(_SecretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.Email, user_email));


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
        public IActionResult VerifyToken([FromBody] string token)
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
            string? email = principal.FindFirst(ClaimTypes.Email).Value;
            bool exists = _UserAuxiliarService.UserExists(email);
            if (email == null || !exists)
            {
                return Unauthorized();
            }

            var user = _UserAuxiliarService.GetUser(email);
            bool isUser = true;
            bool userIsAdmin = user.IsAdmin ? true : false;
            return new OkObjectResult(new { isUser = isUser, isAdmin = userIsAdmin });



        }
    }
}
