using Moq;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;
using Q10.UserManager.Infrastructure.Repositories;

namespace Q10.TaskManager.Tests.Application.Services
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<UserRepository> _mockUserRepositoryConcrete;
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserRepositoryConcrete = new Mock<UserRepository>(MockBehavior.Loose, null!);
            _authService = new AuthService(_mockUserRepositoryConcrete.Object);
            // Asignar el mock de la interfaz a la propiedad pública
            _authService.UserRepository = _mockUserRepository.Object;
        }

        #region HashPasswordAsync Tests (Positive)

        [TestMethod]
        public async Task HashPasswordAsync_ValidPassword_ReturnsHashedPassword()
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            var result = await _authService.HashPasswordAsync(password);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.AreNotEqual(password, result);
        }

        [TestMethod]
        public async Task HashPasswordAsync_SamePassword_DifferentHashes()
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            var hash1 = await _authService.HashPasswordAsync(password);
            var hash2 = await _authService.HashPasswordAsync(password);

            // Assert
            Assert.AreNotEqual(hash1, hash2); // BCrypt generates different hashes each time
        }

        #endregion

        #region VerifyPasswordAsync Tests (Positive)

        [TestMethod]
        public async Task VerifyPasswordAsync_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "TestPassword123";
            var hash = await _authService.HashPasswordAsync(password);

            // Act
            var result = await _authService.VerifyPasswordAsync(password, hash);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task VerifyPasswordAsync_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "TestPassword123";
            var wrongPassword = "WrongPassword";
            var hash = await _authService.HashPasswordAsync(password);

            // Act
            var result = await _authService.VerifyPasswordAsync(wrongPassword, hash);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task VerifyPasswordAsync_InvalidHash_ReturnsFalse()
        {
            // Arrange
            var password = "TestPassword123";
            var invalidHash = "invalid.hash.string";

            // Act
            var result = await _authService.VerifyPasswordAsync(password, invalidHash);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region GenerateTokenAsync Tests (Positive)

        [TestMethod]
        public async Task GenerateTokenAsync_ValidInputs_ReturnsToken()
        {
            // Arrange
            var userId = "user-123";
            var email = "test@example.com";
            var role = "User";

            // Act
            var result = await _authService.GenerateTokenAsync(userId, email, role);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public async Task GenerateTokenAsync_DifferentInputs_DifferentTokens()
        {
            // Arrange
            var userId1 = "user-123";
            var userId2 = "user-456";
            var email = "test@example.com";
            var role = "User";

            // Act
            var token1 = await _authService.GenerateTokenAsync(userId1, email, role);
            var token2 = await _authService.GenerateTokenAsync(userId2, email, role);

            // Assert
            Assert.AreNotEqual(token1, token2);
        }

        [TestMethod]
        public async Task GenerateTokenAsync_ValidInputs_TokenContainsClaims()
        {
            // Arrange
            var userId = "user-123";
            var email = "test@example.com";
            var role = "Admin";

            // Act
            var token = await _authService.GenerateTokenAsync(userId, email, role);

            // Assert
            Assert.IsNotNull(token);
            // Token should be a valid JWT format (three parts separated by dots)
            var parts = token.Split('.');
            Assert.AreEqual(3, parts.Length);
        }

        #endregion

        #region RegisterAsync Tests (Positive)

        [TestMethod]
        public async Task RegisterAsync_ValidRequest_CreatesUserAndReturnsToken()
        {
            // Arrange
            var request = new UserRequest
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123",
                FirstName = "John",
                LastName = "Doe"
            };

            var users = new List<User>().AsQueryable();
            var createdUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = "User"
            };

            _mockUserRepository
                .Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            _mockUserRepository
                .Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Token));
            Assert.IsTrue(result.ExpiresAt > DateTime.UtcNow);
            _mockUserRepository.Verify(repo => repo.GetAllUsersAsync(), Times.Once);
            _mockUserRepository.Verify(repo => repo.CreateUserAsync(It.Is<User>(u => 
                u.Username == request.Username && 
                u.Email == request.Email &&
                u.Role == "User")), Times.Once);
        }

        [TestMethod]
        public async Task RegisterAsync_ValidRequest_SetsDefaultRoleToUser()
        {
            // Arrange
            var request = new UserRequest
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123"
            };

            var users = new List<User>().AsQueryable();
            var createdUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = request.Email,
                Role = "User"
            };

            _mockUserRepository
                .Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            _mockUserRepository
                .Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUser);

            // Act
            await _authService.RegisterAsync(request);

            // Assert
            _mockUserRepository.Verify(repo => repo.CreateUserAsync(It.Is<User>(u => 
                u.Role == "User")), Times.Once);
        }

        #endregion

        #region RegisterAsync Tests (Negative)

        [TestMethod]
        public async Task RegisterAsync_DuplicateUsername_ThrowsException()
        {
            // Arrange
            var request = new UserRequest
            {
                Username = "existinguser",
                Email = "newuser@example.com",
                Password = "Password123"
            };

            var existingUser = new User
            {
                Username = "existinguser",
                Email = "existing@example.com"
            };

            var users = new List<User> { existingUser }.AsQueryable();

            _mockUserRepository
                .Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act & Assert
            try
            {
                await _authService.RegisterAsync(request);
                Assert.Fail("Expected Exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Username already exists", ex.Message);
            }
            _mockUserRepository.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public async Task RegisterAsync_DuplicateEmail_ThrowsException()
        {
            // Arrange
            var request = new UserRequest
            {
                Username = "newuser",
                Email = "existing@example.com",
                Password = "Password123"
            };

            var existingUser = new User
            {
                Username = "existinguser",
                Email = "existing@example.com"
            };

            var users = new List<User> { existingUser }.AsQueryable();

            _mockUserRepository
                .Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act & Assert
            try
            {
                await _authService.RegisterAsync(request);
                Assert.Fail("Expected Exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Email already exists", ex.Message);
            }
            _mockUserRepository.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), Times.Never);
        }

        #endregion

        #region LoginAsync Tests (Positive)

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "Password123"
            };

            var passwordHash = await _authService.HashPasswordAsync(request.Password);
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = "testuser@example.com",
                PasswordHash = passwordHash,
                Role = "User"
            };

            var users = new List<User> { user }.AsQueryable();

            _mockUserRepository
                .Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.Token));
            Assert.IsTrue(result.ExpiresAt > DateTime.UtcNow);
        }

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsTokenWithCorrectExpiration()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "Password123"
            };

            var passwordHash = await _authService.HashPasswordAsync(request.Password);
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = "testuser@example.com",
                PasswordHash = passwordHash,
                Role = "User"
            };

            var users = new List<User> { user }.AsQueryable();

            _mockUserRepository
                .Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            var expectedExpiration = DateTime.UtcNow.AddMinutes(60);
            var timeDifference = Math.Abs((result.ExpiresAt - expectedExpiration).TotalMinutes);
            Assert.IsTrue(timeDifference < 1); // Should be within 1 minute
        }

        #endregion

        #region LoginAsync Tests (Negative)

        [TestMethod]
        public async Task LoginAsync_InvalidUsername_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "nonexistent",
                Password = "Password123"
            };

            var users = new List<User>().AsQueryable();

            _mockUserRepository
                .Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act & Assert
            try
            {
                await _authService.LoginAsync(request);
                Assert.Fail("Expected UnauthorizedAccessException was not thrown");
            }
            catch (UnauthorizedAccessException ex)
            {
                Assert.AreEqual("Invalid credentials", ex.Message);
            }
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

            var correctPassword = "Password123";
            var passwordHash = await _authService.HashPasswordAsync(correctPassword);
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = "testuser@example.com",
                PasswordHash = passwordHash,
                Role = "User"
            };

            var users = new List<User> { user }.AsQueryable();

            _mockUserRepository
                .Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act & Assert
            try
            {
                await _authService.LoginAsync(request);
                Assert.Fail("Expected UnauthorizedAccessException was not thrown");
            }
            catch (UnauthorizedAccessException ex)
            {
                Assert.AreEqual("Invalid credentials", ex.Message);
            }
        }

        #endregion
    }
}

