using _4UUsers.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using _4UUsers.Models.User;
using System.Text;

namespace _4UUsers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly BCryptEncryptor _Encryptor;
        private readonly string _SecretKey;

        public AuthController(IConfiguration config)
        {
            try
            {
                _Encryptor = new BCryptEncryptor();
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating Encryptor: " + e.Message);
            }

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

        }

        [HttpGet]
        public IActionResult Getter([FromQuery] string pass)
        {
            try
            {
                return Ok(_Encryptor.Encrypt(pass));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }

        [HttpPost]
        public IActionResult GenerateToken([FromBody] UserModel user)
        {
            const bool isRegistered = true;

            if (isRegistered)
            {
                var keyBytes = Encoding.UTF8.GetBytes(_SecretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
                claims.AddClaim(new Claim(ClaimTypes.Email, user.Email));


                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string createdToken = tokenHandler.WriteToken(tokenConfig);



                return Ok(new {token = createdToken});
            }

        }
    }
}
