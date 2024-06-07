using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using robot_controller_api.Models;
using robot_controller_api.Persistence;
using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace robot_controller_api.Authentication
{
    public class EncodeSHA
    {
        private readonly SHA256 _sha256;

        public EncodeSHA()
        {
            _sha256 = SHA256.Create();
        }

        // Hashes a password and returns the hashed value as a base64-encoded string
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty.");
            }

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = _sha256.ComputeHash(passwordBytes);

            return Convert.ToBase64String(hashBytes);
        }

        // Verifies a password against a hashed password
        public PasswordVerificationResult VerifyHashedPassword(UserModel user, string hashedPassword, string password)
        {
            if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(password))
            {
                return PasswordVerificationResult.Failed;
            }

            byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] newHashBytes = _sha256.ComputeHash(passwordBytes);

            return PasswordVerificationResult.Success;
        }

    }
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private IUserModelDataAccess _userService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserModelDataAccess userService) : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Authorization header is missing.");
            }

            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata is EndpointMetadataCollection metadata)
            {
                // Access metadata and perform actions based on endpoint attributes
                var allowAnonymous = metadata.GetMetadata<AllowAnonymousAttribute>() != null;
                if (allowAnonymous)
                {
                    // Endpoint allows anonymous access, skip authentication
                    return AuthenticateResult.NoResult();
                }
            }
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
                {
                    return AuthenticateResult.Fail("Invalid Authorization header.");
                }

                var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                var credentials = decodedCredentials.Split(":", StringSplitOptions.RemoveEmptyEntries);

                if (credentials.Length != 2)
                {
                    return AuthenticateResult.Fail("Invalid credentials format.");
                }

                var email = credentials[0];
                var password = credentials[1];

                var user = _userService.GetUserByEmail(email);
                if (user == null)
                {
                    Response.StatusCode = 401;
                    return await Task.FromResult(AuthenticateResult.Fail($"Authentication failed."));
                }

                // Validate the password using a password hasher

                var hasher = new EncodeSHA();
                var pwVerificationResult = hasher.VerifyHashedPassword(user,user.PasswordHash, password);

                if (!string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    var passwordHasher = new PasswordHasher<UserModel>();
                    pwVerificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
                }

                if (pwVerificationResult != PasswordVerificationResult.Success)
                {
                    return AuthenticateResult.Fail("Invalid password.");
                }

                // Authentication successful, create claims
                var claims = new[]
                {
                new Claim("name", $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role)
                // any other claims that you think might be useful
                };
                var identity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                var authTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                return await Task.FromResult(AuthenticateResult.Success(authTicket));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }
        }
    }
}
