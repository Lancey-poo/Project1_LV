using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Project1_LV.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Project1_LV.Controllers
{
    [ApiController]
    [Route(".well-known/jwks.json")]
    public class JwksController : ControllerBase
    {
        private readonly KeyManagementService _keyService;

        public JwksController(KeyManagementService keyService)
        {
            _keyService = keyService;
        }

        [HttpGet]
        public IActionResult GetJwks()
        {
            var keys = _keyService.GetValidKeys().Select(key => new JsonWebKey
            {
                Kty = "RSA",
                Use = "sig",
                Kid = key.Kid,
                E = Base64UrlEncoder.Encode(key.Parameters.Exponent),
                N = Base64UrlEncoder.Encode(key.Parameters.Modulus),
            });

            var jwks = new
            {
                keys = keys.ToArray()
            };

            return Ok(jwks);
        }
    }
}
