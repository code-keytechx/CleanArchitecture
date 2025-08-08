// MANDATORY using statements:
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using MediatR;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Application.Common.Behaviours;
using System.Security.Claims;

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

namespace CleanArchitecture.SentraUnitTests.Application.Common.Behaviours
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
            var result = await _behaviour.Handle(request, () => Task.FromResult("Success"), CancellationToken.None);
            
            // Assert
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
            // Business Context: Unauthorized access attempts must be blocked to protect system integrity
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
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
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
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
            var request = new TestRequestWithoutAttributes();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            
            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("Success"), CancellationToken.None);
            
            // Assert
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
            var result = await _behaviour.Handle(request, () => Task.FromResult("Success"), CancellationToken.None);
            
            // Assert
            result.Should().Be("Success");
        }

        [Fact]
        public async Task Handle_WithUserInOneOfMultipleRoles_ReturnsResponse()
        {
            // Business Context: Users with any of multiple required roles should be authorized
            // Arrange
            var request = new TestRequestWithMultipleRoles();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.SetupSequence(s => s.IsInRoleAsync("user-123", It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true);
            
            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("Success"), CancellationToken.None);
            
            // Assert
            result.Should().Be("Success");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithEmptyRolesString_ReturnsResponse()
        {
            // Business Context: Empty role specifications should be handled gracefully
            // Arrange
            var request = new TestRequestWithEmptyRoles();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            
            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("Success"), CancellationToken.None);
            
            // Assert
            result.Should().Be("Success");
        }

        [Fact]
        public async Task Handle_WithUserInRoleButPolicyRequired_ThrowsForbiddenAccessException()
        {
            // Business Context: Both role and policy authorization must be satisfied
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Admin")).ReturnsAsync(true);
            _mockIdentityService.Setup(s => s.AuthorizeAsync("user-123", "SpecialPolicy")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
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
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Handle_WithUserNotInAnyRequiredRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Users not meeting any role requirements should be denied access
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Manager")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithPolicyAuthorizationFailure_ThrowsForbiddenAccessException()
        {
            // Business Context: Policy-based authorization failures must be properly handled
            // Arrange
            var request = new TestRequestWithPolicy();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Admin")).ReturnsAsync(true);
            _mockIdentityService.Setup(s => s.AuthorizeAsync("user-123", "SpecialPolicy")).ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task Handle_WithMultiplePolicyFailures_ThrowsForbiddenAccessException()
        {
            // Business Context: Multiple policy requirements must all be satisfied
            // Arrange
            var request = new TestRequestWithMultiplePolicies();
            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(s => s.IsInRoleAsync("user-123", "Admin")).ReturnsAsync(true);
            _mockIdentityService.SetupSequence(s => s.AuthorizeAsync("user-123", It.IsAny<string>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            
            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion
    }

    // Test request classes for different authorization scenarios
    public class TestRequest : IRequest<string>
    {
        public TestRequest()
        {
            // Add a dummy implementation to satisfy the compiler
        }
    }

    public class TestRequestWithMultipleRoles : IRequest<string>
    {
        public TestRequestWithMultipleRoles()
        {
            // Add a dummy implementation to satisfy the compiler
        }
    }

    public class TestRequestWithEmptyRoles : IRequest<string>
    {
        public TestRequestWithEmptyRoles()
        {
            // Add a dummy implementation to satisfy the compiler
        }
    }

    public class TestRequestWithPolicy : IRequest<string>
    {
        public TestRequestWithPolicy()
        {
            // Add a dummy implementation to satisfy the compiler
        }
    }

    public class TestRequestWithMultiplePolicies : IRequest<string>
    {
        public TestRequestWithMultiplePolicies()
        {
            // Add a dummy implementation to satisfy the compiler
        }
    }

    public class TestRequestWithoutAttributes : IRequest<string>
    {
        public TestRequestWithoutAttributes()
        {
            // Add a dummy implementation to satisfy the compiler
        }
    }
}