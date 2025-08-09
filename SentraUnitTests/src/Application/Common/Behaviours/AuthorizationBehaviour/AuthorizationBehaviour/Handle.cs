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
using CleanArchitecture.Application.Common.Exceptions;

/*
REQUIRED NUGET PACKAGES:
- xunit
- xunit.runner.visualstudio  
- FluentAssertions
- Moq
- Microsoft.NET.Test.Sdk
- MediatR
- Microsoft.AspNetCore.Authorization

CRITICAL: Check .csproj for ALL PackageReference elements and include corresponding using statements
*/

namespace CleanArchitecture.Application.Tests.Behaviors
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
        public async Task Handle_WithValidUserAndRoleAuthorization_ReturnsResponse()
        {
            // Business Context: Ensures only authorized users can access protected resources
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
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
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserNotInRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Enforces role-based access control for sensitive operations
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "User")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserNotAuthorizedByPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Enforces policy-based authorization for complex access rules
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        public async Task Handle_WithValidUserAndValidRole_ReturnsResponse()
        {
            // Business Context: Standard authorization flow for authenticated users with proper roles
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WithValidUserAndValidPolicy_ReturnsResponse()
        {
            // Business Context: Policy-based authorization flow for complex access control
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WithValidUserAndMultipleRoles_ReturnsResponse()
        {
            // Business Context: Multi-role authorization for flexible access control
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "User")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithUserInOneOfMultipleRoles_ReturnsResponse()
        {
            // Business Context: Validates that authorization succeeds when user meets at least one role requirement
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "User")).ReturnsAsync(true);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Manager")).ReturnsAsync(false);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WithEmptyRolesAttribute_ReturnsResponse()
        {
            // Business Context: Handles edge case where roles attribute is empty but authorization is still required
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithUserNotInAnyRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Ensures proper error handling when user lacks required permissions
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "User")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Manager")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task Handle_WithNullUserAndAuthorizationRequired_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Ensures proper handling of null user scenarios
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithExceptionInRoleCheck_ThrowsException()
        {
            // Business Context: Ensures proper exception handling in authorization logic
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ThrowsAsync(new Exception("Database error"));
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task Handle_WithExceptionInPolicyCheck_ThrowsException()
        {
            // Business Context: Ensures proper exception handling in policy-based authorization
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ThrowsAsync(new Exception("Policy service error"));
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<Exception>();
        }

        #endregion

        private RequestHandlerDelegate<TestResponse> CreateNext()
        {
            return () => Task.FromResult(new TestResponse());
        }
    }

    // Test request and response classes
    public class TestRequest : IRequest<TestResponse>
    {
        [Authorize(Roles = "Admin,User")]
        public string? UserId { get; set; }
    }

    public class TestResponse
    {
        public bool Success { get; set; }
    }
}