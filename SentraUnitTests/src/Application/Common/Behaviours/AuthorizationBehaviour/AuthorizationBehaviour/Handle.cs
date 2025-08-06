using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace CleanArchitecture.SentraUnitTests.Application.Behaviors
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<IUser> _mockUser;
        private readonly AuthorizationBehaviour<TestRequest, string> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockUser = new Mock<IUser>();
            _behaviour = new AuthorizationBehaviour<TestRequest, string>(_mockIdentityService.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndValidRole_ReturnsResponse()
        {
            // Business Context: Ensure authorized users with valid roles can access protected resources
            // Arrange
            var testRequest = new TestRequest { UserId = "user123", Roles = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(testRequest, () => Task.FromResult("Success"), CancellationToken.None);

            // Assert
            result.Should().Be("Success");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Ensure unauthenticated users are denied access
            // Arrange
            var testRequest = new TestRequest { UserId = null, Roles = "Admin" };
            _mockUser.Setup(u => u.Id).Returns((string)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(testRequest, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndInvalidRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Ensure authorized users with invalid roles are denied access
            // Arrange
            var testRequest = new TestRequest { UserId = "user123", Roles = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(testRequest, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithNullUserId_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Ensure null user IDs are handled securely
            // Arrange
            var testRequest = new TestRequest { UserId = null, Roles = "Admin" };
            _mockUser.Setup(u => u.Id).Returns((string)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(testRequest, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithEmptyRoles_ThrowsForbiddenAccessException()
        {
            // Business Context: Ensure empty roles are handled securely
            // Arrange
            var testRequest = new TestRequest { UserId = "user123", Roles = "" };
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(testRequest, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithValidRequest_ReturnsResponse()
        {
            // Business Context: Ensure valid requests are processed successfully
            // Arrange
            var testRequest = new TestRequest { UserId = "user123", Roles = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(testRequest, () => Task.FromResult("Success"), CancellationToken.None);

            // Assert
            result.Should().Be("Success");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithNullRoles_ReturnsResponse()
        {
            // Business Context: Ensure requests with null roles are processed successfully
            // Arrange
            var testRequest = new TestRequest { UserId = "user123", Roles = null };
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", null)).ReturnsAsync(false);

            // Act
            var result = await _behaviour.Handle(testRequest, () => Task.FromResult("Success"), CancellationToken.None);

            // Assert
            result.Should().Be("Success");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Ensure null requests are handled gracefully
            // Arrange
            TestRequest? testRequest = null;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(testRequest!, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInIsInRoleAsync_ThrowsException()
        {
            // Business Context: Ensure exceptions in authorization checks are handled gracefully
            // Arrange
            var testRequest = new TestRequest { UserId = "user123", Roles = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(testRequest, () => Task.FromResult("Success"), CancellationToken.None))
                .Should().ThrowAsync<Exception>("Test exception");
        }

        #endregion
    }

    public class TestRequest : IRequest<string>
    {
        public string? UserId { get; set; }
        public string? Roles { get; set; }
    }
}