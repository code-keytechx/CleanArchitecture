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
- Microsoft.AspNetCore.Mvc.Testing
- Microsoft.EntityFrameworkCore.InMemory
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.Extensions.Configuration.Json
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
        public async Task Handle_WithUserMissingRequiredRole_ThrowsForbiddenAccessException()
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
        public async Task Handle_WithUserHavingRequiredRole_Succeeds()
        {
            // Business Context: Allows authorized users to access protected resources
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

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        public async Task Handle_WithNoAuthorizationAttributes_ReturnsResponse()
        {
            // Business Context: Allows unrestricted access for public operations
            // Arrange
            var request = new PublicRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WithValidUserAndValidPolicy_ReturnsResponse()
        {
            // Business Context: Ensures policy-based authorization works correctly
            // Arrange
            var request = new PolicyRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEdit")).ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithUserHavingMultipleRolesAndOneValid_ReturnsResponse()
        {
            // Business Context: Supports flexible role assignment for complex access control
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.SetupSequence(s => s.IsInRoleAsync("test-user-123", It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WithEmptyRolesString_ReturnsResponse()
        {
            // Business Context: Handles edge case where roles attribute is empty
            // Arrange
            var request = new EmptyRolesRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithUserHavingNoRolesAndRequiredRoles_ThrowsForbiddenAccessException()
        {
            // Business Context: Prevents access when user lacks required roles
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
        public async Task Handle_WithUserHavingNoRolesAndNoRequiredRoles_ReturnsResponse()
        {
            // Business Context: Allows access when no roles are required
            // Arrange
            var request = new NoRolesRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            
            // Act
            var result = await _behaviour.Handle(request, CreateNext(), CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithPolicyAuthorizationFailure_ThrowsForbiddenAccessException()
        {
            // Business Context: Ensures policy-based access control fails gracefully
            // Arrange
            var request = new PolicyRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEdit")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNext(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        private static RequestHandlerDelegate<TestResponse> CreateNext()
        {
            return () => Task.FromResult(new TestResponse());
        }
    }

    // Test request classes for different authorization scenarios
    [Authorize(Roles = "Admin,User")]
    public class TestRequest : IRequest<TestResponse>
    {
    }

    [Authorize(Policy = "CanEdit")]
    public class PolicyRequest : IRequest<TestResponse>
    {
    }

    [Authorize(Roles = "")]
    public class EmptyRolesRequest : IRequest<TestResponse>
    {
    }

    public class NoRolesRequest : IRequest<TestResponse>
    {
    }

    public class PublicRequest : IRequest<TestResponse>
    {
    }

    public class TestResponse
    {
    }
}