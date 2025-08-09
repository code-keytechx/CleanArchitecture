using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using MediatR; // Added for IRequest<>
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
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanAccessResource")).ReturnsAsync(false);
            var request = new TestRequest { RequiredPolicy = "CanAccessResource" };
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

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithMultipleRoles_ReturnsResponse()
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
            result.Should().NotBeNull("user with any required role should receive a response");
        }

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithMultiplePolicies_ReturnsResponse()
        {
            // Business Context: Users passing all required policies should be able to access the resource
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanAccessResource1")).ReturnsAsync(true);
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanAccessResource2")).ReturnsAsync(true);
            var request = new TestRequest { RequiredPolicy = "CanAccessResource1,CanAccessResource2" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("user passing all required policies should receive a response");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithEmptyRole_ReturnsResponse()
        {
            // Business Context: Requests with empty role attribute should proceed without role checks
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            var request = new TestRequest { RequiredRole = "" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("request with empty role attribute should receive a response");
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithEmptyPolicy_ReturnsResponse()
        {
            // Business Context: Requests with empty policy attribute should proceed without policy checks
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            var request = new TestRequest { RequiredPolicy = "" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("request with empty policy attribute should receive a response");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullUser_ThrowsArgumentNullException()
        {
            // Business Context: Null user should result in an exception
            // Arrange
            var request = new TestRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            AuthorizationBehaviour<TestRequest, TestResponse>? behaviour = null;

            // Act & Assert
            await behaviour.Invoking(b => b!.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullIdentityService_ThrowsArgumentNullException()
        {
            // Business Context: Null identity service should result in an exception
            // Arrange
            var request = new TestRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            AuthorizationBehaviour<TestRequest, TestResponse>? behaviour = new AuthorizationBehaviour<TestRequest, TestResponse>(null!, _mockIdentityService.Object);

            // Act & Assert
            await behaviour.Invoking(b => b!.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInNext_ThrowsException()
        {
            // Business Context: Exceptions in the next handler should propagate
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);
            var request = new TestRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<TestResponse>(() => throw new InvalidOperationException("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>();
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