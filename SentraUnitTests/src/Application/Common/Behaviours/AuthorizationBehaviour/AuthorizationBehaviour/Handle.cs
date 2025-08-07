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
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));

            // Act
            var result = await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _identityServiceMock.Verify(x => x.IsInRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _identityServiceMock.Verify(x => x.AuthorizeAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var request = new MockRequestWithAuthAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns((string)null);

            // Act
            Func<Task> act = async () => await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
            _identityServiceMock.Verify(x => x.IsInRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _identityServiceMock.Verify(x => x.AuthorizeAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserHasRolesButNotAuthorized_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithRolesAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).Returns(Task.FromResult(false));
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "User")).Returns(Task.FromResult(false));

            // Act
            Func<Task> act = async () => await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            _identityServiceMock.Verify(x => x.IsInRoleAsync("userId", "Admin"), Times.Once);
            _identityServiceMock.Verify(x => x.IsInRoleAsync("userId", "User"), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasRolesAndAuthorized_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithRolesAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).Returns(Task.FromResult(false));
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "User")).Returns(Task.FromResult(true));

            // Act
            var result = await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _identityServiceMock.Verify(x => x.IsInRoleAsync("userId", "Admin"), Times.Once);
            _identityServiceMock.Verify(x => x.IsInRoleAsync("userId", "User"), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasPoliciesButNotAuthorized_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithPolicyAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).Returns(Task.FromResult(false));

            // Act
            Func<Task> act = async () => await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            _identityServiceMock.Verify(x => x.AuthorizeAsync("userId", "CanEdit"), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasPoliciesAndAuthorized_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithPolicyAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).Returns(Task.FromResult(true));

            // Act
            var result = await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _identityServiceMock.Verify(x => x.AuthorizeAsync("userId", "CanEdit"), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasBothRolesAndPoliciesAndAuthorized_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithRolesAndPolicyAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).Returns(Task.FromResult(true));
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).Returns(Task.FromResult(true));

            // Act
            var result = await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _identityServiceMock.Verify(x => x.IsInRoleAsync("userId", "Admin"), Times.Once);
            _identityServiceMock.Verify(x => x.AuthorizeAsync("userId", "CanEdit"), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasRolesAndPoliciesButNotAuthorized_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithRolesAndPolicyAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).Returns(Task.FromResult(false));
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).Returns(Task.FromResult(false));

            // Act
            Func<Task> act = async () => await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            _identityServiceMock.Verify(x => x.IsInRoleAsync("userId", "Admin"), Times.Once);
            _identityServiceMock.Verify(x => x.AuthorizeAsync("userId", "CanEdit"), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasMultipleRolesAndOneAuthorized_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithMultipleRolesAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Admin")).Returns(Task.FromResult(false));
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "User")).Returns(Task.FromResult(true));
            _identityServiceMock.Setup(x => x.IsInRoleAsync("userId", "Guest")).Returns(Task.FromResult(false));

            // Act
            var result = await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _identityServiceMock.Verify(x => x.IsInRoleAsync("userId", "Admin"), Times.Once);
            _identityServiceMock.Verify(x => x.IsInRoleAsync("userId", "User"), Times.Once);
            _identityServiceMock.Verify(x => x.IsInRoleAsync("userId", "Guest"), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasMultiplePoliciesAndOneNotAuthorized_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var request = new MockRequestWithMultiplePoliciesAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns("userId");
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanEdit")).Returns(Task.FromResult(true));
            _identityServiceMock.Setup(x => x.AuthorizeAsync("userId", "CanDelete")).Returns(Task.FromResult(false));

            // Act
            Func<Task> act = async () => await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            _identityServiceMock.Verify(x => x.AuthorizeAsync("userId", "CanEdit"), Times.Once);
            _identityServiceMock.Verify(x => x.AuthorizeAsync("userId", "CanDelete"), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasEmptyRolesAttribute_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithEmptyRolesAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns("userId");

            // Act
            var result = await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _identityServiceMock.Verify(x => x.IsInRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _identityServiceMock.Verify(x => x.AuthorizeAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserHasEmptyPolicyAttribute_ShouldCallNext()
        {
            // Arrange
            var request = new MockRequestWithEmptyPolicyAttribute();
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));
            _userMock.Setup(x => x.Id).Returns("userId");

            // Act
            var result = await _authorizationBehaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _identityServiceMock.Verify(x => x.IsInRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _identityServiceMock.Verify(x => x.AuthorizeAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }

    // Mock classes for testing
    public class MockRequest
    {
    }

    public class MockRequestWithAuthAttribute : MockRequest
    {
    }

    public class MockRequestWithRolesAttribute : MockRequest
    {
    }

    public class MockRequestWithPolicyAttribute : MockRequest
    {
    }

    public class MockRequestWithRolesAndPolicyAttribute : MockRequest
    {
    }

    public class MockRequestWithMultipleRolesAttribute : MockRequest
    {
    }

    public class MockRequestWithMultiplePoliciesAttribute : MockRequest
    {
    }

    public class MockRequestWithEmptyRolesAttribute : MockRequest
    {
    }

    public class MockRequestWithEmptyPolicyAttribute : MockRequest
    {
    }

    public class MockResponse
    {
    }

    // Mock attributes for testing
    public class AuthorizeAttribute : Attribute
    {
        public string Roles { get; set; }
        public string Policy { get; set; }

        public AuthorizeAttribute()
        {
        }

        public AuthorizeAttribute(string roles, string policy)
        {
            Roles = roles;
            Policy = policy;
        }
    }

    // Extension methods to simulate GetCustomAttributes behavior
    public static class MockRequestExtensions
    {
        public static IEnumerable<AuthorizeAttribute> GetCustomAttributes<T>(this Type type) where T : Attribute
        {
            if (type == typeof(MockRequestWithAuthAttribute))
            {
                return new List<AuthorizeAttribute> { new AuthorizeAttribute() };
            }
            else if (type == typeof(MockRequestWithRolesAttribute))
            {
                return new List<AuthorizeAttribute> { new AuthorizeAttribute("Admin,User", null) };
            }
            else if (type == typeof(MockRequestWithPolicyAttribute))
            {
                return new List<AuthorizeAttribute> { new AuthorizeAttribute(null, "CanEdit") };
            }
            else if (type == typeof(MockRequestWithRolesAndPolicyAttribute))
            {
                return new List<AuthorizeAttribute> { new AuthorizeAttribute("Admin", "CanEdit") };
            }
            else if (type == typeof(MockRequestWithMultipleRolesAttribute))
            {
                return new List<AuthorizeAttribute> { new AuthorizeAttribute("Admin,User,Guest", null) };
            }
            else if (type == typeof(MockRequestWithMultiplePoliciesAttribute))
            {
                return new List<AuthorizeAttribute> { new AuthorizeAttribute(null, "CanEdit"), new AuthorizeAttribute(null, "CanDelete") };
            }
            else if (type == typeof(MockRequestWithEmptyRolesAttribute))
            {
                return new List<AuthorizeAttribute> { new AuthorizeAttribute("", null) };
            }
            else if (type == typeof(MockRequestWithEmptyPolicyAttribute))
            {
                return new List<AuthorizeAttribute> { new AuthorizeAttribute(null, "") };
            }
            return new List<AuthorizeAttribute>();
        }
    }
}