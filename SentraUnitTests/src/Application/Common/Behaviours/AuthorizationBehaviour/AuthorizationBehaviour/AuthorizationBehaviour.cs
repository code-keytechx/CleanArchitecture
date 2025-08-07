using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace CleanArchitecture.SentraUnitTests.Application.Common.Behaviours
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly AuthorizationBehaviour<TestRequest, TestResponse> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockUser = new Mock<IUser>();
            _mockIdentityService = new Mock<IIdentityService>();
            _behaviour = new AuthorizationBehaviour<TestRequest, TestResponse>(_mockUser.Object, _mockIdentityService.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndRequiredRole_ReturnsResponse()
        {
            // Business Context: Authorization is critical for accessing sensitive data
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned for authorized user");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthorized access must be prevented
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserNotInRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Users must have the correct role to access resources
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserNotAuthorizedByPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Users must be authorized by policy to access resources
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Policy = "CanAccessResource" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanAccessResource")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithNoAuthorizationAttributes_ReturnsResponse()
        {
            // Business Context: Requests without authorization attributes should proceed normally
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned for request without authorization attributes");
        }

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithUserInRequiredRole_ReturnsResponse()
        {
            // Business Context: Users with the required role should be able to access resources
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned for user in required role");
        }

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithUserAuthorizedByPolicy_ReturnsResponse()
        {
            // Business Context: Users authorized by policy should be able to access resources
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Policy = "CanAccessResource" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanAccessResource")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned for user authorized by policy");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithMultipleRolesAndUserInOne_ReturnsResponse()
        {
            // Business Context: Users with multiple roles should be able to access resources if they match one
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin,User" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "User")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned for user in one of the required roles");
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithMultiplePoliciesAndUserAuthorizedByOne_ReturnsResponse()
        {
            // Business Context: Users authorized by multiple policies should be able to access resources if they match one
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Policy = "CanAccessResource,CanEditResource" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanAccessResource")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditResource")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned for user authorized by one of the required policies");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullUserId_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Null user ID should result in unauthorized access
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithEmptyRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Empty role should result in forbidden access
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Roles = "" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithEmptyPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Empty policy should result in forbidden access
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Policy = "" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInIsInRoleAsync_ThrowsException()
        {
            // Business Context: Exceptions in role checking should propagate
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ThrowsAsync(new Exception("Role check failed"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<Exception>().WithMessage("Role check failed");
        }

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInAuthorizeAsync_ThrowsException()
        {
            // Business Context: Exceptions in policy authorization should propagate
            // Arrange
            var request = new TestRequest();
            var authorizeAttribute = new AuthorizeAttribute { Policy = "CanAccessResource" };
            var attributes = new[] { authorizeAttribute };
            request.GetType().GetField("<>p__1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(request, attributes);
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanAccessResource")).ThrowsAsync(new Exception("Policy check failed"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<Exception>().WithMessage("Policy check failed");
        }

        #endregion

        // Helper classes
        public class TestRequest : IRequest<TestResponse>
        {
            private AuthorizeAttribute[] _authorizeAttributes = Array.Empty<AuthorizeAttribute>();

            public object[] GetCustomAttributes(bool inherit)
            {
                return _authorizeAttributes;
            }

            public object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                return _authorizeAttributes.Where(a => a.GetType() == attributeType).ToArray();
            }

            public bool IsDefined(Type attributeType, bool inherit)
            {
                return _authorizeAttributes.Any(a => a.GetType() == attributeType);
            }

            public void AddAuthorizeAttribute(AuthorizeAttribute attribute)
            {
                var attributes = _authorizeAttributes.ToList();
                attributes.Add(attribute);
                _authorizeAttributes = attributes.ToArray();
            }
        }

        public class TestResponse { }
    }
}