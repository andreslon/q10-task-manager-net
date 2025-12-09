using Moq;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Q10.TaskManager.Tests.Services
{
    [TestClass]
    public class TaskBulkCommandServiceTests
    {
        public Mock<IRabbitMQRepository> MockRabbitMQRepository { get; set; } = null!;
        public ITaskBulkCommandService TaskBulkCommandService { get; set; } = null!;

        [TestInitialize]
        public void Initialize()
        {
            MockRabbitMQRepository = new Mock<IRabbitMQRepository>();
            TaskBulkCommandService = new TaskBulkCommandService(MockRabbitMQRepository.Object);
        }

        #region ProcessBulkTasksAsync Tests

        [TestMethod]
        public async Task ProcessBulkTasksAsync_ValidTasks_PublishesCommandAndReturnsTaskIds()
        {
            // Arrange
            var tasks = new List<TaskBulkRequest>
            {
                new TaskBulkRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Task 1",
                    Description = "Description 1"
                },
                new TaskBulkRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Task 2",
                    Description = "Description 2"
                },
                new TaskBulkRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Task 3",
                    Description = "Description 3"
                }
            };

            MockRabbitMQRepository
                .Setup(repo => repo.PublishAsync(It.IsAny<TaskBulkCommand>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await TaskBulkCommandService.ProcessBulkTasksAsync(tasks);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(tasks[0].Id, result[0]);
            Assert.AreEqual(tasks[1].Id, result[1]);
            Assert.AreEqual(tasks[2].Id, result[2]);
            MockRabbitMQRepository.Verify(
                repo => repo.PublishAsync(It.Is<TaskBulkCommand>(c => c.Tasks.Count == 3), "task-bulk-queue"),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessBulkTasksAsync_EmptyList_PublishesCommandAndReturnsEmptyList()
        {
            // Arrange
            var tasks = new List<TaskBulkRequest>();

            MockRabbitMQRepository
                .Setup(repo => repo.PublishAsync(It.IsAny<TaskBulkCommand>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await TaskBulkCommandService.ProcessBulkTasksAsync(tasks);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            MockRabbitMQRepository.Verify(
                repo => repo.PublishAsync(It.Is<TaskBulkCommand>(c => c.Tasks.Count == 0), "task-bulk-queue"),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessBulkTasksAsync_SingleTask_PublishesCommandAndReturnsTaskId()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var tasks = new List<TaskBulkRequest>
            {
                new TaskBulkRequest
                {
                    Id = taskId,
                    Title = "Single Task",
                    Description = "Single Description"
                }
            };

            MockRabbitMQRepository
                .Setup(repo => repo.PublishAsync(It.IsAny<TaskBulkCommand>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await TaskBulkCommandService.ProcessBulkTasksAsync(tasks);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(taskId, result[0]);
            MockRabbitMQRepository.Verify(
                repo => repo.PublishAsync(It.Is<TaskBulkCommand>(c => c.Tasks.Count == 1 && c.Tasks[0].Id == taskId), "task-bulk-queue"),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessBulkTasksAsync_ValidTasks_CommandContainsCorrectTasks()
        {
            // Arrange
            var task1 = new TaskBulkRequest
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Task 1",
                Description = "Description 1"
            };
            var task2 = new TaskBulkRequest
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Task 2",
                Description = "Description 2"
            };
            var tasks = new List<TaskBulkRequest> { task1, task2 };

            TaskBulkCommand? capturedCommand = null;
            MockRabbitMQRepository
                .Setup(repo => repo.PublishAsync(It.IsAny<TaskBulkCommand>(), It.IsAny<string>()))
                .Callback<TaskBulkCommand, string>((cmd, queue) => capturedCommand = cmd)
                .Returns(Task.CompletedTask);

            // Act
            var result = await TaskBulkCommandService.ProcessBulkTasksAsync(tasks);

            // Assert
            Assert.IsNotNull(capturedCommand);
            Assert.AreEqual(2, capturedCommand.Tasks.Count);
            Assert.AreEqual(task1.Title, capturedCommand.Tasks[0].Title);
            Assert.AreEqual(task2.Title, capturedCommand.Tasks[1].Title);
            Assert.IsNotNull(capturedCommand.Id);
            Assert.IsTrue(capturedCommand.CreatedAt <= DateTime.UtcNow);
        }

        #endregion
    }
}


