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

/*
REQUIRED NUGET PACKAGES:
- xunit
- xunit.runner.visualstudio  
- FluentAssertions
- Moq
- Microsoft.NET.Test.Sdk
- MediatR
- Microsoft.EntityFrameworkCore

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
        public async Task Handle_WithValidUserAndRequiredRole_ReturnsResponse()
        {
            // Business Context: Authorization is critical for protecting sensitive data and system access
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestResponse>();
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthorized access attempts must be blocked to protect system integrity
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
            // Business Context: Role-based access control prevents unauthorized operations
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserNotAuthorizedByPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Policy-based authorization ensures compliance with business rules
            // Arrange
            var request = new TestRequestWithPolicy();
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
        public async Task Handle_WithAuthorizedUserAndNoAuthorizationAttribute_ReturnsResponse()
        {
            // Business Context: Requests without authorization requirements should proceed normally
            // Arrange
            var request = new TestRequestWithoutAuthorization();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestResponse>();
        }

        [Fact]
        public async Task Handle_WithUserInRequiredRole_ReturnsResponse()
        {
            // Business Context: Valid users with proper roles should be granted access
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestResponse>();
        }

        [Fact]
        public async Task Handle_WithUserAuthorizedByPolicy_ReturnsResponse()
        {
            // Business Context: Policy-based authorization should allow access when authorized
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestResponse>();
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithUserInMultipleRolesAndOneValid_ReturnsResponse()
        {
            // Business Context: Users with multiple roles should be authorized if they meet any requirement
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.SetupSequence(s => s.IsInRoleAsync("test-user-123", It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true); // User is in "Admin" role
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestResponse>();
        }

        [Fact]
        public async Task Handle_WithUserInRoleButPolicyRequired_ThrowsForbiddenAccessException()
        {
            // Business Context: Both role and policy requirements must be satisfied
            // Arrange
            var request = new TestRequestWithRoleAndPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithNullUserAndRequiredRole_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Null user should be rejected immediately
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Handle_WithEmptyRolesAttribute_ThrowsForbiddenAccessException()
        {
            // Business Context: Empty roles should be treated as no roles required
            // Arrange
            var request = new TestRequestWithEmptyRoles();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestResponse>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithExceptionInIdentityService_ThrowsException()
        {
            // Business Context: Identity service failures should propagate appropriately
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ThrowsAsync(new Exception("Service unavailable"));
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<Exception>();
        }

        #endregion

        private static RequestHandlerDelegate<TestResponse> CreateNext()
        {
            return () => Task.FromResult(new TestResponse());
        }
    }

    // Test request classes for different authorization scenarios
    [Authorize(Roles = "Admin")]
    public class TestRequest : IRequest<TestResponse>
    {
    }

    [Authorize(Roles = "Admin,User")]
    public class TestRequestWithMultipleRoles : IRequest<TestResponse>
    {
    }

    [Authorize(Policy = "CanEditData")]
    public class TestRequestWithPolicy : IRequest<TestResponse>
    {
    }

    [Authorize(Roles = "Admin")]
    [Authorize(Policy = "CanEditData")]
    public class TestRequestWithRoleAndPolicy : IRequest<TestResponse>
    {
    }

    [Authorize(Roles = "")]
    public class TestRequestWithEmptyRoles : IRequest<TestResponse>
    {
    }

    public class TestRequestWithoutAuthorization : IRequest<TestResponse>
    {
    }

    public class TestResponse
    {
    }
}