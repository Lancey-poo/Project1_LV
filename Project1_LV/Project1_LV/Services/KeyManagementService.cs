using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using Project1_LV.Models;

namespace Project1_LV.Services
{
    public class KeyManagementService
    {
        private readonly ConcurrentDictionary<string, RsaKey> _keys = new ConcurrentDictionary<string, RsaKey>();

        public KeyManagementService()
        {
            // Generate an initial key
            GenerateNewKey();
        }

        public RsaKey GenerateNewKey()
        {
            using (var rsa = RSA.Create(2048))
            {
                var key = new RsaKey
                {
                    Kid = Guid.NewGuid().ToString(),
                    Parameters = rsa.ExportParameters(true),
                    Expiry = DateTime.UtcNow.AddHours(1) // Key expires in 1 hour
                };

                _keys[key.Kid] = key;
                return key;
            }
        }

        public IEnumerable<RsaKey> GetValidKeys()
        {
            var now = DateTime.UtcNow;
            foreach (var key in _keys.Values)
            {
                if (key.Expiry > now)
                {
                    yield return key;
                }
            }
        }

        public RsaKey GetKeyById(string kid)
        {
            _keys.TryGetValue(kid, out var key);
            return key;
        }

        public void RemoveExpiredKeys()
        {
            var now = DateTime.UtcNow;
            foreach (var key in _keys.Values)
            {
                if (key.Expiry <= now)
                {
                    _keys.TryRemove(key.Kid, out _);
                }
            }
        }

        public RsaKey GetExpiredKey()
        {
            // For the purpose of issuing JWTs with expired keys
            foreach (var key in _keys.Values)
            {
                if (key.Expiry <= DateTime.UtcNow)
                {
                    return key;
                }
            }

            // Generate a new expired key if none exist
            using (var rsa = RSA.Create(2048))
            {
                var key = new RsaKey
                {
                    Kid = Guid.NewGuid().ToString(),
                    Parameters = rsa.ExportParameters(true),
                    Expiry = DateTime.UtcNow.AddHours(-1) // Expired 1 hour ago
                };

                _keys[key.Kid] = key;
                return key;
            }
        }
    }
}
