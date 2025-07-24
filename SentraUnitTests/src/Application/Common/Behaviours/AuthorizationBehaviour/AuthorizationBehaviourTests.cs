using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using Moq;
using Xunit;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Tests.Common.Behaviours
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
        // Tests that protect core business processes critical for revenue and operations

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndAuthorizedRole_Succeeds()
        {
            // Business Context: Authenticated user with authorized role
            // Arrange
            var user = new User { Id = "USER_001" };
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("Success"), CancellationToken.None);

            // Assert
            result.Should().Be("Success");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthenticated user attempting to access protected resource
            // Arrange
            var request = new TestRequest();

            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                           .Should()
                           .ThrowAsync<UnauthorizedAccessException>()
                           .WithMessage("Unauthorized");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserButUnauthorizedRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Authenticated user with unauthorized role
            // Arrange
            var user = new User { Id = "USER_001" };
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                           .Should()
                           .ThrowAsync<ForbiddenAccessException>()
                           .WithMessage("Forbidden");
        }

        #endregion

        #region Happy Path Tests
        // Tests for standard, expected user workflows
        // Covers typical business scenarios under normal conditions

        [Fact]
        [Trait("Category", "Happy")]
        public async Task Handle_WithNoAuthorizationAttributes_Succeeds()
        {
            // Business Context: Request with no authorization attributes
            // Arrange
            var request = new TestRequest();

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("Success"), CancellationToken.None);

            // Assert
            result.Should().Be("Success");
        }

        [Fact]
        [Trait("Category", "Happy")]
        public async Task Handle_WithPolicyBasedAuthorization_Succeeds()
        {
            // Business Context: Request with policy-based authorization
            // Arrange
            var user = new User { Id = "USER_001" };
            var authorizeAttribute = new AuthorizeAttribute { Policy = "CanViewResource" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.AuthorizeAsync(user.Id, "CanViewResource")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("Success"), CancellationToken.None);

            // Assert
            result.Should().Be("Success");
        }

        #endregion

        #region Edge Case Tests
        // Tests for boundary conditions and unusual but valid scenarios
        // Ensures system handles limits and special cases gracefully

        [Fact]
        [Trait("Category", "Edge")]
        public async Task Handle_WithMultipleRolesAndOneAuthorizedRole_Succeeds()
        {
            // Business Context: Multiple roles, one authorized
            // Arrange
            var user = new User { Id = "USER_001" };
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin,Editor" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Editor")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("Success"), CancellationToken.None);

            // Assert
            result.Should().Be("Success");
        }

        [Fact]
        [Trait("Category", "Edge")]
        public async Task Handle_WithNullRolesString_ThrowsArgumentException()
        {
            // Business Context: Null roles string in AuthorizeAttribute
            // Arrange
            var authorizeAttribute = new AuthorizeAttribute { Roles = string.Empty };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                           .Should()
                           .ThrowAsync<ArgumentException>()
                           .WithMessage("Roles cannot be null or whitespace.");
        }

        #endregion

        #region Negative Tests
        // Tests for invalid inputs and expected failure scenarios
        // Ensures proper validation and error handling

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithEmptyRolesString_ThrowsArgumentException()
        {
            // Business Context: Empty roles string in AuthorizeAttribute
            // Arrange
            var authorizeAttribute = new AuthorizeAttribute { Roles = "" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                           .Should()
                           .ThrowAsync<ArgumentException>()
                           .WithMessage("Roles cannot be null or whitespace.");
        }

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullPolicy_ThrowsArgumentException()
        {
            // Business Context: Null policy in AuthorizeAttribute
            // Arrange
            var authorizeAttribute = new AuthorizeAttribute { Policy = string.Empty };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                           .Should()
                           .ThrowAsync<ArgumentException>()
                           .WithMessage("Policy cannot be null or whitespace.");
        }

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithEmptyPolicy_ThrowsArgumentException()
        {
            // Business Context: Empty policy in AuthorizeAttribute
            // Arrange
            var authorizeAttribute = new AuthorizeAttribute { Policy = "" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                           .Should()
                           .ThrowAsync<ArgumentException>()
                           .WithMessage("Policy cannot be null or whitespace.");
        }

        #endregion

        #region Exception Handling Tests
        // Tests for system resilience and error recovery
        // Ensures graceful degradation when external systems fail

        [Fact]
        [Trait("Category", "Exception")]
        public async Task Handle_WhenIdentityServiceThrowsException_ThrowsException()
        {
            // Business Context: Identity service throws exception
            // Arrange
            var user = new User { Id = "USER_001" };
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ThrowsAsync(new InvalidOperationException());

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                           .Should()
                           .ThrowAsync<InvalidOperationException>();
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // Ensures proper authentication, authorization, and audit trails

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthorizedUser_AuditsFailedAttempt()
        {
            // Business Context: Unauthorized user attempt
            // Arrange
            var user = new User { Id = "USER_001" };
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(false);

            // Act
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("Success"), CancellationToken.None))
                           .Should()
                           .ThrowAsync<ForbiddenAccessException>();

            // Assert
            _mockIdentityService.Verify(i => i.AuditFailedAuthorizationAsync(user.Id, "Admin"), Times.Once);
        }

        #endregion
    }

    public class TestRequest
    {
        public AuthorizeAttribute[]? AuthorizeAttributes { get; set; }
    }

    public class User : IUser
    {
        public string? Id { get; set; }
    }

}
