using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace CleanArchitecture.SentraUnitTests.Application.Common.Security
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<IUser> _mockUser;
        private readonly AuthorizationBehaviour<TestRequest, TestResponse> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockUser = new Mock<IUser>();
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
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Roles = "Admin" });
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
            // Business Context: Unauthorized access should be prevented
            // Arrange
            var request = new TestRequest();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Roles = "Admin" });
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserNotInRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Users not in required roles should be denied access
            // Arrange
            var request = new TestRequest();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Roles = "Admin" });
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
            // Business Context: Users not authorized by policy should be denied access
            // Arrange
            var request = new TestRequest();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Policy = "CanEdit" });
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEdit")).ReturnsAsync(false);

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
            // Business Context: Users in required roles should be authorized
            // Arrange
            var request = new TestRequest();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Roles = "Admin" });
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
            // Business Context: Users authorized by policy should be allowed
            // Arrange
            var request = new TestRequest();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Policy = "CanEdit" });
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEdit")).ReturnsAsync(true);

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
            // Business Context: Users in any of the required roles should be authorized
            // Arrange
            var request = new TestRequest();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Roles = "Admin,Editor" });
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Editor")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned for user in one of the required roles");
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithMultiplePoliciesAndUserAuthorizedByOne_ReturnsResponse()
        {
            // Business Context: Users authorized by any of the policies should be allowed
            // Arrange
            var request = new TestRequest();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Policy = "CanEdit,CanDelete" });
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEdit")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanDelete")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned for user authorized by one of the policies");
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
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Roles = "Admin" });
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
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Roles = "" });
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
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Policy = "" });
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
            // Business Context: Exceptions in role check should propagate
            // Arrange
            var request = new TestRequest();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Roles = "Admin" });
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
            // Business Context: Exceptions in policy check should propagate
            // Arrange
            var request = new TestRequest();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute { Policy = "CanEdit" });
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEdit")).ThrowsAsync(new Exception("Policy check failed"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<Exception>().WithMessage("Policy check failed");
        }

        #endregion

        // Helper classes
        public class TestRequest : IRequest<TestResponse>
        {
        }

        public class TestResponse
        {
        }
    }
}