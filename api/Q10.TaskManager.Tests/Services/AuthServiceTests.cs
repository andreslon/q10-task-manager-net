using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Q10.TaskManager.Application.DTOs.Requests;
using Q10.TaskManager.Application.DTOs.Responses;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Domain.Interfaces;

namespace Q10.TaskManager.Tests.Services
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _authService = new AuthService(_mockUserRepository.Object);
        }

        [TestMethod]
        public async Task RegisterAsync_ValidUser_ReturnsAuthResponse()
        {
            // Arrange
            var userRequest = new UserRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123",
                FirstName = "Test",
                LastName = "User"
            };

            var existingUsers = new List<User>().AsQueryable();
            _mockUserRepository.Setup(r => r.GetAllUsersAsync())
                .ReturnsAsync(existingUsers);

            var createdUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = userRequest.Username,
                Email = userRequest.Email,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Role = "User"
            };

            _mockUserRepository.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _authService.RegisterAsync(userRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Token));
            _mockUserRepository.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
        }

        [TestMethod]
        public async Task RegisterAsync_DuplicateUsername_ThrowsException()
        {
            // Arrange
            var userRequest = new UserRequest
            {
                Username = "existinguser",
                Email = "test@example.com",
                Password = "password123"
            };

            var existingUsers = new List<User>
            {
                new User { Username = "existinguser", Email = "other@example.com" }
            }.AsQueryable();

            _mockUserRepository.Setup(r => r.GetAllUsersAsync())
                .ReturnsAsync(existingUsers);

            // Act & Assert
            try
            {
                await _authService.RegisterAsync(userRequest);
                Assert.Fail("Expected Exception was not thrown");
            }
            catch (Exception)
            {
                // Expected exception
            }
        }

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "User"
            };

            var users = new List<User> { user }.AsQueryable();
            _mockUserRepository.Setup(r => r.GetAllUsersAsync())
                .ReturnsAsync(users);

            _mockUserRepository.Setup(r => r.UpdateUserAsync(It.IsAny<string>(), It.IsAny<User>()))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Token));
        }

        [TestMethod]
        public async Task LoginAsync_InvalidCredentials_ThrowsException()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            var users = new List<User>().AsQueryable();
            _mockUserRepository.Setup(r => r.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act & Assert
            try
            {
                await _authService.LoginAsync(loginRequest);
                Assert.Fail("Expected UnauthorizedAccessException was not thrown");
            }
            catch (UnauthorizedAccessException)
            {
                // Expected exception
            }
        }

        [TestMethod]
        public async Task HashPasswordAsync_ReturnsHashedPassword()
        {
            // Act
            var hash = await _authService.HashPasswordAsync("password123");

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(hash));
            Assert.AreNotEqual("password123", hash);
        }

        [TestMethod]
        public async Task VerifyPasswordAsync_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "password123";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            // Act
            var result = await _authService.VerifyPasswordAsync(password, hash);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task VerifyPasswordAsync_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "password123";
            var hash = BCrypt.Net.BCrypt.HashPassword("differentpassword");

            // Act
            var result = await _authService.VerifyPasswordAsync(password, hash);

            // Assert
            Assert.IsFalse(result);
        }
    }
}

