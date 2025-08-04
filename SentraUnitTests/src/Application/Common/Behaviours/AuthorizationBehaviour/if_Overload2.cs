using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using Moq;
using Shouldly;
using Xunit;

public class AuthorizationBehaviourTests
{
    private readonly Mock<IAuthorizationService> _mockAuthorizationService;
    private readonly AuthorizationBehaviour<SampleRequest, SampleResponse> _behaviour;

    public AuthorizationBehaviourTests()
    {
        _mockAuthorizationService = new Mock<IAuthorizationService>();
        _behaviour = new AuthorizationBehaviour<SampleRequest, SampleResponse>(_mockAuthorizationService.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task Handle_WithAuthorizedUser_PassesThrough()
    {
        // Arrange
        var user = new ClaimsPrincipal(new Claim[] { new Claim(ClaimTypes.Role, "Admin") });
        var context = new CommandHandlerContext(user);
        var request = new SampleRequest();
        var next = new Mock<IPipelineBehavior<SampleRequest, SampleResponse>>();
        next.Setup(n => n.Handle(request, It.IsAny<CancellationToken>())).ReturnsAsync(new SampleResponse());

        // Act
        var response = await _behaviour.Handle(request, context, next.Object);

        // Assert
        response.ShouldNotBeNull();
        next.Verify(n => n.Handle(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task Handle_WithUnauthorizedUser_ThrowsForbiddenAccessException()
    {
        // Arrange
        var user = new ClaimsPrincipal(new Claim[] { new Claim(ClaimTypes.Role, "Guest") });
        var context = new CommandHandlerContext(user);
        var request = new SampleRequest();

        // Act & Assert
        await Should.ThrowAsync<ForbiddenAccessException>(() =>
            _behaviour.Handle(request, context, Mock.Of<IPipelineBehavior<SampleRequest, SampleResponse>>()));
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task Handle_WithNullUser_ThrowsArgumentNullException()
    {
        // Arrange
        var context = new CommandHandlerContext(null);
        var request = new SampleRequest();

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _behaviour.Handle(request, context, Mock.Of<IPipelineBehavior<SampleRequest, SampleResponse>>()));
    }

    [Fact]
    public async Task Handle_WithEmptyRolesList_ThrowsArgumentException()
    {
        // Arrange
        var user = new ClaimsPrincipal(new Claim[] { new Claim(ClaimTypes.Role, "") });
        var context = new CommandHandlerContext(user);
        var request = new SampleRequest();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() =>
            _behaviour.Handle(request, context, Mock.Of<IPipelineBehavior<SampleRequest, SampleResponse>>()));
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task Handle_WhenAuthorizationServiceThrowsException_RethrowsException()
    {
        // Arrange
        var user = new ClaimsPrincipal(new Claim[] { new Claim(ClaimTypes.Role, "Admin") });
        var context = new CommandHandlerContext(user);
        var request = new SampleRequest();
        var exception = new InvalidOperationException("Authorization check failed");
        _mockAuthorizationService.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<IEnumerable<string>>()))
                                  .ThrowsAsync(exception);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() =>
            _behaviour.Handle(request, context, Mock.Of<IPipelineBehavior<SampleRequest, SampleResponse>>()));
    }

    #endregion

    #region Helper Methods

    private class SampleRequest { }

    private class SampleResponse { }

    private class CommandHandlerContext : ICommandHandlerContext
    {
        public ClaimsPrincipal User { get; }

        public CommandHandlerContext(ClaimsPrincipal user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }
    }

    #endregion
}