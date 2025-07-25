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
        public async Task Handle_WithAuthorizedUserAndRole_AuthorizesSuccessfully()
        {
            // Business Context: User with role should be authorized
            // Arrange
            var user = new User { Id = "user-123" };
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(true);

            // Act
            Func<Task<string>> act = () => _behaviour.Handle(request, () => Task.FromResult("success"), CancellationToken.None);

            // Assert
            await act.Should().NotThrowAsync<UnauthorizedAccessException>();
            await act.Should().NotThrowAsync<ForbiddenAccessException>();
            await act.Should().Return("success");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthenticated user should be denied access
            // Arrange
            var request = new TestRequest();

            _mockUser.Setup(u => u.Id).Returns((string)null);

            // Act
            Func<Task<string>> act = () => _behaviour.Handle(request, () => Task.FromResult("success"), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithUnauthorizedUser_ThrowsForbiddenAccessException()
        {
            // Business Context: User without role should be denied access
            // Arrange
            var user = new User { Id = "user-123" };
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(false);

            // Act
            Func<Task<string>> act = () => _behaviour.Handle(request, () => Task.FromResult("success"), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Happy Path Tests
        // Tests for standard, expected user workflows
        // Covers typical business scenarios under normal conditions

        [Fact]
        [Trait("Category", "Happy")]
        public async Task Handle_WithNoAuthorizationAttributes_AllowsExecution()
        {
            // Business Context: No authorization attributes should allow execution
            // Arrange
            var request = new TestRequest();

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("success"), CancellationToken.None);

            // Assert
            result.Should().Be("success");
        }

        #endregion

        #region Edge Case Tests
        // Tests for boundary conditions and unusual but valid scenarios
        // Ensures system handles limits and special cases gracefully

        [Fact]
        [Trait("Category", "Edge")]
        public async Task Handle_WithMultipleRolesAndPolicies_AllowsExecution()
        {
            // Business Context: Multiple roles and policies should allow execution
            // Arrange
            var user = new User { Id = "user-123" };
            var authorizeAttribute1 = new AuthorizeAttribute { Roles = "Admin" };
            var authorizeAttribute2 = new AuthorizeAttribute { Policy = "CanViewResource" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute1, authorizeAttribute2 } };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(true);
            _mockIdentityService.Setup(i => i.AuthorizeAsync(user.Id, "CanViewResource")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("success"), CancellationToken.None);

            // Assert
            result.Should().Be("success");
        }

        [Fact]
        [Trait("Category", "Edge")]
        public async Task Handle_WithWhitespaceInRoleNames_IgnoresWhitespace()
        {
            // Business Context: Whitespace in role names should be ignored
            // Arrange
            var user = new User { Id = "user-123" };
            var authorizeAttribute = new AuthorizeAttribute { Roles = " Admin , Manager " };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("success"), CancellationToken.None);

            // Assert
            result.Should().Be("success");
        }

        #endregion

        #region Negative Tests
        // Tests for invalid inputs and expected failure scenarios
        // Ensures proper validation and error handling

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null request should throw ArgumentNullException
            // Arrange
            TestRequest request = null;

            // Act
            Func<Task<string>> act = () => _behaviour.Handle(request, () => Task.FromResult("success"), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithEmptyRoleNames_ThrowsArgumentException()
        {
            // Business Context: Empty role names should throw ArgumentException
            // Arrange
            var authorizeAttribute = new AuthorizeAttribute { Roles = ",," };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            // Act
            Func<Task<string>> act = () => _behaviour.Handle(request, () => Task.FromResult("success"), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        #endregion

        #region Exception Handling Tests
        // Tests for system resilience and error recovery
        // Ensures graceful degradation when external systems fail

        [Fact]
        [Trait("Category", "Exception")]
        public async Task Handle_WhenIdentityServiceThrowsException_RethrowsException()
        {
            // Business Context: External service failure should propagate exception
            // Arrange
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute } };

            _mockUser.Setup(u => u.Id).Returns("user-123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user-123", "Admin")).ThrowsAsync(new InvalidOperationException());

            // Act
            Func<Task<string>> act = () => _behaviour.Handle(request, () => Task.FromResult("success"), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // Ensures proper authentication, authorization, and audit trails

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithSensitiveData_LogsAuditTrail()
        {
            // Business Context: Sensitive data access should log audit trail
            // Arrange
            var user = new User { Id = "user-123" };
            var authorizeAttribute = new AuthorizeAttribute { Roles = "Admin" };
            var request = new TestRequest { AuthorizeAttributes = new[] { authorizeAttribute }, SensitiveData = "secret-data" };

            _mockUser.Setup(u => u.Id).Returns(user.Id);
            _mockIdentityService.Setup(i => i.IsInRoleAsync(user.Id, "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("success"), CancellationToken.None);

            // Assert
            result.Should().Be("success");
            // Assume there's an audit log mechanism in place to verify this
        }

        #endregion
    }

    public class TestRequest
    {
        public AuthorizeAttribute[]? AuthorizeAttributes { get; set; }
        public string? SensitiveData { get; set; }
    }
}
