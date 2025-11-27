using Microsoft.VisualStudio.TestTools.UnitTesting;
using Q10.TaskManager.Domain.Entities;

namespace Q10.TaskManager.Tests.Domain
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void UpdateLastLogin_SetsLastLoginAt()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com"
            };

            // Act
            user.UpdateLastLogin();

            // Assert
            Assert.IsNotNull(user.LastLoginAt);
            Assert.IsTrue(user.LastLoginAt.Value <= DateTime.UtcNow);
        }

        [TestMethod]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                IsActive = true
            };

            // Act
            user.Deactivate();

            // Assert
            Assert.IsFalse(user.IsActive);
        }

        [TestMethod]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                IsActive = false
            };

            // Act
            user.Activate();

            // Assert
            Assert.IsTrue(user.IsActive);
        }
    }
}


