using AutoMapper;
using CleanArchitecture.Domain.Entities;
using Shouldly;
using Xunit;

namespace CleanArchitecture.Tests.Mapping
{
    public class TodoItemBriefDtoMappingTests
    {
        private readonly IMapper _mapper;

        public TodoItemBriefDtoMappingTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TodoItemBriefDto>();
            });

            _mapper = config.CreateMapper();
        }

        #region Happy Path Tests

        [Fact]
        public void Map_TodoItemToTodoItemBriefDto_ShouldReturnCorrectValues()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Complete project report",
                Description = "Review and finalize the project report.",
                DueDate = DateTime.UtcNow.AddDays(5),
                Priority = Priority.Medium,
                IsCompleted = false
            };

            // Act
            var dto = _mapper.Map<TodoItemBriefDto>(todoItem);

            // Assert
            dto.Id.ShouldBe(todoItem.Id);
            dto.Title.ShouldBe(todoItem.Title);
            dto.Description.ShouldBe(todoItem.Description);
            dto.DueDate.ShouldBe(todoItem.DueDate);
            dto.Priority.ShouldBe(todoItem.Priority);
            dto.IsCompleted.ShouldBe(todoItem.IsCompleted);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Map_TodoItemWithNullTitle_ShouldMapEmptyString()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = null,
                Description = "Review and finalize the project report.",
                DueDate = DateTime.UtcNow.AddDays(5),
                Priority = Priority.Medium,
                IsCompleted = false
            };

            // Act
            var dto = _mapper.Map<TodoItemBriefDto>(todoItem);

            // Assert
            dto.Title.ShouldBe(string.Empty);
        }

        [Fact]
        public void Map_TodoItemWithNullDescription_ShouldMapEmptyString()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Complete project report",
                Description = null,
                DueDate = DateTime.UtcNow.AddDays(5),
                Priority = Priority.Medium,
                IsCompleted = false
            };

            // Act
            var dto = _mapper.Map<TodoItemBriefDto>(todoItem);

            // Assert
            dto.Description.ShouldBe(string.Empty);
        }

        [Fact]
        public void Map_TodoItemWithNullDueDate_ShouldMapDefaultDateTime()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Complete project report",
                Description = "Review and finalize the project report.",
                DueDate = null,
                Priority = Priority.Medium,
                IsCompleted = false
            };

            // Act
            var dto = _mapper.Map<TodoItemBriefDto>(todoItem);

            // Assert
            dto.DueDate.ShouldBeNull();
        }

        #endregion

        #region Negative Tests

        [Fact]
        public void Map_NullTodoItem_ShouldThrowArgumentNullException()
        {
            // Arrange
            TodoItem todoItem = null;

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => _mapper.Map<TodoItemBriefDto>(todoItem));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public void Map_TodoItemWithInvalidPriority_ShouldThrowArgumentException()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Complete project report",
                Description = "Review and finalize the project report.",
                DueDate = DateTime.UtcNow.AddDays(5),
                Priority = (Priority)(int.MaxValue), // Invalid priority
                IsCompleted = false
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => _mapper.Map<TodoItemBriefDto>(todoItem));
        }

        #endregion
    }
}