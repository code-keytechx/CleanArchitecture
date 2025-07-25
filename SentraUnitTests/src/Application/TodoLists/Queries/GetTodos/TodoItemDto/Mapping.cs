using AutoMapper;
using CleanArchitecture.Domain.Entities;
using Shouldly;
using Xunit;

namespace CleanArchitecture.Tests.Mapping
{
    public class TodoItemDtoMappingTests
    {
        private readonly IMapper _mapper;

        public TodoItemDtoMappingTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<Mapping>();
            });

            _mapper = config.CreateMapper();
        }

        #region Happy Path Tests

        [Fact]
        public void Map_TodoItemToTodoItemDto_ShouldMapPriorityCorrectly()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Title = "Complete project report",
                Description = "Review and finalize the project report",
                Priority = Domain.Enums.TodoItemPriority.High
            };

            // Act
            var dto = _mapper.Map<TodoItemDto>(todoItem);

            // Assert
            dto.Title.ShouldBe(todoItem.Title);
            dto.Description.ShouldBe(todoItem.Description);
            dto.Priority.ShouldBe((int)todoItem.Priority);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Map_TodoItemWithNullTitle_ShouldMapEmptyString()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Title = null,
                Description = "Review and finalize the project report",
                Priority = Domain.Enums.TodoItemPriority.Medium
            };

            // Act
            var dto = _mapper.Map<TodoItemDto>(todoItem);

            // Assert
            dto.Title.ShouldBe(string.Empty);
            dto.Description.ShouldBe(todoItem.Description);
            dto.Priority.ShouldBe((int)todoItem.Priority);
        }

        [Fact]
        public void Map_TodoItemWithNullDescription_ShouldMapEmptyString()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Title = "Complete project report",
                Description = null,
                Priority = Domain.Enums.TodoItemPriority.Low
            };

            // Act
            var dto = _mapper.Map<TodoItemDto>(todoItem);

            // Assert
            dto.Title.ShouldBe(todoItem.Title);
            dto.Description.ShouldBe(string.Empty);
            dto.Priority.ShouldBe((int)todoItem.Priority);
        }

        [Fact]
        public void Map_TodoItemWithDefaultPriority_ShouldMapZero()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Title = "Complete project report",
                Description = "Review and finalize the project report",
                Priority = default(Domain.Enums.TodoItemPriority)
            };

            // Act
            var dto = _mapper.Map<TodoItemDto>(todoItem);

            // Assert
            dto.Title.ShouldBe(todoItem.Title);
            dto.Description.ShouldBe(todoItem.Description);
            dto.Priority.ShouldBe(0);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public void Map_TodoItemWithInvalidPriority_ShouldThrowArgumentException()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Title = "Complete project report",
                Description = "Review and finalize the project report",
                Priority = (Domain.Enums.TodoItemPriority)(-1)
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => _mapper.Map<TodoItemDto>(todoItem));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public void Map_NullTodoItem_ShouldThrowArgumentNullException()
        {
            // Arrange
            TodoItem todoItem = null;

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => _mapper.Map<TodoItemDto>(todoItem));
        }

        #endregion

        #region Helper Methods

        private static TodoItem CreateTodoItem(string title, string description, Domain.Enums.TodoItemPriority priority)
        {
            return new TodoItem
            {
                Title = title,
                Description = description,
                Priority = priority
            };
        }

        #endregion
    }
}