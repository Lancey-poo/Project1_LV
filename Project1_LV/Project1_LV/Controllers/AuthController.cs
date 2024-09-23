using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Project1_LV.Services;
using Project1_LV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Project1_LV.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly KeyManagementService _keyService;

        public AuthController(KeyManagementService keyService)
        {
            _keyService = keyService;
        }

        [HttpPost]
        public IActionResult Authenticate([FromQuery] bool expired = false)
        {
            RsaKey key;
            DateTime expiry;

            if (expired)
            {
                key = _keyService.GetExpiredKey();
                expiry = DateTime.UtcNow.AddHours(-1); // Token expired 1 hour ago
            }
            else
            {
                key = _keyService.GetValidKeys().FirstOrDefault();
                if (key == null)
                {
                    key = _keyService.GenerateNewKey();
                }
                expiry = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour
            }

            var token = GenerateToken(key, expiry);

            return Ok(new { token });
        }

        private string GenerateToken(RsaKey key, DateTime expiry)
        {
            var handler = new JwtSecurityTokenHandler();
            var rsa = RSA.Create();
            rsa.ImportParameters(key.Parameters);

            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa)
                {
                    KeyId = key.Kid
                },
                SecurityAlgorithms.RsaSha256
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = expiry,
                SigningCredentials = signingCredentials,
                Claims = new Dictionary<string, object>
                {
                    { "sub", "1234567890" }, // Subject identifier
                    { "name", "John Doe" },
                    { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
                }
            };

            var securityToken = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(securityToken);
        }
    }
}
