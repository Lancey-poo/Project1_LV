using System;
using System.Security.Cryptography;

namespace Project1_LV.Models
{
    public class RsaKey
    {
        public string Kid { get; set; }
        public RSAParameters Parameters { get; set; }
        public DateTime Expiry { get; set; }
    }
}
