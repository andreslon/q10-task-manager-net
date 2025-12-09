using Moq;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Q10.TaskManager.Tests.Services
{
    [TestClass]
    public class ProcessBulkServiceTests
    {
        public Mock<ITaskRepository> MockTaskRepository { get; set; } = null!;
        public Mock<IRabbitMQRepository> MockRabbitMQRepository { get; set; } = null!;
        public IProcessBulkService ProcessBulkService { get; set; } = null!;

        [TestInitialize]
        public void Initialize()
        {
            MockTaskRepository = new Mock<ITaskRepository>();
            MockRabbitMQRepository = new Mock<IRabbitMQRepository>();
            ProcessBulkService = new ProcessBulkService(MockTaskRepository.Object, MockRabbitMQRepository.Object);
        }

        #region ProcessBulkCommand Tests

        [TestMethod]
        public async Task ProcessBulkCommand_ValidTasks_CreatesAllTasks()
        {
            // Arrange
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>
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
                    }
                }
            };

            MockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem task) => task);

            // Act
            await ProcessBulkService.ProcessBulkCommand(command);

            // Assert
            MockTaskRepository.Verify(
                repo => repo.CreateTaskAsync(It.Is<TaskItem>(t => t.Title == "Task 1")),
                Times.Once);
            MockTaskRepository.Verify(
                repo => repo.CreateTaskAsync(It.Is<TaskItem>(t => t.Title == "Task 2")),
                Times.Once);
        }

        [TestMethod]
        public async Task ProcessBulkCommand_EmptyTaskList_DoesNotCreateAnyTasks()
        {
            // Arrange
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>()
            };

            // Act
            await ProcessBulkService.ProcessBulkCommand(command);

            // Assert
            MockTaskRepository.Verify(
                repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ProcessBulkCommand_TaskCreationFails_ContinuesProcessingOtherTasks()
        {
            // Arrange
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>
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
                }
            };

            var callCount = 0;
            MockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem task) =>
                {
                    callCount++;
                    if (callCount == 2) // Fail on second task
                    {
                        throw new Exception("Database error");
                    }
                    return task;
                });

            // Act
            await ProcessBulkService.ProcessBulkCommand(command);

            // Assert
            // Should attempt to create all 3 tasks, even though one fails
            MockTaskRepository.Verify(
                repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()),
                Times.Exactly(3));
        }

        [TestMethod]
        public async Task ProcessBulkCommand_ValidTask_CreatesTaskWithCorrectProperties()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>
                {
                    new TaskBulkRequest
                    {
                        Id = taskId,
                        Title = "Test Task",
                        Description = "Test Description"
                    }
                }
            };

            TaskItem? capturedTask = null;
            MockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .Callback<TaskItem>(task => capturedTask = task)
                .ReturnsAsync((TaskItem task) => task);

            // Act
            await ProcessBulkService.ProcessBulkCommand(command);

            // Assert
            Assert.IsNotNull(capturedTask);
            Assert.AreEqual(taskId, capturedTask.Id);
            Assert.AreEqual("Test Task", capturedTask.Title);
            Assert.AreEqual("Test Description", capturedTask.Description);
        }

        [TestMethod]
        public async Task ProcessBulkCommand_SingleTask_CreatesTaskSuccessfully()
        {
            // Arrange
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>
                {
                    new TaskBulkRequest
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Single Task",
                        Description = "Single Description"
                    }
                }
            };

            MockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem task) => task);

            // Act
            await ProcessBulkService.ProcessBulkCommand(command);

            // Assert
            MockTaskRepository.Verify(
                repo => repo.CreateTaskAsync(It.Is<TaskItem>(t => t.Title == "Single Task")),
                Times.Once);
        }

        #endregion

        #region StartConsumingAsync Tests

        [TestMethod]
        public async Task StartConsumingAsync_CallsRabbitMQRepository()
        {
            // Arrange
            MockRabbitMQRepository
                .Setup(repo => repo.StartConsumingAsync<TaskBulkCommand>(
                    It.IsAny<string>(),
                    It.IsAny<Func<TaskBulkCommand, Task>>()))
                .Returns(Task.CompletedTask);

            // Act
            await ProcessBulkService.StartConsumingAsync();

            // Assert
            MockRabbitMQRepository.Verify(
                repo => repo.StartConsumingAsync<TaskBulkCommand>(
                    "task-bulk-queue",
                    It.IsAny<Func<TaskBulkCommand, Task>>()),
                Times.Once);
        }

        #endregion
    }
}


