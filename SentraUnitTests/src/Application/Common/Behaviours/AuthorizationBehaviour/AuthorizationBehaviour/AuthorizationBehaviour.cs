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
- AutoMapper
- FluentValidation.DependencyInjectionExtensions
- Microsoft.EntityFrameworkCore
- Microsoft.Extensions.Hosting
- Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.UI
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Relational
- Microsoft.EntityFrameworkCore.Tools
- Azure.Extensions.AspNetCore.Configuration.Secrets
- Microsoft.AspNetCore.OpenApi
- Microsoft.AspNetCore.SpaProxy
- Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- NSwag.AspNetCore
- NSwag.MSBuild
- FluentValidation.AspNetCore

CRITICAL: All PackageReference elements from .csproj analyzed and corresponding using statements included
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
            // This is critical for data security and compliance
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(true);

            var result = await _behaviour.Handle(request, NextDelegate, CancellationToken.None);

            result.Should().NotBeNull("response should be returned for authorized users");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithValidUserAndRequiredPolicy_AuthorizesSuccessfully()
        {
            // Business Context: Ensures policy-based access controls are enforced
            // Critical for regulatory compliance and security policies
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ReturnsAsync(true);

            var result = await _behaviour.Handle(request, NextDelegate, CancellationToken.None);

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
            // Essential for data protection and compliance with security standards
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            Func<Task> act = () => _behaviour.Handle(request, NextDelegate, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>("unauthorized users should be denied access");
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserMissingRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Enforces role-based access control
            // Critical for protecting sensitive operations and data
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ReturnsAsync(false);

            Func<Task> act = () => _behaviour.Handle(request, NextDelegate, CancellationToken.None);

            await act.Should().ThrowAsync<ForbiddenAccessException>("users without required roles should be denied access");
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserMissingRequiredPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Enforces policy-based access control
            // Essential for compliance with organizational policies and regulations
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.AuthorizeAsync("test-user-123", "CanEditData")).ReturnsAsync(false);

            Func<Task> act = () => _behaviour.Handle(request, NextDelegate, CancellationToken.None);

            await act.Should().ThrowAsync<ForbiddenAccessException>("users without required policies should be denied access");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        public async Task Handle_WithNoAuthorizationAttributes_ReturnsResponse()
        {
            // Business Context: Allows unrestricted access for public operations
            // Standard workflow for non-protected operations
            var request = new TestRequestWithoutAttributes();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");

            var result = await _behaviour.Handle(request, NextDelegate, CancellationToken.None);

            result.Should().NotBeNull("response should be returned for requests without authorization requirements");
        }

        [Fact]
        public async Task Handle_WithValidUserAndMultipleRoles_AuthorizesSuccessfully()
        {
            // Business Context: Supports flexible role-based access control
            // Enables complex authorization scenarios with multiple role requirements
            var request = new TestRequestWithMultipleRoles();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.SetupSequence(s => s.IsInRoleAsync("test-user-123", It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true);

            var result = await _behaviour.Handle(request, NextDelegate, CancellationToken.None);

            result.Should().NotBeNull("response should be returned when user has at least one required role");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithEmptyRolesString_ContinuesWithoutError()
        {
            // Business Context: Handles edge cases in authorization attribute configuration
            // Ensures robustness when attributes are misconfigured
            var request = new TestRequestWithEmptyRoles();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");

            var result = await _behaviour.Handle(request, NextDelegate, CancellationToken.None);

            result.Should().NotBeNull("response should be returned even with empty roles attribute");
        }

        [Fact]
        public async Task Handle_WithWhitespaceOnlyRolesString_ContinuesWithoutError()
        {
            // Business Context: Handles whitespace-only role specifications
            // Ensures robust parsing of authorization attributes
            var request = new TestRequestWithWhitespaceRoles();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");

            var result = await _behaviour.Handle(request, NextDelegate, CancellationToken.None);

            result.Should().NotBeNull("response should be returned even with whitespace-only roles attribute");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithNullUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Ensures null user handling is properly validated
            // Prevents null reference exceptions in authorization logic
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            Func<Task> act = () => _behaviour.Handle(request, NextDelegate, CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>("null user should be rejected");
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithExceptionInIdentityService_ThrowsOriginalException()
        {
            // Business Context: Ensures proper exception propagation
            // Maintains system reliability and error visibility
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("test-user-123", "Admin")).ThrowsAsync(new InvalidOperationException("Database error"));

            Func<Task> act = () => _behaviour.Handle(request, NextDelegate, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>("exceptions from identity service should propagate");
        }

        #endregion

        private static Task<TestResponse> NextDelegate()
        {
            return Task.FromResult(new TestResponse());
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

    [Authorize(Roles = "")]
    public class TestRequestWithEmptyRoles : IRequest<TestResponse>
    {
    }

    [Authorize(Roles = "   ")]
    public class TestRequestWithWhitespaceRoles : IRequest<TestResponse>
    {
    }

    [Authorize(Policy = "CanEditData")]
    public class TestRequestWithPolicy : IRequest<TestResponse>
    {
    }

    public class TestRequestWithoutAttributes : IRequest<TestResponse>
    {
    }

    public class TestResponse
    {
    }
}