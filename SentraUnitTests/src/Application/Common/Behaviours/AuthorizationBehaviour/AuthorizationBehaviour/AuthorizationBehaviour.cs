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
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Relational
- Microsoft.EntityFrameworkCore.Tools

CRITICAL: Check .csproj for ALL PackageReference elements and include corresponding using statements
*/

namespace CleanArchitecture.Application.Tests.Behaviours
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly AuthorizationBehaviour<TestRequest, string> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockUser = new Mock<IUser>();
            _mockIdentityService = new Mock<IIdentityService>();
            _behaviour = new AuthorizationBehaviour<TestRequest, string>(_mockUser.Object, _mockIdentityService.Object);
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
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, CreateNextDelegate<string>(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("Success");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthorized access attempts must be rejected to protect system integrity
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNextDelegate<string>(), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserNotInRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Role-based access control prevents unauthorized operations
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Manager")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNextDelegate<string>(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserNotAuthorizedByPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Policy-based authorization ensures compliance with business rules
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("user-123", "CanEditData")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNextDelegate<string>(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
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
            _mockUser.Setup(u => u.Id).Returns("user-123");

            // Act
            var result = await _behaviour.Handle(request, CreateNextDelegate<string>(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("Success");
        }

        [Fact]
        public async Task Handle_WithUserInRequiredRole_ReturnsResponse()
        {
            // Business Context: Valid users with proper roles should be granted access
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, CreateNextDelegate<string>(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("Success");
        }

        [Fact]
        public async Task Handle_WithUserAuthorizedByPolicy_ReturnsResponse()
        {
            // Business Context: Policy-based authorization should work correctly for valid users
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("user-123", "CanEditData")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, CreateNextDelegate<string>(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("Success");
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
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.SetupSequence(s => s.IsInRoleAsync("user-123", It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, CreateNextDelegate<string>(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("Success");
        }

        [Fact]
        public async Task Handle_WithEmptyRolesAttribute_ReturnsResponse()
        {
            // Business Context: Empty roles should be handled gracefully
            // Arrange
            var request = new TestRequestWithEmptyRoles();
            _mockUser.Setup(u => u.Id).Returns("user-123");

            // Act
            var result = await _behaviour.Handle(request, CreateNextDelegate<string>(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("Success");
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
            await _behaviour.Invoking(b => b.Handle(request, CreateNextDelegate<string>(), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Handle_WithUserInNoRolesAndRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Users with no matching roles should be denied access
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Admin")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNextDelegate<string>(), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithExceptionInIdentityService_ThrowsOriginalException()
        {
            // Business Context: Exceptions from identity service should propagate correctly
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Admin")).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, CreateNextDelegate<string>(), CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database error");
        }

        #endregion

        private static RequestHandlerDelegate<TResponse> CreateNextDelegate<TResponse>()
        {
            return () => Task.FromResult(default(TResponse)!);
        }
    }

    // Test request classes for different authorization scenarios
    [Authorize(Roles = "Admin,Manager")]
    public class TestRequest : IRequest<string>
    {
    }

    [Authorize(Policy = "CanEditData")]
    public class TestRequestWithPolicy : IRequest<string>
    {
    }

    [Authorize(Roles = "")]
    public class TestRequestWithEmptyRoles : IRequest<string>
    {
    }

    public class TestRequestWithoutAuth : IRequest<string>
    {
    }
}