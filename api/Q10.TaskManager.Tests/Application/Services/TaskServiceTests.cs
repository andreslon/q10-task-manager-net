using Moq;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Tests.Application.Services
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

        #region CreateTask Tests (Positive)

        [TestMethod]
        public async Task CreateTask_ValidTask_ReturnsCreatedTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Test Task",
                Description = "Test Description"
            };
            var createdTask = new TaskItem
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Test Task",
                Description = "Test Description"
            };

            _mockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(createdTask);

            // Act
            var result = await _taskService.CreateTask(task);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(createdTask.Id, result.Id);
            Assert.AreEqual(createdTask.Title, result.Title);
            Assert.AreEqual(createdTask.Description, result.Description);
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateTask_ValidTask_CallsRepositoryOnce()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Test Task",
                Description = "Test Description"
            };
            var createdTask = new TaskItem
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Test Task",
                Description = "Test Description"
            };

            _mockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(createdTask);

            // Act
            await _taskService.CreateTask(task);

            // Assert
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.Is<TaskItem>(t => 
                t.Title == task.Title && t.Description == task.Description)), Times.Once);
        }

        #endregion

        #region CreateTask Tests (Negative)

        [TestMethod]
        public async Task CreateTask_NullTask_ThrowsArgumentNullException()
        {
            // Arrange
            TaskItem? task = null;

            // Act & Assert
            try
            {
                await _taskService.CreateTask(task!);
                Assert.Fail("Expected ArgumentNullException was not thrown");
            }
            catch (ArgumentNullException)
            {
                // Expected exception
            }
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateTask_EmptyDescription_ThrowsArgumentException()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Test Task",
                Description = string.Empty
            };

            // Act & Assert
            try
            {
                await _taskService.CreateTask(task);
                Assert.Fail("Expected ArgumentException was not thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Task description cannot be null or empty", ex.Message);
            }
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateTask_NullDescription_ThrowsArgumentException()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Test Task",
                Description = null!
            };

            // Act & Assert
            try
            {
                await _taskService.CreateTask(task);
                Assert.Fail("Expected ArgumentException was not thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Task description cannot be null or empty", ex.Message);
            }
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        #endregion

        #region GetAllTasks Tests (Positive)

        [TestMethod]
        public async Task GetAllTasks_ReturnsAllTasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = "1", Title = "Task 1", Description = "Description 1" },
                new TaskItem { Id = "2", Title = "Task 2", Description = "Description 2" },
                new TaskItem { Id = "3", Title = "Task 3", Description = "Description 3" }
            }.AsQueryable();

            _mockTaskRepository
                .Setup(repo => repo.GetAllTasksAsync())
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasks();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            _mockTaskRepository.Verify(repo => repo.GetAllTasksAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllTasks_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var tasks = new List<TaskItem>().AsQueryable();

            _mockTaskRepository
                .Setup(repo => repo.GetAllTasksAsync())
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasks();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllTasks_FiltersNullTasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = "1", Title = "Task 1", Description = "Description 1" },
                new TaskItem { Id = "2", Title = "Task 2", Description = "Description 2" }
            }.AsQueryable();

            _mockTaskRepository
                .Setup(repo => repo.GetAllTasksAsync())
                .ReturnsAsync(tasks!);

            // Act
            var result = await _taskService.GetAllTasks();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        #endregion

        #region GetTaskById Tests (Positive)

        [TestMethod]
        public async Task GetTaskById_ValidId_ReturnsTask()
        {
            // Arrange
            var taskId = "test-id";
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
            var result = await _taskService.GetTaskById(taskId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(taskId, result.Id);
            Assert.AreEqual(task.Title, result.Title);
            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task GetTaskById_NonExistentId_ReturnsNull()
        {
            // Arrange
            var taskId = "non-existent-id";

            _mockTaskRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync((TaskItem)null!);

            // Act
            var result = await _taskService.GetTaskById(taskId);

            // Assert
            Assert.IsNull(result);
            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }

        #endregion

        #region UpdateTask Tests (Positive)

        [TestMethod]
        public async Task UpdateTask_ValidTask_ReturnsUpdatedTask()
        {
            // Arrange
            var taskId = "test-id";
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Updated Task",
                Description = "Updated Description"
            };
            var updatedTask = new TaskItem
            {
                Id = taskId,
                Title = "Updated Task",
                Description = "Updated Description"
            };

            _mockTaskRepository
                .Setup(repo => repo.UpdateTaskAsync(taskId, It.IsAny<TaskItem>()))
                .ReturnsAsync(updatedTask);

            // Act
            var result = await _taskService.UpdateTask(taskId, task);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(updatedTask.Id, result.Id);
            Assert.AreEqual(updatedTask.Title, result.Title);
            _mockTaskRepository.Verify(repo => repo.UpdateTaskAsync(taskId, It.IsAny<TaskItem>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateTask_ValidTask_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var taskId = "test-id";
            var task = new TaskItem
            {
                Title = "Updated Task",
                Description = "Updated Description"
            };
            var updatedTask = new TaskItem
            {
                Id = taskId,
                Title = "Updated Task",
                Description = "Updated Description"
            };

            _mockTaskRepository
                .Setup(repo => repo.UpdateTaskAsync(taskId, It.IsAny<TaskItem>()))
                .ReturnsAsync(updatedTask);

            // Act
            await _taskService.UpdateTask(taskId, task);

            // Assert
            _mockTaskRepository.Verify(repo => repo.UpdateTaskAsync(
                taskId, 
                It.Is<TaskItem>(t => t.Title == task.Title && t.Description == task.Description)), 
                Times.Once);
        }

        #endregion

        #region DeleteTask Tests (Positive)

        [TestMethod]
        public async Task DeleteTask_ValidId_ReturnsTrue()
        {
            // Arrange
            var taskId = "test-id";

            _mockTaskRepository
                .Setup(repo => repo.DeleteTaskAsync(taskId))
                .ReturnsAsync(true);

            // Act
            var result = await _taskService.DeleteTask(taskId);

            // Assert
            Assert.IsTrue(result);
            _mockTaskRepository.Verify(repo => repo.DeleteTaskAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteTask_NonExistentId_ReturnsFalse()
        {
            // Arrange
            var taskId = "non-existent-id";

            _mockTaskRepository
                .Setup(repo => repo.DeleteTaskAsync(taskId))
                .ReturnsAsync(false);

            // Act
            var result = await _taskService.DeleteTask(taskId);

            // Assert
            Assert.IsFalse(result);
            _mockTaskRepository.Verify(repo => repo.DeleteTaskAsync(taskId), Times.Once);
        }

        #endregion
    }
}

