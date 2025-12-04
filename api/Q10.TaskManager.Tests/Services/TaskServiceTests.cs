using Moq;
using Q10.TaskManager.Application.Services;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Q10.TaskManager.Tests.Services
{
    [TestClass]
    public class TaskServiceTests
    {
        public Mock<ITaskRepository> MockTaskRepository { get; set; } = null!;
        public ITaskService TaskService { get; set; } = null!;

        [TestInitialize]
        public void Initialize()
        {
            MockTaskRepository = new Mock<ITaskRepository>();
            TaskService = new TaskService(MockTaskRepository.Object);
        }

        #region CreateTask Tests

        [TestMethod]
        public async Task CreateTask_ValidTask_ReturnsCreatedTask()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                Title = "Test",
                Description = "This is a test"
            };

            MockTaskRepository
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(taskItem);

            // Act
            var result = await TaskService.CreateTask(taskItem);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(taskItem.Id, result.Id);
            Assert.AreEqual(taskItem.Title, result.Title);
            Assert.AreEqual(taskItem.Description, result.Description);
            MockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateTask_NullTask_ThrowsArgumentNullException()
        {
            // Arrange
            TaskItem? taskItem = null;

            // Act & Assert
            try
            {
                await TaskService.CreateTask(taskItem!);
                Assert.Fail("Expected ArgumentNullException was not thrown");
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNotNull(ex);
                Assert.AreEqual("task", ex.ParamName);
            }

            MockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        #endregion

        #region DeleteTask Tests

        [TestMethod]
        public async Task DeleteTask_ExistingTask_ReturnsTrue()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            MockTaskRepository
                .Setup(repo => repo.DeleteTaskAsync(taskId))
                .ReturnsAsync(true);

            // Act
            var result = await TaskService.DeleteTask(taskId);

            // Assert
            Assert.IsTrue(result);
            MockTaskRepository.Verify(repo => repo.DeleteTaskAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteTask_NonExistingTask_ReturnsFalse()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            MockTaskRepository
                .Setup(repo => repo.DeleteTaskAsync(taskId))
                .ReturnsAsync(false);

            // Act
            var result = await TaskService.DeleteTask(taskId);

            // Assert
            Assert.IsFalse(result);
            MockTaskRepository.Verify(repo => repo.DeleteTaskAsync(taskId), Times.Once);
        }

        #endregion

        #region GetAllTasks Tests

        [TestMethod]
        public async Task GetAllTasks_WithTasks_ReturnsAllTasks()
        {
            // Arrange
            var taskList = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid().ToString(), Title = "Task 1", Description = "Description 1" },
                new TaskItem { Id = Guid.NewGuid().ToString(), Title = "Task 2", Description = "Description 2" },
                new TaskItem { Id = Guid.NewGuid().ToString(), Title = "Task 3", Description = "Description 3" }
            }.AsQueryable();

            MockTaskRepository
                .Setup(repo => repo.GetAllTasksAsync())
                .ReturnsAsync(taskList);

            // Act
            var result = await TaskService.GetAllTasks();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(t => t.Title == "Task 1"));
            Assert.IsTrue(result.Any(t => t.Title == "Task 2"));
            Assert.IsTrue(result.Any(t => t.Title == "Task 3"));
            MockTaskRepository.Verify(repo => repo.GetAllTasksAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllTasks_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<TaskItem>().AsQueryable();
            MockTaskRepository
                .Setup(repo => repo.GetAllTasksAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await TaskService.GetAllTasks();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            MockTaskRepository.Verify(repo => repo.GetAllTasksAsync(), Times.Once);
        }

        #endregion

        #region GetTaskById Tests

        [TestMethod]
        public async Task GetTaskById_ExistingTask_ReturnsTask()
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
            var result = await TaskService.GetTaskById(taskId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(taskId, result.Id);
            Assert.AreEqual("Test Task", result.Title);
            Assert.AreEqual("Test Description", result.Description);
            MockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }

        [TestMethod]
        public async Task GetTaskById_NonExistingTask_ReturnsNull()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
#pragma warning disable CS8620 // Nullability mismatch - intentional for testing null return scenario
            MockTaskRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);
#pragma warning restore CS8620

            // Act
            var result = await TaskService.GetTaskById(taskId);

            // Assert
            Assert.IsNull(result);
            MockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }

        #endregion

        #region UpdateTask Tests

        [TestMethod]
        public async Task UpdateTask_ExistingTask_ReturnsUpdatedTask()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var originalTask = new TaskItem
            {
                Id = taskId,
                Title = "Original Title",
                Description = "Original Description"
            };

            var updatedTask = new TaskItem
            {
                Id = taskId,
                Title = "Updated Title",
                Description = "Updated Description"
            };

            MockTaskRepository
                .Setup(repo => repo.UpdateTaskAsync(taskId, It.IsAny<TaskItem>()))
                .ReturnsAsync(updatedTask);

            // Act
            var result = await TaskService.UpdateTask(taskId, updatedTask);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(taskId, result.Id);
            Assert.AreEqual("Updated Title", result.Title);
            Assert.AreEqual("Updated Description", result.Description);
            MockTaskRepository.Verify(repo => repo.UpdateTaskAsync(taskId, It.IsAny<TaskItem>()), Times.Once);
        }

        #endregion
    }
}
