using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Persistence;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.Tests.QueryHandlers
{
    public class GetTodoItemsWithPaginationQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetTodoItemsWithPaginationQueryHandler _handler;

        public GetTodoItemsWithPaginationQueryHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetTodoItemsWithPaginationQueryHandler(_mockContext.Object, _mockMapper.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithValidRequest_ReturnsPaginatedTodoItems()
        {
            // Arrange
            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Task 1", ListId = 1 },
                new TodoItem { Id = 2, Title = "Task 2", ListId = 1 },
                new TodoItem { Id = 3, Title = "Task 3", ListId = 1 }
            };

            _mockContext.Setup(ctx => ctx.TodoItems
                .Where(x => x.ListId == 1)
                .OrderBy(x => x.Title)
                .Skip(0)
                .Take(2)
                .ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoItems.Skip(0).Take(2).ToList());

            _mockMapper.Setup(mapper => mapper.Map<List<TodoItemBriefDto>>(todoItems.Skip(0).Take(2)))
                .Returns(todoItems.Skip(0).Take(2).Select(item => new TodoItemBriefDto { Id = item.Id, Title = item.Title }).ToList());

            var request = new GetTodoItemsWithPaginationQuery { ListId = 1, PageNumber = 1, PageSize = 2 };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Items.Count.ShouldBe(2);
            result.Items.First().Id.ShouldBe(1);
            result.Items.First().Title.ShouldBe("Task 1");
            result.Items.Last().Id.ShouldBe(2);
            result.Items.Last().Title.ShouldBe("Task 2");
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalPages.ShouldBe(2);
            result.TotalCount.ShouldBe(3);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithEmptyList_ReturnsEmptyPaginatedList()
        {
            // Arrange
            var todoItems = new List<TodoItem>();

            _mockContext.Setup(ctx => ctx.TodoItems
                .Where(x => x.ListId == 1)
                .OrderBy(x => x.Title)
                .Skip(0)
                .Take(2)
                .ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoItems.Skip(0).Take(2).ToList());

            _mockMapper.Setup(mapper => mapper.Map<List<TodoItemBriefDto>>(todoItems.Skip(0).Take(2)))
                .Returns(todoItems.Skip(0).Take(2).Select(item => new TodoItemBriefDto { Id = item.Id, Title = item.Title }).ToList());

            var request = new GetTodoItemsWithPaginationQuery { ListId = 1, PageNumber = 1, PageSize = 2 };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Items.Count.ShouldBe(0);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalPages.ShouldBe(1);
            result.TotalCount.ShouldBe(0);
        }

        [Fact]
        public async Task Handle_WithLargePageSize_ReturnsAllItems()
        {
            // Arrange
            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Task 1", ListId = 1 },
                new TodoItem { Id = 2, Title = "Task 2", ListId = 1 },
                new TodoItem { Id = 3, Title = "Task 3", ListId = 1 }
            };

            _mockContext.Setup(ctx => ctx.TodoItems
                .Where(x => x.ListId == 1)
                .OrderBy(x => x.Title)
                .Skip(0)
                .Take(3)
                .ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoItems.Skip(0).Take(3).ToList());

            _mockMapper.Setup(mapper => mapper.Map<List<TodoItemBriefDto>>(todoItems.Skip(0).Take(3)))
                .Returns(todoItems.Skip(0).Take(3).Select(item => new TodoItemBriefDto { Id = item.Id, Title = item.Title }).ToList());

            var request = new GetTodoItemsWithPaginationQuery { ListId = 1, PageNumber = 1, PageSize = 3 };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Items.Count.ShouldBe(3);
            result.Items.First().Id.ShouldBe(1);
            result.Items.First().Title.ShouldBe("Task 1");
            result.Items.Last().Id.ShouldBe(3);
            result.Items.Last().Title.ShouldBe("Task 3");
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(3);
            result.TotalPages.ShouldBe(1);
            result.TotalCount.ShouldBe(3);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithInvalidListId_ReturnsEmptyPaginatedList()
        {
            // Arrange
            var todoItems = new List<TodoItem>();

            _mockContext.Setup(ctx => ctx.TodoItems
                .Where(x => x.ListId == 2)
                .OrderBy(x => x.Title)
                .Skip(0)
                .Take(2)
                .ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoItems.Skip(0).Take(2).ToList());

            _mockMapper.Setup(mapper => mapper.Map<List<TodoItemBriefDto>>(todoItems.Skip(0).Take(2)))
                .Returns(todoItems.Skip(0).Take(2).Select(item => new TodoItemBriefDto { Id = item.Id, Title = item.Title }).ToList());

            var request = new GetTodoItemsWithPaginationQuery { ListId = 2, PageNumber = 1, PageSize = 2 };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Items.Count.ShouldBe(0);
            result.PageIndex.ShouldBe(1);
            result.PageSize.ShouldBe(2);
            result.TotalPages.ShouldBe(1);
            result.TotalCount.ShouldBe(0);
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenDatabaseThrowsException_RethrowsException()
        {
            // Arrange
            var request = new GetTodoItemsWithPaginationQuery { ListId = 1, PageNumber = 1, PageSize = 2 };

            _mockContext.Setup(ctx => ctx.TodoItems
                .Where(x => x.ListId == 1)
                .OrderBy(x => x.Title)
                .Skip(0)
                .Take(2)
                .ToListAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(request, CancellationToken.None));
        }

        #endregion
    }
}