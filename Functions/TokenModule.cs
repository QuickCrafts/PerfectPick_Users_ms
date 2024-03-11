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

namespace _PerfectPickUsers_MS.Functions
{
    public class TokenModule
    {
        public readonly string _SecretLoginKey;
        public readonly string _SecretTempKey;
        public readonly AESModule _AESModule;

        public TokenModule()
        {
            try
            {
                _SecretLoginKey = Environment.GetEnvironmentVariable("secretKey") ?? throw new Exception("Secret key not found in environment");
                _SecretTempKey = Environment.GetEnvironmentVariable("secretTempTokenKey") ?? throw new Exception("Temp key not found in environment");
                _AESModule = new AESModule();
            } catch (Exception e)
            {
                throw new Exception("Error while setting up TokenModule: " + e.Message);
            }
        }

        public string GenerateToken(int userID, bool login,bool? isAdmin)
        {
            try
            {
                string key = login ? _SecretLoginKey : _SecretTempKey;
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var claims = new ClaimsIdentity();
                string ID = Convert.ToString(userID);
                string Encrypted = _AESModule.EncryptString(ID);
                claims.AddClaim(new Claim(ClaimTypes.SerialNumber, Encrypted));
                if (isAdmin.HasValue)
                {
                    claims.AddClaim(new Claim(ClaimTypes.Role, isAdmin.Value ? "Admin" : "User"));
                }




                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddHours(4),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string createdToken = tokenHandler.WriteToken(tokenConfig);

                return createdToken;
            }
            catch (Exception e)
            {
                throw new Exception("Error while generating token: " + e.Message);
            }
        }

        public int? ValidateToken(string token, bool login)
        {
            try
            {
                string key = login ? _SecretLoginKey : _SecretTempKey;
                var keyBytes = Encoding.UTF8.GetBytes(key);
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
                var userPotentialID = principal.FindFirst(ClaimTypes.SerialNumber);
                if (userPotentialID != null)
                {
                    string encryptedID = userPotentialID.Value;
                    int decrypted = _AESModule.DecryptString(encryptedID);
                    return Convert.ToInt16(decrypted);
                }

                return null;

            }
            catch (Exception e)
            {
                throw new Exception("Error while validating token: " + e.Message);
            }
        }
    }
}
