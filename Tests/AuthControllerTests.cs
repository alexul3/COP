using Controllers;
using System.Net.Http.Json;
using Tests;
using Xunit;

namespace COP.Tests.Controllers
{
    public class AuthControllerTests : IClassFixture<TestApplication>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(TestApplication factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsUserInfo()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.EnsureSuccessStatusCode();

            var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();
            Assert.NotNull(userInfo);
            Assert.Equal("testuser", userInfo.Username);
            Assert.Equal("Student", userInfo.Role);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "wronguser",
                Password = "wrongpass"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithNewUser_ReturnsUserInfo()
        {
            // Arrange
            var registerRequest = new LoginRequest
            {
                Username = "newuser",
                Password = "newpass123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

            // Assert
            response.EnsureSuccessStatusCode();

            var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();
            Assert.NotNull(userInfo);
            Assert.Equal("newuser", userInfo.Username);
        }

        [Fact]
        public async Task Register_WithExistingUser_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new LoginRequest
            {
                Username = "testuser", // Уже существует
                Password = "anypassword"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}