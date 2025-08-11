using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using MediatR;
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
            var request = new TestRequest { RequiredRole = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned");
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
            var request = new TestRequest { RequiredRole = "Admin" };
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserNotInRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Users without required roles should be denied access
            // Arrange
            var request = new TestRequest { RequiredRole = "Admin" };
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
            var request = new TestRequest { RequiredPolicy = "AdminPolicy" };
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "AdminPolicy")).ReturnsAsync(false);

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
            response.Should().NotBeNull("response should be returned");
        }

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithUserInRequiredRole_ReturnsResponse()
        {
            // Business Context: Users with required roles should be authorized
            // Arrange
            var request = new TestRequest { RequiredRole = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned");
        }

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithUserAuthorizedByPolicy_ReturnsResponse()
        {
            // Business Context: Users authorized by policy should be allowed
            // Arrange
            var request = new TestRequest { RequiredPolicy = "AdminPolicy" };
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "AdminPolicy")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithMultipleRolesAndUserInOneRole_ReturnsResponse()
        {
            // Business Context: Users in any of the required roles should be authorized
            // Arrange
            var request = new TestRequest { RequiredRole = "Admin,User" };
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "User")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("response should be returned");
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithMultipleRolesAndUserInNoRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Users not in any of the required roles should be denied access
            // Arrange
            var request = new TestRequest { RequiredRole = "Admin,User" };
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "User")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null requests should be handled gracefully
            // Arrange
            TestRequest? request = null;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request!, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullUserId_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Null user IDs should be treated as unauthorized
            // Arrange
            var request = new TestRequest { RequiredRole = "Admin" };
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithIsInRoleAsyncThrowingException_ThrowsException()
        {
            // Business Context: Exceptions from IsInRoleAsync should propagate
            // Arrange
            var request = new TestRequest { RequiredRole = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ThrowsAsync(new Exception("Test Exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<Exception>().WithMessage("Test Exception");
        }

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithAuthorizeAsyncThrowingException_ThrowsException()
        {
            // Business Context: Exceptions from AuthorizeAsync should propagate
            // Arrange
            var request = new TestRequest { RequiredPolicy = "AdminPolicy" };
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "AdminPolicy")).ThrowsAsync(new Exception("Test Exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<Exception>().WithMessage("Test Exception");
        }

        #endregion

        // Helper classes
        [Authorize(Roles = "Admin")]
        public class TestRequest : IRequest<TestResponse>
        {
            public string? RequiredRole { get; set; }
            public string? RequiredPolicy { get; set; }
        }

        public class TestResponse
        {
        }
    }
}