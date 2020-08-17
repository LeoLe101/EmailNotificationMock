//using Microsoft.AspNetCore.Http;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;

//namespace EmailServer
//{
//    /// <summary>
//    /// Auth methods for working with Authentication
//    /// </summary>
//    public static class AuthUtil
//    {
//        /// <summary>
//        /// Generates a Jwt bearer token containing the users username
//        /// </summary>
//        /// <param name="user">The users details</param>
//        /// <returns></returns>
//        public static string GenerateJwtToken(this DbUser user, IList<string> roles)
//        {
//            // Set our tokens claims
//            List<Claim> claims = new List<Claim>
//            {
//                // Add user Id so that UserManager.GetUserAsync can find the user based on Id
//                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),

//                // Add user email confirmation status. Unconfirmed users can read but cant post
//                new Claim(Constants.JwtConfirmed, user.EmailConfirmed ? Constants.StrY : Constants.StrN),

//                // Add company id for AuthZ
//                new Claim(JwtRegisteredClaimNames.Sub, user.CompanyId.ToString()),

//                // Unique ID for this token
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),

//                // The issue date of the token
//                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
//            };

//            // Using claims identity to group claims (Allows the use of Array vs List above and
//            // add the roles below using claimsIdentity.AddClaims())
//            // ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");

//            // Add roles for AuthZ
//            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

//            // Create the credentials used to generate the token
//            SigningCredentials credentials = new SigningCredentials(
//                // Get the secret key from configuration
//                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SharedGlobals.Configuration["Jwt:SecretKey"])),
//                // Use HS256 algorithm
//                SecurityAlgorithms.HmacSha256
//            );

//            DateTime now = DateTime.UtcNow;

//            // Generate the Jwt Token
//            JwtSecurityToken token = new JwtSecurityToken(
//                claims: claims,
//                signingCredentials: credentials,
//                issuer: SharedGlobals.Configuration["Jwt:Issuer"],
//                audience: SharedGlobals.Configuration["Jwt:Audience"],
//                // Token cant be used before its generated. More useful if
//                // it has to be generated now, but needs to be locked down
//                // until sometime in the future
//                // notBefore: now,
//                // Expire if not used for 1 hour
//                expires: now.AddHours(1)
//            );

//            // Return the generated token
//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }

//        public static JwtProps DecodeExpiredToken(IHttpContextAccessor accessor)
//        {
//            // Try to extract the authorization header
//            string authorization = accessor?.HttpContext?.Request?.Headers[Constants.StrAuthorization].FirstOrDefault();

//            // If no header is found, error out
//            if (String.IsNullOrWhiteSpace(authorization))
//            {
//                throw new ApiUnauthorizedException($"Auth_RefreshToken: {Constants.ErrorUnauthorized}:", "no_authorization");
//            }

//            if (!authorization.StartsWith(Constants.StrBearer, StringComparison.OrdinalIgnoreCase))
//            {
//                throw new ApiUnauthorizedException($"Auth_RefreshToken: {Constants.ErrorUnauthorized}:", "invalid_authorization");
//            }

//            string accessToken = authorization.Substring(Constants.StrBearer.Length).Trim();

//            if (String.IsNullOrWhiteSpace(accessToken))
//            {
//                throw new ApiUnauthorizedException($"Auth_RefreshToken: {Constants.ErrorUnauthorized}:", "no_access_token");
//            }

//            // Init new validation parameters without expiration check
//            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
//            {
//                // Validate expiration (Change from OG params)
//                ValidateLifetime = false,

//                // Validate issuer
//                ValidateIssuer = true,
//                // Set issuer
//                ValidIssuer = SharedGlobals.Configuration["Jwt:Issuer"],

//                // Validate audience
//                ValidateAudience = true,
//                // Set audience
//                ValidAudience = SharedGlobals.Configuration["Jwt:Audience"],

//                // Validate signature
//                ValidateIssuerSigningKey = true,
//                // Set signing key
//                IssuerSigningKey = new SymmetricSecurityKey(
//                    // Get our secret key from configuration
//                    Encoding.UTF8.GetBytes(SharedGlobals.Configuration["Jwt:SecretKey"])
//                ),
//            };

//            // Validate token with our params minus the exiration. We still want to make sure the token is ours
//            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
//            ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
//            JwtSecurityToken jwtToken = securityToken as JwtSecurityToken;
//            // Ensure the token's algo is correct
//            if ((jwtToken == null) || (!jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)))
//            {
//                throw new ApiUnauthorizedException($"Auth_RefreshToken: {Constants.ErrorUnauthorized}:", "invalid_access_token");
//            }

//            // Extract email validation status
//            var isValidBool = Boolean.TryParse(principal?.FindFirstValue(Constants.JwtConfirmed).Trim(), out bool isConfirmed);
//            // Check for bool validity
//            if (!isValidBool)
//            {
//                isConfirmed = false;
//            }

//            // Return the generated token
//            return new JwtProps()
//            {
//                role = principal?.FindFirstValue(ClaimTypes.Role),
//                companyId = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub).Trim(),
//                userId = principal?.FindFirstValue(ClaimsIdentity.DefaultNameClaimType).Trim(),
//                isConfirmed = isConfirmed,
//            };
//        }


//        public static bool IsGlobalAdmin(IHttpContextAccessor accessor)
//        {
//            return accessor?.HttpContext?.User?.IsInRole(UserType.GlobalAdmin.ToValue()) ?? false;
//        }

//        public static bool IsCompanyAdmin(IHttpContextAccessor accessor)
//        {
//            return accessor?.HttpContext?.User?.IsInRole(UserType.CompanyAdmin.ToValue()) ?? false;
//        }

//        public static bool IsAnyAdmin(IHttpContextAccessor accessor)
//        {
//            return (AuthUtil.IsGlobalAdmin(accessor) || AuthUtil.IsCompanyAdmin(accessor));
//        }

//        public static string GetUserId(IHttpContextAccessor accessor)
//        {
//            return accessor?.HttpContext?.User?.Identity?.Name?.Trim();
//        }

//        public static string GetCompanyId(IHttpContextAccessor accessor)
//        {
//            return accessor?.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub)?.Trim();
//        }
//    }
//}
