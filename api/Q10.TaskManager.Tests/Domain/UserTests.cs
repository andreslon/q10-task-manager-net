using Q10.TaskManager.Domain.Entities;

namespace Q10.TaskManager.Tests.Domain
{
    [TestClass]
    public class UserTests
    {
        #region Constructor and Initialization Tests (Positive)

        [TestMethod]
        public void Constructor_InitializesWithDefaultValues_PropertiesSetCorrectly()
        {
            // Act
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword"
            };

            // Assert
            Assert.IsNotNull(user.Id);
            Assert.IsFalse(string.IsNullOrEmpty(user.Id));
            Assert.AreEqual("testuser", user.Username);
            Assert.AreEqual("test@example.com", user.Email);
            Assert.AreEqual("hashedpassword", user.PasswordHash);
            Assert.AreEqual(string.Empty, user.FirstName);
            Assert.AreEqual(string.Empty, user.LastName);
            Assert.IsTrue((DateTime.UtcNow - user.CreatedAt).TotalSeconds < 1);
            Assert.IsNull(user.LastLoginAt);
            Assert.IsTrue(user.IsActive);
            Assert.AreEqual("User", user.Role);
        }

        [TestMethod]
        public void Constructor_GeneratesUniqueIds_EachInstanceHasDifferentId()
        {
            // Act
            var user1 = new User { Username = "user1", Email = "user1@test.com", PasswordHash = "hash1" };
            var user2 = new User { Username = "user2", Email = "user2@test.com", PasswordHash = "hash2" };

            // Assert
            Assert.AreNotEqual(user1.Id, user2.Id);
        }

        [TestMethod]
        public void Constructor_SetsCreatedAtTimestamp_WithinReasonableTime()
        {
            // Act
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Assert
            var timeDifference = DateTime.UtcNow - user.CreatedAt;
            Assert.IsTrue(timeDifference.TotalSeconds >= 0 && timeDifference.TotalSeconds < 1);
        }

        [TestMethod]
        public void Constructor_SetsDefaultRole_ToUser()
        {
            // Act
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Assert
            Assert.AreEqual("User", user.Role);
        }

        [TestMethod]
        public void Constructor_SetsIsActive_ToTrue()
        {
            // Act
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Assert
            Assert.IsTrue(user.IsActive);
        }

        [TestMethod]
        public void Constructor_InitializesLastLoginAt_AsNull()
        {
            // Act
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Assert
            Assert.IsNull(user.LastLoginAt);
        }

        #endregion

        #region Property Assignment Tests (Positive)

        [TestMethod]
        public void Username_SetValidValue_UpdatesCorrectly()
        {
            // Arrange
            var user = new User { Username = "initial", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.Username = "newusername";

            // Assert
            Assert.AreEqual("newusername", user.Username);
        }

        [TestMethod]
        public void Email_SetValidValue_UpdatesCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", Email = "old@test.com", PasswordHash = "hash" };

            // Act
            user.Email = "new@test.com";

            // Assert
            Assert.AreEqual("new@test.com", user.Email);
        }

        [TestMethod]
        public void PasswordHash_SetValidValue_UpdatesCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "oldhash" };

            // Act
            user.PasswordHash = "newhash";

            // Assert
            Assert.AreEqual("newhash", user.PasswordHash);
        }

        [TestMethod]
        public void FirstName_SetValidValue_UpdatesCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.FirstName = "John";

            // Assert
            Assert.AreEqual("John", user.FirstName);
        }

        [TestMethod]
        public void LastName_SetValidValue_UpdatesCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.LastName = "Doe";

            // Assert
            Assert.AreEqual("Doe", user.LastName);
        }

        [TestMethod]
        public void Role_SetValidValue_UpdatesCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.Role = "Admin";

            // Assert
            Assert.AreEqual("Admin", user.Role);
        }

        [TestMethod]
        public void IsActive_SetToFalse_UpdatesCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.IsActive = false;

            // Assert
            Assert.IsFalse(user.IsActive);
        }

        [TestMethod]
        public void LastLoginAt_SetValidDateTime_UpdatesCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };
            var loginTime = DateTime.UtcNow;

            // Act
            user.LastLoginAt = loginTime;

            // Assert
            Assert.AreEqual(loginTime, user.LastLoginAt);
        }

        [TestMethod]
        public void LastLoginAt_SetToNull_ClearsValue()
        {
            // Arrange
            var user = new User 
            { 
                Username = "test", 
                Email = "test@test.com", 
                PasswordHash = "hash",
                LastLoginAt = DateTime.UtcNow
            };

            // Act
            user.LastLoginAt = null;

            // Assert
            Assert.IsNull(user.LastLoginAt);
        }

        #endregion

        #region MaxLength Validation Tests (Positive)

        [TestMethod]
        public void Username_MaxLengthValue_AcceptsCorrectly()
        {
            // Arrange
            var user = new User { Email = "test@test.com", PasswordHash = "hash" };
            var maxLengthUsername = new string('A', 100); // Exactly 100 characters

            // Act
            user.Username = maxLengthUsername;

            // Assert
            Assert.AreEqual(100, user.Username.Length);
            Assert.AreEqual(maxLengthUsername, user.Username);
        }

        [TestMethod]
        public void Email_MaxLengthValue_AcceptsCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", PasswordHash = "hash" };
            var maxLengthEmail = new string('A', 255); // Exactly 255 characters

            // Act
            user.Email = maxLengthEmail;

            // Assert
            Assert.AreEqual(255, user.Email.Length);
            Assert.AreEqual(maxLengthEmail, user.Email);
        }

        [TestMethod]
        public void FirstName_MaxLengthValue_AcceptsCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };
            var maxLengthFirstName = new string('A', 100); // Exactly 100 characters

            // Act
            user.FirstName = maxLengthFirstName;

            // Assert
            Assert.AreEqual(100, user.FirstName.Length);
            Assert.AreEqual(maxLengthFirstName, user.FirstName);
        }

        [TestMethod]
        public void LastName_MaxLengthValue_AcceptsCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };
            var maxLengthLastName = new string('A', 100); // Exactly 100 characters

            // Act
            user.LastName = maxLengthLastName;

            // Assert
            Assert.AreEqual(100, user.LastName.Length);
            Assert.AreEqual(maxLengthLastName, user.LastName);
        }

        [TestMethod]
        public void Role_MaxLengthValue_AcceptsCorrectly()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };
            var maxLengthRole = new string('A', 50); // Exactly 50 characters

            // Act
            user.Role = maxLengthRole;

            // Assert
            Assert.AreEqual(50, user.Role.Length);
            Assert.AreEqual(maxLengthRole, user.Role);
        }

        #endregion

        #region Edge Cases and Negative Tests

        [TestMethod]
        public void Username_SetEmptyString_AcceptsEmptyString()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.Username = string.Empty;

            // Assert
            Assert.AreEqual(string.Empty, user.Username);
        }

        [TestMethod]
        public void Email_SetEmptyString_AcceptsEmptyString()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.Email = string.Empty;

            // Assert
            Assert.AreEqual(string.Empty, user.Email);
        }

        [TestMethod]
        public void PasswordHash_SetEmptyString_AcceptsEmptyString()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.PasswordHash = string.Empty;

            // Assert
            Assert.AreEqual(string.Empty, user.PasswordHash);
        }

        [TestMethod]
        public void FirstName_SetEmptyString_AcceptsEmptyString()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.FirstName = string.Empty;

            // Assert
            Assert.AreEqual(string.Empty, user.FirstName);
        }

        [TestMethod]
        public void LastName_SetEmptyString_AcceptsEmptyString()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.LastName = string.Empty;

            // Assert
            Assert.AreEqual(string.Empty, user.LastName);
        }

        [TestMethod]
        public void Role_SetEmptyString_AcceptsEmptyString()
        {
            // Arrange
            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            user.Role = string.Empty;

            // Assert
            Assert.AreEqual(string.Empty, user.Role);
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void User_CompleteUserProfile_AllPropertiesSetCorrectly()
        {
            // Arrange & Act
            var user = new User
            {
                Username = "johndoe",
                Email = "john.doe@example.com",
                PasswordHash = "hashedpassword123",
                FirstName = "John",
                LastName = "Doe",
                Role = "Admin",
                IsActive = true,
                LastLoginAt = DateTime.UtcNow
            };

            // Assert
            Assert.IsNotNull(user.Id);
            Assert.AreEqual("johndoe", user.Username);
            Assert.AreEqual("john.doe@example.com", user.Email);
            Assert.AreEqual("hashedpassword123", user.PasswordHash);
            Assert.AreEqual("John", user.FirstName);
            Assert.AreEqual("Doe", user.LastName);
            Assert.AreEqual("Admin", user.Role);
            Assert.IsTrue(user.IsActive);
            Assert.IsNotNull(user.LastLoginAt);
            Assert.IsTrue((DateTime.UtcNow - user.CreatedAt).TotalSeconds < 1);
        }

        [TestMethod]
        public void User_DeactivateUser_IsActiveBecomesFalse()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash",
                IsActive = true
            };

            // Act
            user.IsActive = false;

            // Assert
            Assert.IsFalse(user.IsActive);
        }

        [TestMethod]
        public void User_RecordLogin_LastLoginAtUpdated()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash"
            };
            var loginTime = DateTime.UtcNow;

            // Act
            user.LastLoginAt = loginTime;

            // Assert
            Assert.IsNotNull(user.LastLoginAt);
            Assert.AreEqual(loginTime, user.LastLoginAt);
        }

        [TestMethod]
        public void User_ChangeRole_UpdatesRoleCorrectly()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash",
                Role = "User"
            };

            // Act
            user.Role = "Moderator";
            user.Role = "Admin";

            // Assert
            Assert.AreEqual("Admin", user.Role);
        }

        #endregion

        #region Deactivate Method Tests (Positive)

        [TestMethod]
        public void Deactivate_WhenUserIsActive_SetsIsActiveToFalse()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash",
                IsActive = true
            };

            // Act
            user.Deactive();

            // Assert
            Assert.IsFalse(user.IsActive);
        }

        [TestMethod]
        public void Deactivate_WhenUserIsAlreadyInactive_RemainsInactive()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash",
                IsActive = false
            };

            // Act
            user.Deactive();

            // Assert
            Assert.IsFalse(user.IsActive);
        }

        [TestMethod]
        public void Deactivate_DoesNotModifyOtherProperties()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = "hash123",
                FirstName = "John",
                LastName = "Doe",
                Role = "Admin",
                IsActive = true
            };
            var originalUsername = user.Username;
            var originalEmail = user.Email;
            var originalPasswordHash = user.PasswordHash;
            var originalFirstName = user.FirstName;
            var originalLastName = user.LastName;
            var originalRole = user.Role;

            // Act
            user.Deactive();

            // Assert
            Assert.IsFalse(user.IsActive);
            Assert.AreEqual(originalUsername, user.Username);
            Assert.AreEqual(originalEmail, user.Email);
            Assert.AreEqual(originalPasswordHash, user.PasswordHash);
            Assert.AreEqual(originalFirstName, user.FirstName);
            Assert.AreEqual(originalLastName, user.LastName);
            Assert.AreEqual(originalRole, user.Role);
        }

        [TestMethod]
        public void Deactivate_OnNewUserWithDefaultActive_SetsToInactive()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash"
            };
            // New users are active by default
            Assert.IsTrue(user.IsActive);

            // Act
            user.Deactive();

            // Assert
            Assert.IsFalse(user.IsActive);
        }

        #endregion

        #region Activate Method Tests (Positive)

        [TestMethod]
        public void Activate_WhenUserIsInactive_SetsIsActiveToTrue()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash",
                IsActive = false
            };

            // Act
            user.Activate();

            // Assert
            Assert.IsTrue(user.IsActive);
        }

        [TestMethod]
        public void Activate_WhenUserIsAlreadyActive_RemainsActive()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash",
                IsActive = true
            };

            // Act
            user.Activate();

            // Assert
            Assert.IsTrue(user.IsActive);
        }

        [TestMethod]
        public void Activate_DoesNotModifyOtherProperties()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = "hash123",
                FirstName = "John",
                LastName = "Doe",
                Role = "Admin",
                IsActive = false
            };
            var originalUsername = user.Username;
            var originalEmail = user.Email;
            var originalPasswordHash = user.PasswordHash;
            var originalFirstName = user.FirstName;
            var originalLastName = user.LastName;
            var originalRole = user.Role;

            // Act
            user.Activate();

            // Assert
            Assert.IsTrue(user.IsActive);
            Assert.AreEqual(originalUsername, user.Username);
            Assert.AreEqual(originalEmail, user.Email);
            Assert.AreEqual(originalPasswordHash, user.PasswordHash);
            Assert.AreEqual(originalFirstName, user.FirstName);
            Assert.AreEqual(originalLastName, user.LastName);
            Assert.AreEqual(originalRole, user.Role);
        }

        #endregion

        #region Activate/Deactivate Integration Tests

        [TestMethod]
        public void ActivateAndDeactivate_MultipleTransitions_StateChangesCorrectly()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash",
                IsActive = true
            };

            // Act & Assert - Deactivate
            user.Deactive();
            Assert.IsFalse(user.IsActive);

            // Act & Assert - Activate
            user.Activate();
            Assert.IsTrue(user.IsActive);

            // Act & Assert - Deactivate again
            user.Deactive();
            Assert.IsFalse(user.IsActive);

            // Act & Assert - Activate again
            user.Activate();
            Assert.IsTrue(user.IsActive);
        }

        [TestMethod]
        public void DeactivateAndActivate_RepeatedCalls_IsIdempotent()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash",
                IsActive = true
            };

            // Act - Multiple deactivations
            user.Deactive();
            user.Deactive();
            user.Deactive();

            // Assert
            Assert.IsFalse(user.IsActive);

            // Act - Multiple activations
            user.Activate();
            user.Activate();
            user.Activate();

            // Assert
            Assert.IsTrue(user.IsActive);
        }

        [TestMethod]
        public void ActivateAndDeactivate_WithOtherPropertyChanges_DoesNotInterfere()
        {
            // Arrange
            var user = new User
            {
                Username = "test",
                Email = "test@test.com",
                PasswordHash = "hash",
                IsActive = true
            };

            // Act
            user.Deactive();
            user.Username = "updateduser";
            user.Email = "updated@test.com";
            user.Activate();
            user.Role = "Admin";

            // Assert
            Assert.IsTrue(user.IsActive);
            Assert.AreEqual("updateduser", user.Username);
            Assert.AreEqual("updated@test.com", user.Email);
            Assert.AreEqual("Admin", user.Role);
        }

        #endregion
    }
}
