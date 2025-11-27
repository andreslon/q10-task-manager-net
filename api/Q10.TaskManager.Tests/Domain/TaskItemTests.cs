using Q10.TaskManager.Domain.Entities;

namespace Q10.TaskManager.Tests.Domain
{
    [TestClass]
    public class TaskItemTests
    {
        #region Constructor and Initialization Tests (Positive)

        [TestMethod]
        public void Constructor_InitializesWithDefaultValues_PropertiesSetCorrectly()
        {
            // Act
            var taskItem = new TaskItem
            {
                Title = "Test Title",
                Description = "Test Description"
            };

            // Assert
            Assert.IsNotNull(taskItem.Id);
            Assert.IsFalse(string.IsNullOrEmpty(taskItem.Id));
            Assert.AreEqual("Test Title", taskItem.Title);
            Assert.AreEqual("Test Description", taskItem.Description);
            Assert.IsTrue((DateTime.UtcNow - taskItem.Created).TotalSeconds < 1);
            Assert.IsTrue((DateTime.UtcNow - taskItem.Updated).TotalSeconds < 1);
        }

        [TestMethod]
        public void Constructor_GeneratesUniqueIds_EachInstanceHasDifferentId()
        {
            // Act
            var taskItem1 = new TaskItem { Title = "Title 1" };
            var taskItem2 = new TaskItem { Title = "Title 2" };

            // Assert
            Assert.AreNotEqual(taskItem1.Id, taskItem2.Id);
        }

        [TestMethod]
        public void Constructor_SetsCreatedTimestamp_WithinReasonableTime()
        {
            // Act
            var taskItem = new TaskItem { Title = "Test" };

            // Assert
            var timeDifference = DateTime.UtcNow - taskItem.Created;
            Assert.IsTrue(timeDifference.TotalSeconds >= 0 && timeDifference.TotalSeconds < 1);
        }

        #endregion

        #region UpdateTitle Tests (Positive)

        [TestMethod]
        public void UpdateTitle_ValidTitle_UpdatesTitle()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Initial Title",
                Description = "Initial Description"
            };
            var newTitle = "Updated Title";
            // Act
            taskItem.UpdateTitle(newTitle);

            // Assert
            Assert.AreEqual(newTitle, taskItem.Title);
        }

        [TestMethod]
        public void UpdateTitle_ValidTitle_UpdatesTimestamp()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Initial Title",
                Description = "Initial Description"
            };
            var initialUpdated = taskItem.Updated;
            Thread.Sleep(10); // Small delay to ensure timestamp difference
            var newTitle = "Updated Title";
            
            // Act
            taskItem.UpdateTitle(newTitle);

            // Assert
            Assert.IsTrue(taskItem.Updated > initialUpdated);
            Assert.IsTrue((DateTime.UtcNow - taskItem.Updated).TotalSeconds < 1);
        }

        [TestMethod]
        public void UpdateTitle_MaxLengthTitle_UpdatesSuccessfully()
        {
            // Arrange
            var taskItem = new TaskItem { Title = "Initial Title" };
            var maxLengthTitle = new string('A', 400); // Exactly 400 characters

            // Act
            taskItem.UpdateTitle(maxLengthTitle);

            // Assert
            Assert.AreEqual(400, taskItem.Title.Length);
            Assert.AreEqual(maxLengthTitle, taskItem.Title);
        }

        [TestMethod]
        public void UpdateTitle_ValidTitle_PreservesDescription()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Initial Title",
                Description = "Initial Description"
            };
            var newTitle = "Updated Title";

            // Act
            taskItem.UpdateTitle(newTitle);

            // Assert
            Assert.AreEqual("Initial Description", taskItem.Description);
        }

        #endregion

        #region UpdateTitle Tests (Negative)

        [TestMethod]
        public void UpdateTitle_NullTitle_ThrowsArgumentException()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Initial Title",
                Description = "Initial Description"
            };

            // Act & Assert
            var exception = Assert.ThrowsExactly<ArgumentException>(() => taskItem.UpdateTitle(null!));
            Assert.AreEqual("Title cannot be null or empty", exception.Message);
        }

        [TestMethod]
        public void UpdateTitle_EmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Initial Title",
                Description = "Initial Description"
            };

            // Act & Assert
            var exception = Assert.ThrowsExactly<ArgumentException>(() => taskItem.UpdateTitle(string.Empty));
            Assert.AreEqual("Title cannot be null or empty", exception.Message);
        }

        [TestMethod]
        public void UpdateTitle_WhitespaceOnlyTitle_AcceptsWhitespace()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Initial Title",
                Description = "Initial Description"
            };
            var whitespaceTitle = "   ";

            // Act
            taskItem.UpdateTitle(whitespaceTitle);

            // Assert
            // Note: The implementation only checks for null/empty, not whitespace
            Assert.AreEqual(whitespaceTitle, taskItem.Title);
        }

        [TestMethod]
        public void UpdateTitle_ExceedsMaxLength_ThrowsArgumentException()
        {
            // Arrange
            var taskItem = new TaskItem { Title = "Initial Title" };
            var tooLongTitle = new string('A', 401); // 401 characters, exceeds max of 400

            // Act & Assert
            var exception = Assert.ThrowsExactly<ArgumentException>(() => taskItem.UpdateTitle(tooLongTitle));
            Assert.AreEqual("Title cannot exceed 400 characters", exception.Message);
        }

        [TestMethod]
        public void UpdateTitle_ExceedsMaxLength_DoesNotUpdateTitle()
        {
            // Arrange
            var taskItem = new TaskItem { Title = "Initial Title" };
            var originalTitle = taskItem.Title;
            var tooLongTitle = new string('A', 401);

            // Act & Assert
            try
            {
                taskItem.UpdateTitle(tooLongTitle);
                Assert.Fail("Expected ArgumentException was not thrown");
            }
            catch (ArgumentException)
            {
                // Assert that title was not updated
                Assert.AreEqual(originalTitle, taskItem.Title);
            }
        }

        #endregion

        #region UpdateDescription Tests (Positive)

        [TestMethod]
        public void UpdateDescription_ValidDescription_UpdatesDescription()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Test Title",
                Description = "Initial Description"
            };
            var newDescription = "Updated Description";

            // Act
            taskItem.UpdateDescription(newDescription);

            // Assert
            Assert.AreEqual(newDescription, taskItem.Description);
        }

        [TestMethod]
        public void UpdateDescription_ValidDescription_UpdatesTimestamp()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Test Title",
                Description = "Initial Description"
            };
            var initialUpdated = taskItem.Updated;
            Thread.Sleep(10); // Small delay to ensure timestamp difference
            var newDescription = "Updated Description";

            // Act
            taskItem.UpdateDescription(newDescription);

            // Assert
            Assert.IsTrue(taskItem.Updated > initialUpdated);
            Assert.IsTrue((DateTime.UtcNow - taskItem.Updated).TotalSeconds < 1);
        }

        [TestMethod]
        public void UpdateDescription_MaxLengthDescription_UpdatesSuccessfully()
        {
            // Arrange
            var taskItem = new TaskItem { Title = "Test Title", Description = "Initial" };
            var maxLengthDescription = new string('B', 500); // Exactly 500 characters

            // Act
            taskItem.UpdateDescription(maxLengthDescription);

            // Assert
            Assert.AreEqual(500, taskItem.Description.Length);
            Assert.AreEqual(maxLengthDescription, taskItem.Description);
        }

        [TestMethod]
        public void UpdateDescription_ValidDescription_PreservesTitle()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Test Title",
                Description = "Initial Description"
            };
            var newDescription = "Updated Description";

            // Act
            taskItem.UpdateDescription(newDescription);

            // Assert
            Assert.AreEqual("Test Title", taskItem.Title);
        }

        [TestMethod]
        public void UpdateDescription_EmptyString_ThrowsArgumentException()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Test Title",
                Description = "Initial Description"
            };

            // Act & Assert
            var exception = Assert.ThrowsExactly<ArgumentException>(() => taskItem.UpdateDescription(string.Empty));
            Assert.AreEqual("Description cannot be null or empty", exception.Message);
        }

        #endregion

        #region UpdateDescription Tests (Negative)

        [TestMethod]
        public void UpdateDescription_NullDescription_ThrowsArgumentException()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Test Title",
                Description = "Initial Description"
            };

            // Act & Assert
            var exception = Assert.ThrowsExactly<ArgumentException>(() => taskItem.UpdateDescription(null!));
            Assert.AreEqual("Description cannot be null or empty", exception.Message);
        }

        [TestMethod]
        public void UpdateDescription_ExceedsMaxLength_ThrowsArgumentException()
        {
            // Arrange
            var taskItem = new TaskItem { Title = "Test Title", Description = "Initial" };
            var tooLongDescription = new string('B', 501); // 501 characters, exceeds max of 500

            // Act & Assert
            var exception = Assert.ThrowsExactly<ArgumentException>(() => taskItem.UpdateDescription(tooLongDescription));
            Assert.AreEqual("Description cannot exceed 500 characters", exception.Message);
        }

        [TestMethod]
        public void UpdateDescription_ExceedsMaxLength_DoesNotUpdateDescription()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Test Title",
                Description = "Initial Description"
            };
            var originalDescription = taskItem.Description;
            var tooLongDescription = new string('B', 501);

            // Act & Assert
            try
            {
                taskItem.UpdateDescription(tooLongDescription);
                Assert.Fail("Expected ArgumentException was not thrown");
            }
            catch (ArgumentException)
            {
                // Assert that description was not updated
                Assert.AreEqual(originalDescription, taskItem.Description);
            }
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void UpdateTitleAndDescription_MultipleUpdates_UpdatesCorrectly()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Initial Title",
                Description = "Initial Description"
            };

            // Act
            taskItem.UpdateTitle("First Update");
            Thread.Sleep(10);
            taskItem.UpdateDescription("First Description Update");
            Thread.Sleep(10);
            taskItem.UpdateTitle("Second Update");
            Thread.Sleep(10);
            taskItem.UpdateDescription("Second Description Update");

            // Assert
            Assert.AreEqual("Second Update", taskItem.Title);
            Assert.AreEqual("Second Description Update", taskItem.Description);
            Assert.IsTrue((DateTime.UtcNow - taskItem.Updated).TotalSeconds < 1);
        }

        #endregion
    }
}
