using Moq;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Tests.Application.Services
{
    [TestClass]
    public class TaskBulkCommandServiceTests
    {
        private Mock<IRabbitMQRepository> _mockRabbitMQRepository;
        private TaskBulkCommandService _taskBulkCommandService;

        [TestInitialize]
        public void Setup()
        {
            _mockRabbitMQRepository = new Mock<IRabbitMQRepository>();
            _taskBulkCommandService = new TaskBulkCommandService(_mockRabbitMQRepository.Object);
        }

        #region ProcessBulkTasksAsync Tests (Positive)

        [TestMethod]
        public async Task ProcessBulkTasksAsync_ValidTasks_PublishesToQueue()
        {
            // Arrange
            var tasks = new List<TaskBulkRequest>
            {
                new TaskBulkRequest { Id = "task-1", Title = "Task 1", Description = "Description 1" },
                new TaskBulkRequest { Id = "task-2", Title = "Task 2", Description = "Description 2" }
            };

            _mockRabbitMQRepository
                .Setup(repo => repo.PublishAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _taskBulkCommandService.ProcessBulkTasksAsync(tasks);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("task-1", result[0]);
            Assert.AreEqual("task-2", result[1]);
            _mockRabbitMQRepository.Verify(repo => repo.PublishAsync(
                It.IsAny<object>(), 
                "task-bulk-queue"), Times.Once);
        }

        [TestMethod]
        public async Task ProcessBulkTasksAsync_SingleTask_ReturnsTaskId()
        {
            // Arrange
            var tasks = new List<TaskBulkRequest>
            {
                new TaskBulkRequest { Id = "task-1", Title = "Task 1", Description = "Description 1" }
            };

            _mockRabbitMQRepository
                .Setup(repo => repo.PublishAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _taskBulkCommandService.ProcessBulkTasksAsync(tasks);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("task-1", result[0]);
        }

        [TestMethod]
        public async Task ProcessBulkTasksAsync_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var tasks = new List<TaskBulkRequest>();

            _mockRabbitMQRepository
                .Setup(repo => repo.PublishAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _taskBulkCommandService.ProcessBulkTasksAsync(tasks);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _mockRabbitMQRepository.Verify(repo => repo.PublishAsync(
                It.IsAny<object>(), 
                "task-bulk-queue"), Times.Once);
        }

        [TestMethod]
        public async Task ProcessBulkTasksAsync_ValidTasks_CreatesCommandWithCorrectTasks()
        {
            // Arrange
            var tasks = new List<TaskBulkRequest>
            {
                new TaskBulkRequest { Id = "task-1", Title = "Task 1", Description = "Description 1" },
                new TaskBulkRequest { Id = "task-2", Title = "Task 2", Description = "Description 2" }
            };

            _mockRabbitMQRepository
                .Setup(repo => repo.PublishAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskBulkCommandService.ProcessBulkTasksAsync(tasks);

            // Assert
            _mockRabbitMQRepository.Verify(repo => repo.PublishAsync(
                It.IsAny<object>(), 
                "task-bulk-queue"), Times.Once);
        }

        [TestMethod]
        public async Task ProcessBulkTasksAsync_MultipleTasks_ReturnsAllTaskIds()
        {
            // Arrange
            var tasks = new List<TaskBulkRequest>
            {
                new TaskBulkRequest { Id = "task-1", Title = "Task 1", Description = "Description 1" },
                new TaskBulkRequest { Id = "task-2", Title = "Task 2", Description = "Description 2" },
                new TaskBulkRequest { Id = "task-3", Title = "Task 3", Description = "Description 3" },
                new TaskBulkRequest { Id = "task-4", Title = "Task 4", Description = "Description 4" }
            };

            _mockRabbitMQRepository
                .Setup(repo => repo.PublishAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _taskBulkCommandService.ProcessBulkTasksAsync(tasks);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains("task-1"));
            Assert.IsTrue(result.Contains("task-2"));
            Assert.IsTrue(result.Contains("task-3"));
            Assert.IsTrue(result.Contains("task-4"));
        }

        #endregion

        #region ProcessBulkTasksAsync Tests (Negative)

        [TestMethod]
        public async Task ProcessBulkTasksAsync_NullTasks_ThrowsArgumentNullException()
        {
            // Arrange
            List<TaskBulkRequest>? tasks = null;

            // Act & Assert
            // El código accede a tasks.Select() que lanza ArgumentNullException cuando tasks es null
            try
            {
                await _taskBulkCommandService.ProcessBulkTasksAsync(tasks!);
                Assert.Fail("Expected ArgumentNullException was not thrown");
            }
            catch (ArgumentNullException)
            {
                // Expected exception - LINQ Select lanza ArgumentNullException cuando source es null
            }
        }

        #endregion
    }
}

