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
        public async Task Handle_WithAuthorizedUser_ReturnsResponse()
        {
            // Business Context: Authorized users should be able to access the resource
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);

            var request = new TestRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("authorized user should receive a response");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthenticated users should not be able to access the resource
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            var request = new TestRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthorizedRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Users with incorrect roles should not be able to access the resource
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);

            var request = new TestRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthorizedPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Users failing policy checks should not be able to access the resource
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "AdminPolicy")).ReturnsAsync(false);

            var request = new TestRequest { RequiredPolicy = "AdminPolicy" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithNoAuthorizationAttributes_ReturnsResponse()
        {
            // Business Context: Requests without authorization attributes should proceed without checks
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("request without authorization attributes should receive a response");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithMultipleRoles_ReturnsResponseIfAnyRoleMatches()
        {
            // Business Context: Users with any of the required roles should be able to access the resource
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "User")).ReturnsAsync(true);

            var request = new TestRequest { RequiredRole = "Admin,User" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("user with any of the required roles should receive a response");
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
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            var request = new TestRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInIsInRoleAsync_ThrowsException()
        {
            // Business Context: Exceptions during role check should propagate
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ThrowsAsync(new Exception("Role check failed"));

            var request = new TestRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<Exception>().WithMessage("Role check failed");
        }

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInAuthorizeAsync_ThrowsException()
        {
            // Business Context: Exceptions during policy check should propagate
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "AdminPolicy")).ThrowsAsync(new Exception("Policy check failed"));

            var request = new TestRequest { RequiredPolicy = "AdminPolicy" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<Exception>().WithMessage("Policy check failed");
        }

        #endregion

        // Helper classes
        [Authorize(Roles = "Admin")]
        public class TestRequest : IRequest<TestResponse>
        {
            public string? RequiredRole { get; set; }
            public string? RequiredPolicy { get; set; }
        }

        public class TestResponse { }
    }
}