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
- Microsoft.Extensions.DependencyInjection.Abstractions
- Microsoft.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.UI
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Relational
- Microsoft.EntityFrameworkCore.Tools
- Aspire.Microsoft.EntityFrameworkCore.SqlServer
- Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
- Azure.Identity
- Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
- AutoMapper
- FluentValidation.DependencyInjectionExtensions
- Ardalis.GuardClauses
- AutoFixture
- Bogus

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
            // Business Context: Authorization is critical for protecting sensitive data and functionality
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("response should be returned for authorized user");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithValidUserAndRequiredPolicy_ReturnsResponse()
        {
            // Business Context: Policy-based authorization ensures compliance with business rules
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("response should be returned for authorized user");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthorized access attempts must be blocked to protect data
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>("unauthorized access should be blocked");
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserMissingRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Role-based access control prevents unauthorized operations
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>("user without required role should be denied access");
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserMissingRequiredPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Policy-based access control enforces business rules
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>("user without required policy should be denied access");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        public async Task Handle_WithNoAuthorizationAttributes_ReturnsResponse()
        {
            // Business Context: Requests without authorization requirements should proceed normally
            // Arrange
            var request = new TestRequestWithoutAuth();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("response should be returned for requests without authorization");
        }

        [Fact]
        public async Task Handle_WithUserInRequiredRole_ReturnsResponse()
        {
            // Business Context: Authorized users should be able to access protected resources
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("authorized user should receive response");
        }

        [Fact]
        public async Task Handle_WithUserInAnyOfMultipleRoles_ReturnsResponse()
        {
            // Business Context: Users with any of multiple required roles should be authorized
            // Arrange
            var request = new TestRequestWithMultipleRoles();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Editor")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("user with one of multiple required roles should be authorized");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithUserInRoleButEmptyRolesAttribute_ReturnsResponse()
        {
            // Business Context: Empty roles should be handled gracefully
            // Arrange
            var request = new TestRequestWithEmptyRoles();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("request with empty roles should proceed normally");
        }

        [Fact]
        public async Task Handle_WithUserInRoleButEmptyPolicyAttribute_ReturnsResponse()
        {
            // Business Context: Empty policies should be handled gracefully
            // Arrange
            var request = new TestRequestWithEmptyPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("request with empty policy should proceed normally");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithNullUser_ReturnsResponse()
        {
            // Business Context: Null user should be handled gracefully
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>("null user should be rejected");
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithExceptionInIdentityService_ThrowsOriginalException()
        {
            // Business Context: Exceptions from identity service should propagate properly
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ThrowsAsync(new InvalidOperationException("Service error"));
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>("original exception should be propagated");
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

    [Authorize(Roles = "Admin,Editor")]
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
    }
}