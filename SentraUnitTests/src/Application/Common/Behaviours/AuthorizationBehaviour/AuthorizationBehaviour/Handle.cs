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
        private readonly AuthorizationBehaviour<MockRequest, string> _authorizationBehaviour;

        public AuthorizationBehaviourTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _userMock = new Mock<IUser>();
            _authorizationBehaviour = new AuthorizationBehaviour<MockRequest, string>(_userMock.Object, _identityServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WhenRequestHasNoAuthorizeAttribute_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequest();
            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(n => n()).ReturnsAsync("Success");

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().Be("Success");
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var request = new MockRequestWithAuthAttribute();
            _userMock.Setup(u => u.Id).Returns((string)null);

            var next = new Mock<RequestHandlerDelegate<string>>();

            // Act
            Func<Task<string>> act = async () => await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
            next.Verify(n => n(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthenticatedButNoRolesRequired_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithAuthAttribute();
            _userMock.Setup(u => u.Id).Returns("userId");
            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(n => n()).ReturnsAsync("Success");

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().Be("Success");
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasRequiredRole_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithAuthAttributeAndRoles("Admin");
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(s => s.IsInRoleAsync("userId", "Admin")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(n => n()).ReturnsAsync("Success");

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().Be("Success");
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotHaveRequiredRole_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithAuthAttributeAndRoles("Admin");
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(s => s.IsInRoleAsync("userId", "Admin")).ReturnsAsync(false);
            var next = new Mock<RequestHandlerDelegate<string>>();

            // Act
            Func<Task<string>> act = async () => await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            next.Verify(n => n(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserHasOneOfMultipleRequiredRoles_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithAuthAttributeAndRoles("Admin,Manager");
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.SetupSequence(s => s.IsInRoleAsync("userId", It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true); // Manager role check returns true
            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(n => n()).ReturnsAsync("Success");

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().Be("Success");
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotHaveAnyOfMultipleRequiredRoles_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithAuthAttributeAndRoles("Admin,Manager");
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.SetupSequence(s => s.IsInRoleAsync("userId", It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(false); // Both roles return false
            var next = new Mock<RequestHandlerDelegate<string>>();

            // Act
            Func<Task<string>> act = async () => await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            next.Verify(n => n(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserHasPolicyAuthorization_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithAuthAttributeAndPolicy("CanEdit");
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(s => s.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(n => n()).ReturnsAsync("Success");

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().Be("Success");
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotHavePolicyAuthorization_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithAuthAttributeAndPolicy("CanEdit");
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.Setup(s => s.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(false);
            var next = new Mock<RequestHandlerDelegate<string>>();

            // Act
            Func<Task<string>> act = async () => await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            next.Verify(n => n(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserHasBothRoleAndPolicyAuthorization_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithAuthAttributeAndRolesAndPolicy("Admin", "CanEdit");
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.SetupSequence(s => s.IsInRoleAsync("userId", "Admin"))
                .ReturnsAsync(true);
            _identityServiceMock.Setup(s => s.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(n => n()).ReturnsAsync("Success");

            // Act
            var result = await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().Be("Success");
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasRoleButNotPolicyAuthorization_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithAuthAttributeAndRolesAndPolicy("Admin", "CanEdit");
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.SetupSequence(s => s.IsInRoleAsync("userId", "Admin"))
                .ReturnsAsync(true);
            _identityServiceMock.Setup(s => s.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(false);
            var next = new Mock<RequestHandlerDelegate<string>>();

            // Act
            Func<Task<string>> act = async () => await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            next.Verify(n => n(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserHasPolicyButNotRoleAuthorization_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithAuthAttributeAndRolesAndPolicy("Admin", "CanEdit");
            _userMock.Setup(u => u.Id).Returns("userId");
            _identityServiceMock.SetupSequence(s => s.IsInRoleAsync("userId", "Admin"))
                .ReturnsAsync(false);
            _identityServiceMock.Setup(s => s.AuthorizeAsync("userId", "CanEdit")).ReturnsAsync(true);
            var next = new Mock<RequestHandlerDelegate<string>>();

            // Act
            Func<Task<string>> act = async () => await _authorizationBehaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            next.Verify(n => n(), Times.Never);
        }

        // Mock classes for testing
        public class MockRequest
        {
        }

        [Authorize]
        public class MockRequestWithAuthAttribute
        {
        }

        [Authorize(Roles = "Admin")]
        public class MockRequestWithAuthAttributeAndRoles
        {
            public MockRequestWithAuthAttributeAndRoles(string roles)
            {
                Roles = roles;
            }

            public string Roles { get; }
        }

        [Authorize(Policy = "CanEdit")]
        public class MockRequestWithAuthAttributeAndPolicy
        {
            public MockRequestWithAuthAttributeAndPolicy(string policy)
            {
                Policy = policy;
            }

            public string Policy { get; }
        }

        [Authorize(Roles = "Admin", Policy = "CanEdit")]
        public class MockRequestWithAuthAttributeAndRolesAndPolicy
        {
            public MockRequestWithAuthAttributeAndRolesAndPolicy(string roles, string policy)
            {
                Roles = roles;
                Policy = policy;
            }

            public string Roles { get; }
            public string Policy { get; }
        }
    }
}