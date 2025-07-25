using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using Shouldly;
using Xunit;

namespace CleanArchitecture.Application.Tests.TodoLists.Queries.GetTodos;

public class TodosVmTests
{
    // Test data - varied and realistic
    private readonly List<LookupDto> _priorityLevels = new()
    {
        new LookupDto { Id = 1, Title = "High" },
        new LookupDto { Id = 2, Title = "Medium" },
        new LookupDto { Id = 3, Title = "Low" }
    };

    private readonly List<TodoListDto> _todoLists = new()
    {
        new TodoListDto { Id = 1, Title = "Work", Color = "#FF0000" },
        new TodoListDto { Id = 2, Title = "Personal", Color = "#00FF00" }
    };

    // Mock declarations
    private readonly Mock<ITodoListService> _mockTodoListService;

    // System under test
    private readonly GetTodosQueryHandler _sut;

    // Constructor with setup
    public TodosVmTests()
    {
        _mockTodoListService = new Mock<ITodoListService>();
        _mockTodoListService
            .Setup(s => s.GetPriorityLevelsAsync())
            .ReturnsAsync(_priorityLevels);

        _mockTodoListService
            .Setup(s => s.GetTodoListsAsync())
            .ReturnsAsync(_todoLists);

        _sut = new GetTodosQueryHandler(_mockTodoListService.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task GetTodos_WithValidRequest_ReturnsExpectedResult()
    {
        // Arrange
        var query = new GetTodosQuery();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.PriorityLevels.ShouldNotBeNull();
        result.PriorityLevels.ShouldHaveCount(3);
        result.PriorityLevels.ShouldAllBe(p => p.Id != null && p.Title != null);

        result.Lists.ShouldNotBeNull();
        result.Lists.ShouldHaveCount(2);
        result.Lists.ShouldAllBe(l => l.Id != null && l.Title != null && l.Color != null);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task GetTodos_WithEmptyPriorityLevels_ReturnsEmptyPriorityLevels()
    {
        // Arrange
        _mockTodoListService
            .Setup(s => s.GetPriorityLevelsAsync())
            .ReturnsAsync(Array.Empty<LookupDto>());

        // Act
        var result = await _sut.Handle(new GetTodosQuery(), CancellationToken.None);

        // Assert
        result.PriorityLevels.ShouldNotBeNull();
        result.PriorityLevels.ShouldHaveCount(0);
    }

    [Fact]
    public async Task GetTodos_WithEmptyTodoLists_ReturnsEmptyTodoLists()
    {
        // Arrange
        _mockTodoListService
            .Setup(s => s.GetTodoListsAsync())
            .ReturnsAsync(Array.Empty<TodoListDto>());

        // Act
        var result = await _sut.Handle(new GetTodosQuery(), CancellationToken.None);

        // Assert
        result.Lists.ShouldNotBeNull();
        result.Lists.ShouldHaveCount(0);
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task GetTodos_WithNullPriorityLevels_ThrowsArgumentNullException()
    {
        // Arrange
        _mockTodoListService
            .Setup(s => s.GetPriorityLevelsAsync())
            .ReturnsAsync(null as IEnumerable<LookupDto>);

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _sut.Handle(new GetTodosQuery(), CancellationToken.None));
    }

    [Fact]
    public async Task GetTodos_WithNullTodoLists_ThrowsArgumentNullException()
    {
        // Arrange
        _mockTodoListService
            .Setup(s => s.GetTodoListsAsync())
            .ReturnsAsync(null as IEnumerable<TodoListDto>);

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _sut.Handle(new GetTodosQuery(), CancellationToken.None));
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task GetTodos_WhenGetPriorityLevelsFails_ThrowsBusinessException()
    {
        // Arrange
        _mockTodoListService
            .Setup(s => s.GetPriorityLevelsAsync())
            .ThrowsAsync(new BusinessException("Failed to retrieve priority levels"));

        // Act & Assert
        await Should.ThrowAsync<BusinessException>(() =>
            _sut.Handle(new GetTodosQuery(), CancellationToken.None));
    }

    [Fact]
    public async Task GetTodos_WhenGetTodoListsFails_ThrowsBusinessException()
    {
        // Arrange
        _mockTodoListService
            .Setup(s => s.GetTodoListsAsync())
            .ThrowsAsync(new BusinessException("Failed to retrieve todo lists"));

        // Act & Assert
        await Should.ThrowAsync<BusinessException>(() =>
            _sut.Handle(new GetTodosQuery(), CancellationToken.None));
    }

    #endregion

    #region Helper Methods

    private static GetTodosQuery CreateValidQuery() => new GetTodosQuery();

    #endregion
}