using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace CleanArchitecture.UnitTests.Application.Common.Behaviors
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IUser> _userMock;
        private readonly AuthorizationBehaviour<MockRequest, MockResponse> _authorizationBehaviour;

        public AuthorizationBehaviourTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _userMock = new Mock<IUser>();
            _authorizationBehaviour = new AuthorizationBehaviour<MockRequest, MockResponse>(_userMock.Object, _identityServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WhenRequestHasNoAuthorizeAttribute_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequest();
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();
            var response = new MockResponse();
            next.Setup(n => n()).ReturnsAsync(response);

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            next.Verify(n => n(), Times.Once);
            result.Should().BeSameAs(response);
        }

        [Fact]
        public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var request = new MockRequestWithAuthAttribute();
            _userMock.Setup(u => u.Id).Returns((string)null);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();

            // Act & Assert
            await _authorizationBehaviour.Invoking(async x => await x.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedButNotInRequiredRole_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithAuthAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).ReturnsAsync(false);
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "User")).ReturnsAsync(false);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();

            // Act & Assert
            await _authorizationBehaviour.Invoking(async x => await x.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedAndInRequiredRole_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithAuthAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();
            var response = new MockResponse();
            next.Setup(n => n()).ReturnsAsync(response);

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            next.Verify(n => n(), Times.Once);
            result.Should().BeSameAs(response);
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedAndInOneOfRequiredRoles_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithMultipleRolesAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).ReturnsAsync(false);
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "User")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();
            var response = new MockResponse();
            next.Setup(n => n()).ReturnsAsync(response);

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            next.Verify(n => n(), Times.Once);
            result.Should().BeSameAs(response);
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedButPolicyAuthorizationFails_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithPolicyAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(false);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();

            // Act & Assert
            await _authorizationBehaviour.Invoking(async x => await x.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedAndPolicyAuthorizationPasses_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithPolicyAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();
            var response = new MockResponse();
            next.Setup(n => n()).ReturnsAsync(response);

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            next.Verify(n => n(), Times.Once);
            result.Should().BeSameAs(response);
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedAndBothRoleAndPolicyAuthorizationPasses_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithRoleAndPolicyAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).ReturnsAsync(true);
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();
            var response = new MockResponse();
            next.Setup(n => n()).ReturnsAsync(response);

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            next.Verify(n => n(), Times.Once);
            result.Should().BeSameAs(response);
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedAndRoleAuthorizationPassesButPolicyFails_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithRoleAndPolicyAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).ReturnsAsync(true);
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(false);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();

            // Act & Assert
            await _authorizationBehaviour.Invoking(async x => await x.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedAndRoleAuthorizationFailsButPolicyPasses_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithRoleAndPolicyAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).ReturnsAsync(false);
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();

            // Act & Assert
            await _authorizationBehaviour.Invoking(async x => await x.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedAndMultiplePoliciesAreRequired_ShouldThrowForbiddenAccessExceptionIfAnyFails()
        {
            // Arrange
            var request = new MockRequestWithMultiplePoliciesAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(true);
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanDelete")).ReturnsAsync(false);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();

            // Act & Assert
            await _authorizationBehaviour.Invoking(async x => await x.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedAndMultiplePoliciesAreRequired_ShouldCallNextIfAllPass()
        {
            // Arrange
            var request = new MockRequestWithMultiplePoliciesAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(true);
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanDelete")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();
            var response = new MockResponse();
            next.Setup(n => n()).ReturnsAsync(response);

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            next.Verify(n => n(), Times.Once);
            result.Should().BeSameAs(response);
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedAndRolesAreTrimmed_ShouldHandleWhitespaceCorrectly()
        {
            // Arrange
            var request = new MockRequestWithTrimmedRolesAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).ReturnsAsync(false);
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "User")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();
            var response = new MockResponse();
            next.Setup(n => n()).ReturnsAsync(response);

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            next.Verify(n => n(), Times.Once);
            result.Should().BeSameAs(response);
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedAndRoleContainsWhitespace_ShouldTrimRole()
        {
            // Arrange
            var request = new MockRequestWithWhitespaceInRoleAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).ReturnsAsync(false);
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "User")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<MockResponse>>();
            var response = new MockResponse();
            next.Setup(n => n()).ReturnsAsync(response);

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            next.Verify(n => n(), Times.Once);
            result.Should().BeSameAs(response);
        }
    }

    // Mock classes for testing
    public class MockRequest
    {
    }

    [Authorize(Roles = "Admin")]
    public class MockRequestWithAuthAttribute
    {
    }

    [Authorize(Roles = "Admin,User")]
    public class MockRequestWithMultipleRolesAttribute
    {
    }

    [Authorize(Policy = "CanEdit")]
    public class MockRequestWithPolicyAttribute
    {
    }

    [Authorize(Roles = "Admin", Policy = "CanEdit")]
    public class MockRequestWithRoleAndPolicyAttribute
    {
    }

    [Authorize(Policy = "CanEdit")]
    [Authorize(Policy = "CanDelete")]
    public class MockRequestWithMultiplePoliciesAttribute
    {
    }

    [Authorize(Roles = " Admin , User ")]
    public class MockRequestWithTrimmedRolesAttribute
    {
    }

    [Authorize(Roles = " Admin , User ")]
    public class MockRequestWithWhitespaceInRoleAttribute
    {
    }

    public class MockResponse
    {
    }
}