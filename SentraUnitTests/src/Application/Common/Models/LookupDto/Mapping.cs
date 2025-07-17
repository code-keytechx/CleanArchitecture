using AutoMapper;
using CleanArchitecture.Domain.Entities;
using Shouldly;
using Xunit;

namespace CleanArchitecture.Tests.Mapping
{
    public class LookupDtoTests
    {
        private readonly IMapper _mapper;

        public LookupDtoTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<Mapping>();
            });

            _mapper = config.CreateMapper();
        }

        #region Happy Path Tests

        [Fact]
        public void Map_TodoList_To_LookupDto_Should_Succeed()
        {
            // Arrange
            var todoList = new TodoList
            {
                Id = 1,
                Title = "Grocery Shopping",
                CreatedBy = "user1",
                CreatedDate = DateTime.UtcNow,
                Items = new List<TodoItem>
                {
                    new TodoItem { Id = 1, Title = "Milk", Done = false },
                    new TodoItem { Id = 2, Title = "Eggs", Done = true }
                }
            };

            // Act
            var lookupDto = _mapper.Map<LookupDto>(todoList);

            // Assert
            lookupDto.Id.ShouldBe(todoList.Id);
            lookupDto.Title.ShouldBe(todoList.Title);
            lookupDto.CreatedBy.ShouldBe(todoList.CreatedBy);
            lookupDto.CreatedDate.ShouldBe(todoList.CreatedDate);
            lookupDto.Items.ShouldHaveSingleItem();
            lookupDto.Items.First().Id.ShouldBe(todoList.Items.First().Id);
            lookupDto.Items.First().Title.ShouldBe(todoList.Items.First().Title);
            lookupDto.Items.First().Done.ShouldBe(todoList.Items.First().Done);
        }

        [Fact]
        public void Map_TodoItem_To_LookupDto_Should_Succeed()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Buy Milk",
                Done = false,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var lookupDto = _mapper.Map<LookupDto>(todoItem);

            // Assert
            lookupDto.Id.ShouldBe(todoItem.Id);
            lookupDto.Title.ShouldBe(todoItem.Title);
            lookupDto.Done.ShouldBe(todoItem.Done);
            lookupDto.DueDate.ShouldBe(todoItem.DueDate);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Map_EmptyTodoList_To_LookupDto_Should_Succeed()
        {
            // Arrange
            var todoList = new TodoList
            {
                Id = 1,
                Title = "Grocery Shopping",
                CreatedBy = "user1",
                CreatedDate = DateTime.UtcNow,
                Items = new List<TodoItem>()
            };

            // Act
            var lookupDto = _mapper.Map<LookupDto>(todoList);

            // Assert
            lookupDto.Id.ShouldBe(todoList.Id);
            lookupDto.Title.ShouldBe(todoList.Title);
            lookupDto.CreatedBy.ShouldBe(todoList.CreatedBy);
            lookupDto.CreatedDate.ShouldBe(todoList.CreatedDate);
            lookupDto.Items.ShouldNotHaveItems();
        }

        [Fact]
        public void Map_TodoItem_WithNullDueDate_To_LookupDto_Should_Succeed()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Buy Milk",
                Done = false,
                DueDate = null
            };

            // Act
            var lookupDto = _mapper.Map<LookupDto>(todoItem);

            // Assert
            lookupDto.Id.ShouldBe(todoItem.Id);
            lookupDto.Title.ShouldBe(todoItem.Title);
            lookupDto.Done.ShouldBe(todoItem.Done);
            lookupDto.DueDate.ShouldBeNull();
        }

        #endregion

        #region Negative Tests

        [Fact]
        public void Map_NullTodoList_To_LookupDto_Should_ThrowArgumentNullException()
        {
            // Arrange
            TodoList todoList = null;

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => _mapper.Map<LookupDto>(todoList));
        }

        [Fact]
        public void Map_NullTodoItem_To_LookupDto_Should_ThrowArgumentNullException()
        {
            // Arrange
            TodoItem todoItem = null;

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => _mapper.Map<LookupDto>(todoItem));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public void Map_TodoList_WithInvalidCreatedBy_Should_ThrowArgumentException()
        {
            // Arrange
            var todoList = new TodoList
            {
                Id = 1,
                Title = "Grocery Shopping",
                CreatedBy = "",
                CreatedDate = DateTime.UtcNow,
                Items = new List<TodoItem>()
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => _mapper.Map<LookupDto>(todoList));
        }

        [Fact]
        public void Map_TodoItem_WithEmptyTitle_Should_ThrowArgumentException()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "",
                Done = false,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => _mapper.Map<LookupDto>(todoItem));
        }

        #endregion
    }
}