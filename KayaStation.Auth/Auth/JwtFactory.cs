//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Security.Principal;
//using System.Threading.Tasks;

//namespace KayaStation.Auth
//{
//    public interface IJwtFactory
//    {
//        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
//        ClaimsIdentity GenerateClaimsIdentity(string userName, string id);
//    }

//    public class JwtFactory : IJwtFactory
//    {
//        private readonly JwtIssuerOptions _jwtOptions;

//        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
//        {
//            _jwtOptions = jwtOptions.Value;
//            ThrowIfInvalidOptions(_jwtOptions);
//        }

//        public async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity)
//        {
//            var claims = new[]
//         {
//                 new Claim(JwtRegisteredClaimNames.Sub, userName),
//                 new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
//                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
//                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Rol),
//                 identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id)
//             };

//            // Create the JWT security token and encode it.
//            var jwt = new JwtSecurityToken(
//                issuer: _jwtOptions.Issuer,
//                audience: _jwtOptions.Audience,
//                claims: claims,
//                notBefore: _jwtOptions.NotBefore,
//                expires: _jwtOptions.Expiration,
//                signingCredentials: _jwtOptions.SigningCredentials);

//            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

//            return encodedJwt;
//        }

//        public ClaimsIdentity GenerateClaimsIdentity(string userName, string id)
//        {
//            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
//            {
//                new Claim(Constants.Strings.JwtClaimIdentifiers.Id, id),
//                new Claim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess)
//            });
//        }

//        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
//        private static long ToUnixEpochDate(DateTime date)
//          => (long)Math.Round((date.ToUniversalTime() -
//                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
//                              .TotalSeconds);

//        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
//        {
//            if (options == null) throw new ArgumentNullException(nameof(options));

//            if (options.ValidFor <= TimeSpan.Zero)
//            {
//                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
//            }

//            if (options.SigningCredentials == null)
//            {
//                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
//            }

//            if (options.JtiGenerator == null)
//            {
//                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
//            }
//        }
//    }

//    public class JwtIssuerOptions
//    {
//        /// <summary>
//        /// 4.1.1.  "iss" (Issuer) Claim - The "iss" (issuer) claim identifies the principal that issued the JWT.
//        /// </summary>
//        public string Issuer { get; set; }

//        /// <summary>
//        /// 4.1.2.  "sub" (Subject) Claim - The "sub" (subject) claim identifies the principal that is the subject of the JWT.
//        /// </summary>
//        public string Subject { get; set; }

//        /// <summary>
//        /// 4.1.3.  "aud" (Audience) Claim - The "aud" (audience) claim identifies the recipients that the JWT is intended for.
//        /// </summary>
//        public string Audience { get; set; }

//        /// <summary>
//        /// 4.1.4.  "exp" (Expiration Time) Claim - The "exp" (expiration time) claim identifies the expiration time on or after which the JWT MUST NOT be accepted for processing.
//        /// </summary>
//        public DateTime Expiration => IssuedAt.Add(ValidFor);

//        /// <summary>
//        /// 4.1.5.  "nbf" (Not Before) Claim - The "nbf" (not before) claim identifies the time before which the JWT MUST NOT be accepted for processing.
//        /// </summary>
//        public DateTime NotBefore { get; set; } = DateTime.UtcNow;

//        /// <summary>
//        /// 4.1.6.  "iat" (Issued At) Claim - The "iat" (issued at) claim identifies the time at which the JWT was issued.
//        /// </summary>
//        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

//        /// <summary>
//        /// Set the timespan the token will be valid for (default is 120 min)
//        /// </summary>
//        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(120);



//        /// <summary>
//        /// "jti" (JWT ID) Claim (default ID is a GUID)
//        /// </summary>
//        public Func<Task<string>> JtiGenerator =>
//          () => Task.FromResult(Guid.NewGuid().ToString());

//        /// <summary>
//        /// The signing key to use when generating tokens.
//        /// </summary>
//        public SigningCredentials SigningCredentials { get; set; }
//    }

//    public static class Constants
//    {
//        public static class Strings
//        {
//            public static class JwtClaimIdentifiers
//            {
//                public const string Rol = "rol", Id = "id";
//            }

//            public static class JwtClaims
//            {
//                public const string ApiAccess = "api_access";
//            }
//        }
//    }

//}
