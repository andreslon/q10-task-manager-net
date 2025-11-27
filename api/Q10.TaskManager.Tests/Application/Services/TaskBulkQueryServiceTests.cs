using Moq;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Tests.Application.Services
{
    [TestClass]
    public class TaskBulkQueryServiceTests
    {
        private Mock<ITaskRepository> _mockTaskRepository;
        private TaskBulkQueryService _taskBulkQueryService;

        [TestInitialize]
        public void Setup()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _taskBulkQueryService = new TaskBulkQueryService(_mockTaskRepository.Object);
        }

        #region GetTaskByIdAsync Tests (Positive)

        [TestMethod]
        public async Task GetTaskByIdAsync_ValidTaskId_ReturnsTaskBulkResponse()
        {
            // Arrange
            var taskId = "test-task-id";
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description"
            };

            _mockTaskRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _taskBulkQueryService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(taskId, result.TaskId);
            Assert.AreEqual(task.Title, result.Title);
            Assert.IsTrue(result.IsSuccess);
            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task GetTaskByIdAsync_ValidTaskId_SetsIsSuccessToTrue()
        {
            // Arrange
            var taskId = "test-task-id";
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description"
            };

            _mockTaskRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _taskBulkQueryService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetTaskByIdAsync_ValidTaskId_MapsTaskPropertiesCorrectly()
        {
            // Arrange
            var taskId = "test-task-id";
            var task = new TaskItem
            {
                Id = taskId,
                Title = "My Test Task",
                Description = "My Test Description"
            };

            _mockTaskRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _taskBulkQueryService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.AreEqual(task.Id, result.TaskId);
            Assert.AreEqual(task.Title, result.Title);
        }

        #endregion

        #region GetTaskByIdAsync Tests (Negative)

        [TestMethod]
        public async Task GetTaskByIdAsync_NonExistentTaskId_ReturnsNull()
        {
            // Arrange
            var taskId = "non-existent-id";

            _mockTaskRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync((TaskItem)null!);

            // Act
            var result = await _taskBulkQueryService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.IsNull(result);
            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task GetTaskByIdAsync_EmptyTaskId_CallsRepository()
        {
            // Arrange
            var taskId = string.Empty;

            _mockTaskRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync((TaskItem)null!);

            // Act
            var result = await _taskBulkQueryService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.IsNull(result);
            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }

        #endregion
    }
}

