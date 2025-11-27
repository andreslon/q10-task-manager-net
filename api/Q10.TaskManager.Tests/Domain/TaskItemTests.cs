using Microsoft.VisualStudio.TestTools.UnitTesting;
using Q10.TaskManager.Domain.Entities;

namespace Q10.TaskManager.Tests.Domain
{
    [TestClass]
    public class TaskItemTests
    {
        [TestMethod]
        public void UpdateTitle_ValidTitle_UpdatesTitle()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Old Title",
                Description = "Description"
            };

            var newTitle = "New Title";

            // Act
            task.UpdateTitle(newTitle);

            // Assert
            Assert.AreEqual(newTitle, task.Title);
            Assert.IsTrue(task.Updated > DateTime.UtcNow.AddSeconds(-1));
        }

        [TestMethod]
        public void UpdateTitle_NullOrEmpty_ThrowsException()
        {
            // Arrange
            var task = new TaskItem { Title = "Valid Title" };

            // Act & Assert
            try
            {
                task.UpdateTitle(string.Empty);
                Assert.Fail("Expected ArgumentException was not thrown");
            }
            catch (ArgumentException)
            {
                // Expected exception
            }
        }

        [TestMethod]
        public void UpdateTitle_TooLong_ThrowsException()
        {
            // Arrange
            var task = new TaskItem { Title = "Valid Title" };
            var longTitle = new string('a', 401); // 401 characters

            // Act & Assert
            try
            {
                task.UpdateTitle(longTitle);
                Assert.Fail("Expected ArgumentException was not thrown");
            }
            catch (ArgumentException)
            {
                // Expected exception
            }
        }

        [TestMethod]
        public void UpdateDescription_ValidDescription_UpdatesDescription()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Title",
                Description = "Old Description"
            };

            var newDescription = "New Description";

            // Act
            task.UpdateDescription(newDescription);

            // Assert
            Assert.AreEqual(newDescription, task.Description);
            Assert.IsTrue(task.Updated > DateTime.UtcNow.AddSeconds(-1));
        }

        [TestMethod]
        public void UpdateDescription_Null_SetsEmptyString()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Title",
                Description = "Old Description"
            };

            // Act
            task.UpdateDescription(null!);

            // Assert
            Assert.AreEqual(string.Empty, task.Description);
        }
    }
}

