using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Services;
using Xunit;

namespace WebApplication1.Tests.Services
{
    public class TokenServiceTests
    {
        [Fact]
        public void GenerateToken_ValidConfig_ReturnsTokenWithClaims()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "JwtSettings:SecretKey", "test_super_secret_key_which_is_long_enough" },
                { "JwtSettings:Issuer", "test-issuer" },
                { "JwtSettings:Audience", "test-audience" },
                { "JwtSettings:ExpiryMinutes", "60" }
            };

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

            var svc = new TokenService(configuration, new Mock<ILogger<TokenService>>().Object);

            var token = svc.GenerateToken(123, "user@example.com", "First", "Last", "Admin");

            Assert.False(string.IsNullOrWhiteSpace(token));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
            var subClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var givenName = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value;
            var familyName = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName)?.Value;
            var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            Assert.Equal("user@example.com", emailClaim);
            Assert.Equal("123", subClaim);
            Assert.Equal("First", givenName);
            Assert.Equal("Last", familyName);
            Assert.Equal("Admin", roleClaim);
        }

        [Fact]
        public void GenerateToken_MissingSecret_ThrowsInvalidOperationException()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                // SecretKey intentionally omitted
                { "JwtSettings:Issuer", "test-issuer" },
                { "JwtSettings:Audience", "test-audience" },
                { "JwtSettings:ExpiryMinutes", "60" }
            };

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

            var svc = new TokenService(configuration, new Mock<ILogger<TokenService>>().Object);

            Assert.Throws<InvalidOperationException>(() => svc.GenerateToken(1, "a@b", "F", "L", "role"));
        }

        [Fact]
        public void GenerateToken_UsesExpiryMinutes_FromConfig()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "JwtSettings:SecretKey", "another_super_secret_key_1234567890" },
                { "JwtSettings:Issuer", "iss" },
                { "JwtSettings:Audience", "aud" },
                { "JwtSettings:ExpiryMinutes", "2" }
            };

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            var svc = new TokenService(configuration, new Mock<ILogger<TokenService>>().Object);

            var token = svc.GenerateToken(5, "e@x.com", "G", "H", "User");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var minutes = (jwt.ValidTo - DateTime.UtcNow).TotalMinutes;
            // Allow some leeway for test execution time
            Assert.InRange(minutes, 1.0, 3.0);
        }
    }
}
