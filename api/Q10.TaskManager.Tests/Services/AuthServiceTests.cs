using Moq;
using Microsoft.EntityFrameworkCore;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;
using Q10.UserManager.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Q10.TaskManager.Tests.Services
{
    [TestClass]
    public class AuthServiceTests
    {
        public UserRepository UserRepository { get; set; } = null!;
        public IAuthService AuthService { get; set; } = null!;
        private Q10.TaskManager.Infrastructure.Data.PostgreSQLContext _context = null!;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<Q10.TaskManager.Infrastructure.Data.PostgreSQLContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new Q10.TaskManager.Infrastructure.Data.PostgreSQLContext(options);
            UserRepository = new UserRepository(_context);
            AuthService = new AuthService(UserRepository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
        }

        #region GenerateTokenAsync Tests

        [TestMethod]
        public async Task GenerateTokenAsync_ValidInput_ReturnsToken()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var email = "test@example.com";
            var role = "User";

            // Act
            var result = await AuthService.GenerateTokenAsync(userId, email, role);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public async Task GenerateTokenAsync_DifferentInputs_ReturnsDifferentTokens()
        {
            // Arrange
            var userId1 = Guid.NewGuid().ToString();
            var userId2 = Guid.NewGuid().ToString();
            var email = "test@example.com";
            var role = "User";

            // Act
            var token1 = await AuthService.GenerateTokenAsync(userId1, email, role);
            var token2 = await AuthService.GenerateTokenAsync(userId2, email, role);

            // Assert
            Assert.AreNotEqual(token1, token2);
        }

        #endregion

        #region HashPasswordAsync Tests

        [TestMethod]
        public async Task HashPasswordAsync_ValidPassword_ReturnsHash()
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            var result = await AuthService.HashPasswordAsync(password);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.AreNotEqual(password, result);
        }

        [TestMethod]
        public async Task HashPasswordAsync_SamePassword_ReturnsDifferentHashes()
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            var hash1 = await AuthService.HashPasswordAsync(password);
            var hash2 = await AuthService.HashPasswordAsync(password);

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        #endregion

        #region VerifyPasswordAsync Tests

        [TestMethod]
        public async Task VerifyPasswordAsync_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "TestPassword123";
            var hash = await AuthService.HashPasswordAsync(password);

            // Act
            var result = await AuthService.VerifyPasswordAsync(password, hash);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task VerifyPasswordAsync_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "TestPassword123";
            var wrongPassword = "WrongPassword123";
            var hash = await AuthService.HashPasswordAsync(password);

            // Act
            var result = await AuthService.VerifyPasswordAsync(wrongPassword, hash);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task VerifyPasswordAsync_InvalidHash_ReturnsFalse()
        {
            // Arrange
            var password = "TestPassword123";
            var invalidHash = "invalidhash";

            // Act
            var result = await AuthService.VerifyPasswordAsync(password, invalidHash);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region RegisterAsync Tests

        [TestMethod]
        public async Task RegisterAsync_ValidRequest_ReturnsAuthResponse()
        {
            // Arrange
            var request = new UserRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "TestPassword123",
                FirstName = "Test",
                LastName = "User"
            };

            var emptyUsersList = new List<User>().AsQueryable();
            var createdUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = "User"
            };

            // No need to setup - using real repository with in-memory database

            // Act
            var result = await AuthService.RegisterAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Token);
            Assert.IsFalse(string.IsNullOrEmpty(result.Token));
            Assert.IsTrue(result.ExpiresAt > DateTime.UtcNow);
            // Verify user was created in database
            var users = await UserRepository.GetAllUsersAsync();
            Assert.IsTrue(users.Any(u => u.Username == request.Username));
        }

        [TestMethod]
        public async Task RegisterAsync_UsernameAlreadyExists_ThrowsException()
        {
            // Arrange
            var request = new UserRequest
            {
                Username = "existinguser",
                Email = "test@example.com",
                Password = "TestPassword123"
            };

            var existingUser = new User
            {
                Username = "existinguser",
                Email = "other@example.com"
            };

            // Add existing user to database
            await UserRepository.CreateUserAsync(existingUser);

            // Act & Assert
            try
            {
                await AuthService.RegisterAsync(request);
                Assert.Fail("Expected Exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Username already exists", ex.Message);
            }

            // Verify user was not created
            var users = await UserRepository.GetAllUsersAsync();
            Assert.AreEqual(1, users.Count());
        }

        [TestMethod]
        public async Task RegisterAsync_EmailAlreadyExists_ThrowsException()
        {
            // Arrange
            var request = new UserRequest
            {
                Username = "newuser",
                Email = "existing@example.com",
                Password = "TestPassword123"
            };

            var existingUser = new User
            {
                Username = "otheruser",
                Email = "existing@example.com"
            };

            // Add existing user to database
            await UserRepository.CreateUserAsync(existingUser);

            // Act & Assert
            try
            {
                await AuthService.RegisterAsync(request);
                Assert.Fail("Expected Exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Email already exists", ex.Message);
            }

            // Verify user was not created
            var users = await UserRepository.GetAllUsersAsync();
            Assert.AreEqual(1, users.Count());
        }

        #endregion

        #region LoginAsync Tests

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "TestPassword123"
            };

            var passwordHash = await AuthService.HashPasswordAsync(request.Password);
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = "test@example.com",
                PasswordHash = passwordHash,
                Role = "User"
            };

            // Add user to database
            await UserRepository.CreateUserAsync(user);

            // Act
            var result = await AuthService.LoginAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Token);
            Assert.IsFalse(string.IsNullOrEmpty(result.Token));
            Assert.IsTrue(result.ExpiresAt > DateTime.UtcNow);
            // User should be in database
            var users = await UserRepository.GetAllUsersAsync();
            Assert.IsTrue(users.Any(u => u.Username == request.Username));
        }

        [TestMethod]
        public async Task LoginAsync_InvalidUsername_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "nonexistent",
                Password = "TestPassword123"
            };

            // Database is empty by default

            // Act & Assert
            try
            {
                await AuthService.LoginAsync(request);
                Assert.Fail("Expected UnauthorizedAccessException was not thrown");
            }
            catch (UnauthorizedAccessException ex)
            {
                Assert.AreEqual("Invalid credentials", ex.Message);
            }

            // Exception was thrown as expected
        }

        [TestMethod]
        public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "WrongPassword"
            };

            var correctPasswordHash = await AuthService.HashPasswordAsync("CorrectPassword");
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = "test@example.com",
                PasswordHash = correctPasswordHash,
                Role = "User"
            };

            // Add user to database
            await UserRepository.CreateUserAsync(user);

            // Act & Assert
            try
            {
                await AuthService.LoginAsync(request);
                Assert.Fail("Expected UnauthorizedAccessException was not thrown");
            }
            catch (UnauthorizedAccessException ex)
            {
                Assert.AreEqual("Invalid credentials", ex.Message);
            }

            // Exception was thrown as expected
        }

        #endregion
    }
}

