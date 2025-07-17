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
        var next = new Mock<RequestHandlerDelegate<SampleResponse>>();
        next.Setup(n => n()).Returns(Task.FromResult(new SampleResponse()));

        _mockAuthorizationService.Setup(a => a.Authorize(context.User, It.IsAny<string[]>())).Returns(true);

        // Act
        var response = await _behaviour.Handle(request, context, next.Object);

        // Assert
        response.ShouldNotBeNull();
        next.Verify(n => n(), Times.Once);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task Handle_WithEmptyRolesArray_PassesThrough()
    {
        // Arrange
        var user = new ClaimsPrincipal(new Claim[] { new Claim(ClaimTypes.Role, "Admin") });
        var context = new CommandHandlerContext(user);
        var request = new SampleRequest();
        var next = new Mock<RequestHandlerDelegate<SampleResponse>>();
        next.Setup(n => n()).Returns(Task.FromResult(new SampleResponse()));

        _mockAuthorizationService.Setup(a => a.Authorize(context.User, It.IsAny<string[]>())).Returns(true);

        // Act
        var response = await _behaviour.Handle(request, context, next.Object);

        // Assert
        response.ShouldNotBeNull();
        next.Verify(n => n(), Times.Once);
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task Handle_WithUnauthorizedUser_ThrowsForbiddenAccessException()
    {
        // Arrange
        var user = new ClaimsPrincipal(new Claim[] { new Claim(ClaimTypes.Role, "Guest") });
        var context = new CommandHandlerContext(user);
        var request = new SampleRequest();
        var next = new Mock<RequestHandlerDelegate<SampleResponse>>();

        _mockAuthorizationService.Setup(a => a.Authorize(context.User, It.IsAny<string[]>())).Returns(false);

        // Act & Assert
        await Should.ThrowAsync<ForbiddenAccessException>(() => _behaviour.Handle(request, context, next.Object));
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
        var next = new Mock<RequestHandlerDelegate<SampleResponse>>();

        _mockAuthorizationService.Setup(a => a.Authorize(context.User, It.IsAny<string[]>())).ThrowsAsync(new InvalidOperationException());

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() => _behaviour.Handle(request, context, next.Object));
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
            User = user;
        }
    }

    #endregion
}