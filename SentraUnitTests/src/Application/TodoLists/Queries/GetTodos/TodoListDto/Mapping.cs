using AutoMapper;
using CleanArchitecture.Domain.Entities;
using Shouldly;
using Xunit;

namespace CleanArchitecture.Tests.Mapping
{
    public class TodoListDtoMappingTests
    {
        private readonly IMapper _mapper;

        public TodoListDtoMappingTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<Mapping>();
            });

            _mapper = config.CreateMapper();
        }

        #region Happy Path Tests

        [Fact]
        public void Map_TodoListToTodoListDto_ShouldReturnCorrectDto()
        {
            // Arrange
            var todoList = new TodoList
            {
                Id = 1,
                Title = "Grocery Shopping",
                Description = "Buy milk, bread, and eggs.",
                CreatedBy = "user123",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "user123",
                UpdatedDate = DateTime.UtcNow
            };

            // Act
            var dto = _mapper.Map<TodoListDto>(todoList);

            // Assert
            dto.Id.ShouldBe(todoList.Id);
            dto.Title.ShouldBe(todoList.Title);
            dto.Description.ShouldBe(todoList.Description);
            dto.CreatedBy.ShouldBe(todoList.CreatedBy);
            dto.CreatedDate.ShouldBe(todoList.CreatedDate);
            dto.UpdatedBy.ShouldBe(todoList.UpdatedBy);
            dto.UpdatedDate.ShouldBe(todoList.UpdatedDate);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Map_EmptyTitle_TodoListDto_ShouldReturnEmptyTitle()
        {
            // Arrange
            var todoList = new TodoList
            {
                Id = 1,
                Title = "",
                Description = "Buy milk, bread, and eggs.",
                CreatedBy = "user123",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "user123",
                UpdatedDate = DateTime.UtcNow
            };

            // Act
            var dto = _mapper.Map<TodoListDto>(todoList);

            // Assert
            dto.Title.ShouldBe(string.Empty);
        }

        [Fact]
        public void Map_NullDescription_TodoListDto_ShouldReturnNullDescription()
        {
            // Arrange
            var todoList = new TodoList
            {
                Id = 1,
                Title = "Grocery Shopping",
                Description = null,
                CreatedBy = "user123",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "user123",
                UpdatedDate = DateTime.UtcNow
            };

            // Act
            var dto = _mapper.Map<TodoListDto>(todoList);

            // Assert
            dto.Description.ShouldBeNull();
        }

        #endregion

        #region Negative Tests

        [Fact]
        public void Map_NullTodoList_ShouldThrowArgumentNullException()
        {
            // Arrange
            TodoList todoList = null;

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => _mapper.Map<TodoListDto>(todoList));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public void Map_WhenMappingFails_ShouldThrowAutoMapperMappingException()
        {
            // Arrange
            var todoList = new TodoList
            {
                Id = 1,
                Title = "Grocery Shopping",
                Description = "Buy milk, bread, and eggs.",
                CreatedBy = "user123",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "user123",
                UpdatedDate = DateTime.UtcNow
            };

            // Mock a failing mapping scenario
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TodoList, TodoListDto>().ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Description));
            });

            _mapper = config.CreateMapper();

            // Act & Assert
            Should.Throw<AutoMapperMappingException>(() => _mapper.Map<TodoListDto>(todoList));
        }

        #endregion

        #region Helper Methods

        private static TodoList CreateValidTodoList()
        {
            return new TodoList
            {
                Id = 1,
                Title = "Grocery Shopping",
                Description = "Buy milk, bread, and eggs.",
                CreatedBy = "user123",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "user123",
                UpdatedDate = DateTime.UtcNow
            };
        }

        #endregion
    }
}