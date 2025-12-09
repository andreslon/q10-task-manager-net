using Moq;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace Q10.TaskManager.Tests.Services
{
    [TestClass]
    public class TaskBulkQueryServiceTests
    {
        public Mock<ITaskRepository> MockTaskRepository { get; set; } = null!;
        public ITaskBulkQueryService TaskBulkQueryService { get; set; } = null!;

        [TestInitialize]
        public void Initialize()
        {
            MockTaskRepository = new Mock<ITaskRepository>();
            TaskBulkQueryService = new TaskBulkQueryService(MockTaskRepository.Object);
        }

        #region GetTaskByIdAsync Tests

        [TestMethod]
        public async Task GetTaskByIdAsync_ExistingTask_ReturnsTaskBulkResponse()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var taskItem = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description"
            };

            MockTaskRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync(taskItem);

            // Act
            var result = await TaskBulkQueryService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(taskId, result.TaskId);
            Assert.AreEqual("Test Task", result.Title);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorMessage));
            MockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task GetTaskByIdAsync_NonExistingTask_ReturnsNull()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
#pragma warning disable CS8620 // Nullability mismatch - intentional for testing null return scenario
            MockTaskRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);
#pragma warning restore CS8620

            // Act
            var result = await TaskBulkQueryService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.IsNull(result);
            MockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task GetTaskByIdAsync_ExistingTask_ResponseHasCorrectProperties()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var taskItem = new TaskItem
            {
                Id = taskId,
                Title = "Complex Task Title",
                Description = "Complex Task Description"
            };

            MockTaskRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync(taskItem);

            // Act
            var result = await TaskBulkQueryService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(taskItem.Id, result.TaskId);
            Assert.AreEqual(taskItem.Title, result.Title);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(string.IsNullOrEmpty(result.ErrorMessage));
        }

        #endregion
    }
}


