using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.Tests.Common.Behaviours;

public class AuthorizationBehaviourTests
{
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<IUser> _mockUser;
    private readonly AuthorizationBehaviour<TestRequest, TestResponse> _behaviour;

    public AuthorizationBehaviourTests()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _mockUser = new Mock<IUser>();
        _behaviour = new AuthorizationBehaviour<TestRequest, TestResponse>(_mockIdentityService.Object, _mockUser.Object);
    }

    #region Critical Path Tests

    [Fact]
    [Trait("Category", "Critical")]
    public async Task Handle_WithAuthorizedUserAndPolicy_Succeeds()
    {
        // Arrange
        var user = new User { Id = "user123" };
        var policy = "AdminPolicy";

        _mockUser.Setup(u => u.Id).Returns(user.Id);
        _mockIdentityService.Setup(i => i.AuthorizeAsync(user.Id, policy)).ReturnsAsync(true);

        var request = new TestRequest { Policy = policy };
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();
        next.Setup(n => n()).ReturnsAsync(new TestResponse());

        // Act
        var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        next.Verify(n => n(), Times.Once);
    }

    [Fact]
    [Trait("Category", "Critical")]
    public async Task Handle_WithUnauthorizedUserAndPolicy_FailsWithForbiddenAccessException()
    {
        // Arrange
        var user = new User { Id = "user123" };
        var policy = "AdminPolicy";

        _mockUser.Setup(u => u.Id).Returns(user.Id);
        _mockIdentityService.Setup(i => i.AuthorizeAsync(user.Id, policy)).ReturnsAsync(false);

        var request = new TestRequest { Policy = policy };
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
    }

    #endregion

    #region Happy Path Tests

    [Fact]
    [Trait("Category", "Happy")]
    public async Task Handle_WithNoAuthorizeAttributes_Succeeds()
    {
        // Arrange
        var request = new TestRequest();
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();
        next.Setup(n => n()).ReturnsAsync(new TestResponse());

        // Act
        var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        next.Verify(n => n(), Times.Once);
    }

    [Fact]
    [Trait("Category", "Happy")]
    public async Task Handle_WithAuthenticatedUserButNoRoles_Succeeds()
    {
        // Arrange
        var user = new User { Id = "user123" };
        var request = new TestRequest();

        _mockUser.Setup(u => u.Id).Returns(user.Id);

        var next = new Mock<RequestHandlerDelegate<TestResponse>>();
        next.Setup(n => n()).ReturnsAsync(new TestResponse());

        // Act
        var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        next.Verify(n => n(), Times.Once);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    [Trait("Category", "Edge")]
    public async Task Handle_WithNullUser_IdentifiesAsAnonymous()
    {
        // Arrange
        _mockUser.Setup(u => u.Id).Returns((string)null);

        var request = new TestRequest();
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();
        next.Setup(n => n()).ReturnsAsync(new TestResponse());

        // Act
        var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        next.Verify(n => n(), Times.Once);
    }

    [Fact]
    [Trait("Category", "Edge")]
    public async Task Handle_WithMultipleRolesAndOneMatching_Succeeds()
    {
        // Arrange
        var user = new User { Id = "user123" };
        var roles = "Admin,User";
        var policy = "AdminPolicy";

        _mockUser.Setup(u => u.Id).Returns(user.Id);
        _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(true);

        var request = new TestRequest { Roles = roles, Policy = policy };
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();
        next.Setup(n => n()).ReturnsAsync(new TestResponse());

        // Act
        var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        next.Verify(n => n(), Times.Once);
    }

    [Fact]
    [Trait("Category", "Edge")]
    public async Task Handle_WithMultipleRolesAndNoneMatching_FailsWithForbiddenAccessException()
    {
        // Arrange
        var user = new User { Id = "user123" };
        var roles = "Admin,User";
        var policy = "AdminPolicy";

        _mockUser.Setup(u => u.Id).Returns(user.Id);
        _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(false);

        var request = new TestRequest { Roles = roles, Policy = policy };
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
    }

    #endregion

    #region Negative Tests

    [Fact]
    [Trait("Category", "Negative")]
    public async Task Handle_WithNullPolicy_ThrowsArgumentNullException()
    {
        // Arrange
        var request = new TestRequest { Policy = null };
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
    }

    [Fact]
    [Trait("Category", "Negative")]
    public async Task Handle_WithEmptyPolicy_ThrowsArgumentException()
    {
        // Arrange
        var request = new TestRequest { Policy = "" };
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
    }

    [Fact]
    [Trait("Category", "Negative")]
    public async Task Handle_WithNullRoles_ThrowsArgumentNullException()
    {
        // Arrange
        var request = new TestRequest { Roles = null };
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
    }

    [Fact]
    [Trait("Category", "Negative")]
    public async Task Handle_WithEmptyRoles_ThrowsArgumentException()
    {
        // Arrange
        var request = new TestRequest { Roles = "" };
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
    }

    #endregion

    #region Exception Tests

    [Fact]
    [Trait("Category", "Exception")]
    public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _mockUser.Setup(u => u.Id).Returns((string)null);

        var request = new TestRequest();
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
    }

    [Fact]
    [Trait("Category", "Exception")]
    public async Task Handle_WithExceptionFromIdentityService_ThrowsException()
    {
        // Arrange
        var user = new User { Id = "user123" };
        var policy = "AdminPolicy";

        _mockUser.Setup(u => u.Id).Returns(user.Id);
        _mockIdentityService.Setup(i => i.AuthorizeAsync(user.Id, policy)).ThrowsAsync(new InvalidOperationException("Simulated exception"));

        var request = new TestRequest { Policy = policy };
        var next = new Mock<RequestHandlerDelegate<TestResponse>>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
    }

    #endregion
}

public class TestRequest : IRequest<TestResponse>
{
    public string Policy { get; set; }
    public string Roles { get; set; }
}

public class TestResponse
{
}
