// MANDATORY using statements:
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using MediatR;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Exceptions;

/*
REQUIRED NUGET PACKAGES:
- xunit
- xunit.runner.visualstudio  
- FluentAssertions
- Moq
- Microsoft.NET.Test.Sdk
- MediatR
- Microsoft.Extensions.DependencyInjection.Abstractions

CRITICAL: Check .csproj for ALL PackageReference elements and include corresponding using statements
*/

namespace CleanArchitecture.Application.Common.Behaviours
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
        public async Task Handle_WithValidUserAndRequiredRole_AuthorizesSuccessfully()
        {
            // Business Context: Ensures only authorized users can access protected resources
            // This prevents unauthorized access to sensitive data and functionality
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should be returned for authorized users");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithValidUserAndRequiredPolicy_AuthorizesSuccessfully()
        {
            // Business Context: Ensures policy-based access control works correctly
            // This enforces business rules beyond simple role checks
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should be returned for authorized users");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Prevents unauthorized access to protected resources
            // This is critical for data security and compliance with privacy regulations
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>("unauthenticated users should be denied access");
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserMissingRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Enforces role-based access control
            // This prevents users from accessing functionality they shouldn't have access to
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>("users without required roles should be denied access");
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserFailingPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Enforces policy-based access control
            // This ensures complex business rules are enforced for access control
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>("users failing policy checks should be denied access");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        public async Task Handle_WithNoAuthorizationAttributes_ReturnsResponse()
        {
            // Business Context: Allows unrestricted access for public endpoints
            // This enables normal operation for non-protected functionality
            // Arrange
            var request = new TestRequestWithoutAuth();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should be returned for requests without authorization requirements");
        }

        [Fact]
        public async Task Handle_WithMultipleRolesAndValidRole_AuthorizesSuccessfully()
        {
            // Business Context: Supports flexible role-based access control
            // This allows for complex authorization scenarios with multiple possible roles
            // Arrange
            var request = new TestRequestWithMultipleRoles();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.SetupSequence(s => s.IsInRoleAsync("test-user-123", It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should be returned when user has at least one required role");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithEmptyRolesString_ContinuesWithoutError()
        {
            // Business Context: Handles edge cases in authorization attribute configuration
            // This ensures robustness when attributes are misconfigured
            // Arrange
            var request = new TestRequestWithEmptyRoles();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should be returned even with empty roles attribute");
        }

        [Fact]
        public async Task Handle_WithEmptyPolicyString_ContinuesWithoutError()
        {
            // Business Context: Handles edge cases in authorization attribute configuration
            // This ensures robustness when attributes are misconfigured
            // Arrange
            var request = new TestRequestWithEmptyPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should be returned even with empty policy attribute");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithNullUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Ensures proper handling of missing user context
            // This prevents null reference exceptions and ensures consistent security behavior
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>("null user should result in unauthorized access");
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithExceptionInIdentityService_ThrowsOriginalException()
        {
            // Business Context: Ensures proper error propagation for identity service failures
            // This allows for appropriate error handling at higher levels
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>("exceptions from identity service should propagate");
        }

        #endregion
    }

    // Test request classes for different authorization scenarios
    [Authorize(Roles = "Admin")]
    public class TestRequest : IRequest<TestResponse>
    {
    }

    [Authorize(Roles = "Admin,Manager")]
    public class TestRequestWithMultipleRoles : IRequest<TestResponse>
    {
    }

    [Authorize(Roles = "")]
    public class TestRequestWithEmptyRoles : IRequest<TestResponse>
    {
    }

    [Authorize(Policy = "CanEditData")]
    public class TestRequestWithPolicy : IRequest<TestResponse>
    {
    }

    [Authorize(Policy = "")]
    public class TestRequestWithEmptyPolicy : IRequest<TestResponse>
    {
    }

    public class TestRequestWithoutAuth : IRequest<TestResponse>
    {
    }

    public class TestResponse
    {
        public string? Message { get; set; }
    }
}