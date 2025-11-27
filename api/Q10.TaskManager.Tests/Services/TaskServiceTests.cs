using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Domain.Interfaces;

namespace Q10.TaskManager.Tests.Services
{
    [TestClass]
    public class TaskServiceTests
    {
        private Mock<ITaskRepository> _mockTaskRepository;
        private TaskService _taskService;

        [TestInitialize]
        public void Setup()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _taskService = new TaskService(_mockTaskRepository.Object);
        }

        [TestMethod]
        public async Task CreateTask_ValidTask_ReturnsTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Test Task",
                Description = "Test Description"
            };

            _mockTaskRepository.Setup(r => r.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(task);

            // Act
            var result = await _taskService.CreateTask(task);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(task.Title, result.Title);
            _mockTaskRepository.Verify(r => r.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateTask_NullTask_ThrowsException()
        {
            // Act & Assert
            try
            {
                await _taskService.CreateTask(null!);
                Assert.Fail("Expected ArgumentNullException was not thrown");
            }
            catch (ArgumentNullException)
            {
                // Expected exception
            }
        }

        [TestMethod]
        public async Task CreateTask_EmptyTitle_ThrowsException()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = string.Empty,
                Description = "Test Description"
            };

            // Act & Assert
            try
            {
                await _taskService.CreateTask(task);
                Assert.Fail("Expected ArgumentException was not thrown");
            }
            catch (ArgumentException)
            {
                // Expected exception
            }
        }

        [TestMethod]
        public async Task GetTaskById_ValidId_ReturnsTask()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description"
            };

            _mockTaskRepository.Setup(r => r.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _taskService.GetTaskById(taskId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(taskId, result.Id);
            _mockTaskRepository.Verify(r => r.GetTaskByIdAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task GetTaskById_InvalidId_ReturnsNull()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            _mockTaskRepository.Setup(r => r.GetTaskByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _taskService.GetTaskById(taskId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAllTasks_ReturnsTasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid().ToString(), Title = "Task 1" },
                new TaskItem { Id = Guid.NewGuid().ToString(), Title = "Task 2" }
            }.AsQueryable();

            _mockTaskRepository.Setup(r => r.GetAllTasksAsync())
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasks();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task DeleteTask_ValidId_ReturnsTrue()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            _mockTaskRepository.Setup(r => r.DeleteTaskAsync(taskId))
                .ReturnsAsync(true);

            // Act
            var result = await _taskService.DeleteTask(taskId);

            // Assert
            Assert.IsTrue(result);
            _mockTaskRepository.Verify(r => r.DeleteTaskAsync(taskId), Times.Once);
        }
    }
}

