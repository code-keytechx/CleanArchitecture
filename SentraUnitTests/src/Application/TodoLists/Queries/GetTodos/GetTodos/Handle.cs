using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Persistence;
using CleanArchitecture.Persistence.Entities;
using MediatR;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Tests.Application.Todos
{
    public class GetTodosQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetTodosQueryHandler _handler;

        public GetTodosQueryHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetTodosQueryHandler(_mockContext.Object, _mockMapper.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithValidRequest_ReturnsTodosVm()
        {
            // Arrange
            var todoLists = new List<TodoList>
            {
                new TodoList { Id = 1, Title = "Home Tasks" },
                new TodoList { Id = 2, Title = "Work Tasks" }
            };

            _mockContext.Setup(ctx => ctx.TodoLists.AsNoTracking())
                .Returns(todoLists.AsQueryable());

            _mockMapper.Setup(mapper => mapper.ProjectTo<TodoListDto>(It.IsAny<IQueryable<TodoList>>(), It.IsAny<IMappingConfigurationProvider>()))
                .Returns(todoLists.Select(tl => new TodoListDto { Id = tl.Id, Title = tl.Title }).AsQueryable());

            // Act
            var result = await _handler.Handle(new GetTodosQuery(), CancellationToken.None);

            // Assert
            result.PriorityLevels.ShouldNotBeEmpty();
            result.PriorityLevels.Count.ShouldBe(Enum.GetValues(typeof(PriorityLevel)).Length);
            result.Lists.ShouldNotBeEmpty();
            result.Lists.Count.ShouldBe(2);
            result.Lists.First().Title.ShouldBe("Home Tasks");
            result.Lists.Last().Title.ShouldBe("Work Tasks");
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithEmptyTodoLists_ReturnsEmptyLists()
        {
            // Arrange
            _mockContext.Setup(ctx => ctx.TodoLists.AsNoTracking())
                .Returns(new List<TodoList>().AsQueryable());

            // Act
            var result = await _handler.Handle(new GetTodosQuery(), CancellationToken.None);

            // Assert
            result.PriorityLevels.ShouldNotBeEmpty();
            result.PriorityLevels.Count.ShouldBe(Enum.GetValues(typeof(PriorityLevel)).Length);
            result.Lists.ShouldNotBeEmpty();
            result.Lists.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Handle_WithNullTodoLists_ReturnsDefaultLists()
        {
            // Arrange
            _mockContext.Setup(ctx => ctx.TodoLists.AsNoTracking())
                .Returns((IQueryable<TodoList>)null);

            // Act
            var result = await _handler.Handle(new GetTodosQuery(), CancellationToken.None);

            // Assert
            result.PriorityLevels.ShouldNotBeEmpty();
            result.PriorityLevels.Count.ShouldBe(Enum.GetValues(typeof(PriorityLevel)).Length);
            result.Lists.ShouldNotBeEmpty();
            result.Lists.Count.ShouldBe(0);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithInvalidRequest_ThrowsArgumentNullException()
        {
            // Arrange
            // No arrange needed for null request

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenDatabaseAccessFails_ThrowsDbUpdateException()
        {
            // Arrange
            _mockContext.Setup(ctx => ctx.TodoLists.AsNoTracking())
                .ThrowsAsync(new DbUpdateException("Database update failed"));

            // Act & Assert
            await Should.ThrowAsync<DbUpdateException>(() => _handler.Handle(new GetTodosQuery(), CancellationToken.None));
        }

        #endregion

        #region Helper Methods

        private static GetTodosQuery CreateValidRequest() => new GetTodosQuery();

        #endregion
    }
}