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
- Microsoft.AspNetCore.Mvc.Testing
- Microsoft.EntityFrameworkCore.InMemory
- Microsoft.EntityFrameworkCore.Sqlite
- coverlet.collector
- Ardalis.GuardClauses
- AutoMapper
- FluentValidation.DependencyInjectionExtensions
- Microsoft.EntityFrameworkCore
- Microsoft.Extensions.Hosting
- Aspire.Microsoft.EntityFrameworkCore.SqlServer
- Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
- Azure.Identity
- Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.UI
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Relational
- Microsoft.EntityFrameworkCore.Tools

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
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("authorization should succeed for valid users with required roles");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithValidUserAndRequiredPolicy_AuthorizesSuccessfully()
        {
            // Business Context: Ensures policy-based access control works correctly
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEdit")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("authorization should succeed for valid users with required policies");
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
                .Should().ThrowAsync<UnauthorizedAccessException>("unauthenticated users should be denied access");
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserMissingRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Enforces role-based access control for sensitive operations
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Manager")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>("users without required roles should be denied access");
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserMissingRequiredPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Enforces policy-based access control for sensitive operations
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEdit")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>("users without required policies should be denied access");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        public async Task Handle_WithNoAuthorizationAttributes_ReturnsResponse()
        {
            // Business Context: Allows unrestricted access for public operations
            // Arrange
            var request = new TestRequestWithoutAuth();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("requests without authorization requirements should proceed normally");
        }

        [Fact]
        public async Task Handle_WithValidUserAndMultipleRoles_AuthorizesSuccessfully()
        {
            // Business Context: Supports flexible role-based access control
            // Arrange
            var request = new TestRequestWithMultipleRoles();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Manager")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("authorization should succeed when user has at least one required role");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithEmptyRolesString_AuthorizesSuccessfully()
        {
            // Business Context: Handles edge cases in authorization attribute configuration
            // Arrange
            var request = new TestRequestWithEmptyRoles();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("requests with empty roles should proceed normally");
        }

        [Fact]
        public async Task Handle_WithUserWithMultipleRolesAndPolicy_AuthorizesSuccessfully()
        {
            // Business Context: Supports complex authorization scenarios
            // Arrange
            var request = new TestRequestWithMultipleRolesAndPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEdit")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull("authorization should succeed when user meets all requirements");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithNullUserAndRequiredRole_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Ensures null user handling is robust
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
            // Business Context: Ensures proper error propagation from dependencies
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ThrowsAsync(new InvalidOperationException("Database error"));
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>("exceptions from identity service should propagate");
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

    [Authorize(Roles = "Admin,Manager")]
    public class TestRequestWithMultipleRoles : IRequest<TestResponse>
    {
    }

    [Authorize(Roles = "")]
    public class TestRequestWithEmptyRoles : IRequest<TestResponse>
    {
    }

    [Authorize(Policy = "CanEdit")]
    public class TestRequestWithPolicy : IRequest<TestResponse>
    {
    }

    [Authorize(Roles = "Admin", Policy = "CanEdit")]
    public class TestRequestWithMultipleRolesAndPolicy : IRequest<TestResponse>
    {
    }

    public class TestRequestWithoutAuth : IRequest<TestResponse>
    {
    }

    public class TestResponse
    {
    }
}