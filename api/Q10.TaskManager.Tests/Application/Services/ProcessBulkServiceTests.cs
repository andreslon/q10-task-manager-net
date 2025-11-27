using Moq;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Tests.Application.Services
{
    [TestClass]
    public class ProcessBulkServiceTests
    {
        private Mock<ITaskRepository> _mockTaskRepository;
        private Mock<IRabbitMQRepository> _mockRabbitMQRepository;
        private ProcessBulkService _processBulkService;

        [TestInitialize]
        public void Setup()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockRabbitMQRepository = new Mock<IRabbitMQRepository>();
            _processBulkService = new ProcessBulkService(
                _mockTaskRepository.Object, 
                _mockRabbitMQRepository.Object);
        }

        #region StartConsumingAsync Tests (Positive)

        [TestMethod]
        public async Task StartConsumingAsync_CallsRabbitMQRepository()
        {
            // Arrange
            _mockRabbitMQRepository
                .Setup(repo => repo.StartConsumingAsync<TaskBulkCommand>(
                    It.IsAny<string>(), 
                    It.IsAny<Func<TaskBulkCommand, Task>>()))
                .Returns(Task.CompletedTask);

            // Act
            await _processBulkService.StartConsumingAsync();

            // Assert
            _mockRabbitMQRepository.Verify(repo => repo.StartConsumingAsync<TaskBulkCommand>(
                "task-bulk-queue",
                It.IsAny<Func<TaskBulkCommand, Task>>()), Times.Once);
        }

        #endregion

        #region ProcessBulkCommand Tests (Positive)

        [TestMethod]
        public async Task ProcessBulkCommand_ValidTasks_CreatesAllTasks()
        {
            // Arrange
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>
                {
                    new TaskBulkRequest { Id = "task-1", Title = "Task 1", Description = "Description 1" },
                    new TaskBulkRequest { Id = "task-2", Title = "Task 2", Description = "Description 2" }
                }
            };

            _mockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem task) => Task.FromResult(task).Result);

            // Act
            await _processBulkService.ProcessBulkCommand(command);

            // Assert
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task ProcessBulkCommand_ValidTasks_CreatesTasksWithCorrectProperties()
        {
            // Arrange
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>
                {
                    new TaskBulkRequest { Id = "task-1", Title = "Task 1", Description = "Description 1" }
                }
            };

            _mockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem task) => Task.FromResult(task).Result);

            // Act
            await _processBulkService.ProcessBulkCommand(command);

            // Assert
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.Is<TaskItem>(t => 
                t.Id == "task-1" && 
                t.Title == "Task 1" && 
                t.Description == "Description 1")), Times.Once);
        }

        [TestMethod]
        public async Task ProcessBulkCommand_EmptyTaskList_DoesNotCreateTasks()
        {
            // Arrange
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>()
            };

            // Act
            await _processBulkService.ProcessBulkCommand(command);

            // Assert
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [TestMethod]
        public async Task ProcessBulkCommand_SingleTask_CreatesOneTask()
        {
            // Arrange
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>
                {
                    new TaskBulkRequest { Id = "task-1", Title = "Task 1", Description = "Description 1" }
                }
            };

            _mockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem task) => Task.FromResult(task).Result);

            // Act
            await _processBulkService.ProcessBulkCommand(command);

            // Assert
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        #endregion

        #region ProcessBulkCommand Tests (Negative - Exception Handling)

        [TestMethod]
        public async Task ProcessBulkCommand_RepositoryThrowsException_ContinuesProcessing()
        {
            // Arrange
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>
                {
                    new TaskBulkRequest { Id = "task-1", Title = "Task 1", Description = "Description 1" },
                    new TaskBulkRequest { Id = "task-2", Title = "Task 2", Description = "Description 2" }
                }
            };

            var callCount = 0;
            _mockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .Returns<TaskItem>(task =>
                {
                    callCount++;
                    if (callCount == 1)
                        throw new Exception("Database error");
                    return Task.FromResult(task);
                });

            // Act
            await _processBulkService.ProcessBulkCommand(command);

            // Assert
            // Should attempt to create both tasks even if first one fails
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task ProcessBulkCommand_AllTasksFail_StillProcessesAll()
        {
            // Arrange
            var command = new TaskBulkCommand
            {
                Tasks = new List<TaskBulkRequest>
                {
                    new TaskBulkRequest { Id = "task-1", Title = "Task 1", Description = "Description 1" },
                    new TaskBulkRequest { Id = "task-2", Title = "Task 2", Description = "Description 2" }
                }
            };

            _mockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            await _processBulkService.ProcessBulkCommand(command);

            // Assert
            // Should attempt to create both tasks even if all fail
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task ProcessBulkCommand_NullCommand_ThrowsArgumentNullException()
        {
            // Arrange
            TaskBulkCommand? command = null;

            // Act & Assert
            try
            {
                await _processBulkService.ProcessBulkCommand(command!);
                Assert.Fail("Expected NullReferenceException was not thrown");
            }
            catch (NullReferenceException)
            {
                // Expected exception
            }
        }

        #endregion
    }
}

