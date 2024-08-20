using System;
using System.Security.Cryptography;
using System.Text;
using Raven35.Abstractions.Data;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Abstractions;
using Raven35.Json.Linq;

namespace Raven35.Database.Server.Security.OAuth
{
    using Raven35.Abstractions.Util.Encryptors;

    public class AccessToken
    {
        public string Body { get; set; }
        public string Signature { get; set; }

        private bool MatchesSignature(byte[] key)
        {
            var signatureData = Convert.FromBase64String(Signature);

            using (var rsa = Encryptor.Current.CreateAsymmetrical())
            {
                rsa.ImportCspBlob(key);

                var bodyData = Encoding.Unicode.GetBytes(Body);

                return rsa.VerifyHash(Encryptor.Current.Hash.ComputeForOAuth(bodyData), CryptoConfig.MapNameToOID("SHA1"), signatureData);
            }
        }

        public static bool TryParseBody(byte[] key, string token, out AccessTokenBody body)
        {
            AccessToken accessToken;
            if (TryParse(token, out accessToken) == false)
            {
                body = null;
                return false;
            }

            if (accessToken.MatchesSignature(key) == false)
            {
                body = null;
                return false;
            }

            try
            {
                body = JsonConvert.DeserializeObject<AccessTokenBody>(accessToken.Body);
                return true;
            }
            catch
            {
                body = null;
                return false;
            }
        }

        private static bool TryParse(string token, out AccessToken accessToken)
        {
            try
            {
                accessToken = JsonConvert.DeserializeObject<AccessToken>(token);
                return true;
            }
            catch
            {
                accessToken = null;
                return false;
            }
        }

        public static AccessToken Create(byte[] key, AccessTokenBody tokenBody)
        {
            tokenBody.Issued = (SystemTime.UtcNow - DateTime.MinValue).TotalMilliseconds;

            var body = RavenJObject.FromObject(tokenBody)
                    .ToString(Formatting.None);

            var signature = Sign(body, key);

            return new AccessToken { Body = body, Signature = signature };
        }

        public static string Sign(string body, byte[] key)
        {
            var data = Encoding.Unicode.GetBytes(body);
            using (var rsa = Encryptor.Current.CreateAsymmetrical())
            {
                var hash = Encryptor.Current.Hash.ComputeForOAuth(data);

                rsa.ImportCspBlob(key);

                return Convert.ToBase64String(rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")));
            }
        }

        public string Serialize()
        {
            return RavenJObject.FromObject(this).ToString(Formatting.None);
        }

    }
}
