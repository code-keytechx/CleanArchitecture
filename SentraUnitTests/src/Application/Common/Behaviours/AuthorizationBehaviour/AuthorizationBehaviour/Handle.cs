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

namespace CleanArchitecture.Application.Tests
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
            _behaviour = new AuthorizationBehaviour<TestRequest, string>(_mockIdentityService.Object, _mockUser.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithNoAuthorizeAttributes_ReturnsNextResult()
        {
            // Business Context: No authorization attributes means no checks, should proceed
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("user-id");

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None);

            // Assert
            result.Should().Be("next-result");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndValidRole_ReturnsNextResult()
        {
            // Business Context: Authenticated user with valid role should proceed
            // Arrange
            var request = new TestRequest { UserId = "user-id", RequiredRole = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("user-id");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user-id", "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None);

            // Assert
            result.Should().Be("next-result");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndInvalidRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Authenticated user with invalid role should throw ForbiddenAccessException
            // Arrange
            var request = new TestRequest { UserId = "user-id", RequiredRole = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("user-id");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user-id", "Admin")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndPolicy_ReturnsNextResult()
        {
            // Business Context: Authenticated user with valid policy should proceed
            // Arrange
            var request = new TestRequest { UserId = "user-id", RequiredPolicy = "Policy1" };
            _mockUser.Setup(u => u.Id).Returns("user-id");
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user-id", "Policy1")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None);

            // Assert
            result.Should().Be("next-result");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndInvalidPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Authenticated user with invalid policy should throw ForbiddenAccessException
            // Arrange
            var request = new TestRequest { UserId = "user-id", RequiredPolicy = "Policy1" };
            _mockUser.Setup(u => u.Id).Returns("user-id");
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user-id", "Policy1")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthenticated user should throw UnauthorizedAccessException
            // Arrange
            var request = new TestRequest { UserId = "user-id", RequiredRole = "Admin" };
            _mockUser.Setup(u => u.Id).Returns((string)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        public async Task Handle_WithValidRequest_ReturnsNextResult()
        {
            // Business Context: Valid request should proceed
            // Arrange
            var request = new TestRequest { UserId = "user-id", RequiredRole = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("user-id");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user-id", "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None);

            // Assert
            result.Should().Be("next-result");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null request should throw ArgumentNullException
            // Arrange
            TestRequest? request = null;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request!, () => Task.FromResult("next-result"), CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithInvalidRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Invalid role should throw ForbiddenAccessException
            // Arrange
            var request = new TestRequest { UserId = "user-id", RequiredRole = "InvalidRole" };
            _mockUser.Setup(u => u.Id).Returns("user-id");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user-id", "InvalidRole")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task Handle_WithInvalidPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Invalid policy should throw ForbiddenAccessException
            // Arrange
            var request = new TestRequest { UserId = "user-id", RequiredPolicy = "InvalidPolicy" };
            _mockUser.Setup(u => u.Id).Returns("user-id");
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user-id", "InvalidPolicy")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithExceptionInIsInRoleAsync_ThrowsException()
        {
            // Business Context: Exception in IsInRoleAsync should propagate
            // Arrange
            var request = new TestRequest { UserId = "user-id", RequiredRole = "Admin" };
            _mockUser.Setup(u => u.Id).Returns("user-id");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user-id", "Admin")).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None))
                .Should().ThrowAsync<Exception>("Test exception");
        }

        [Fact]
        public async Task Handle_WithExceptionInAuthorizeAsync_ThrowsException()
        {
            // Business Context: Exception in AuthorizeAsync should propagate
            // Arrange
            var request = new TestRequest { UserId = "user-id", RequiredPolicy = "Policy1" };
            _mockUser.Setup(u => u.Id).Returns("user-id");
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user-id", "Policy1")).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult("next-result"), CancellationToken.None))
                .Should().ThrowAsync<Exception>("Test exception");
        }

        #endregion
    }

    public class TestRequest : IRequest<string>
    {
        public string? UserId { get; set; }
        public string? RequiredRole { get; set; }
        public string? RequiredPolicy { get; set; }
    }
}