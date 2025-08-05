using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace CleanArchitecture.SentraUnitTests.Application.Common.Behaviours
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
        public async Task Handle_WithAuthorizedUser_ReturnsResponse()
        {
            // Business Context: Authorized users should be able to access the resource
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.IsInRoleAsync(request.UserId, "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            result.Should().NotBeNull("authorized user should receive a response");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthenticated users should not be able to access the resource
            // Arrange
            var request = new TestRequest { UserId = null };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthorizedRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Users with incorrect roles should not be able to access the resource
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.IsInRoleAsync(request.UserId, "Admin")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthorizedPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Users failing policy checks should not be able to access the resource
            // Arrange
            var request = new TestRequestWithPolicy { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.AuthorizeAsync(request.UserId, "AdminPolicy")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithNoAuthorizationAttributes_ReturnsResponse()
        {
            // Business Context: Requests without authorization attributes should proceed without checks
            // Arrange
            var request = new TestRequestWithoutAuthorization { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            result.Should().NotBeNull("request without authorization attributes should receive a response");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithMultipleRoles_ReturnsResponseIfAnyRoleMatches()
        {
            // Business Context: Users with any of the required roles should be able to access the resource
            // Arrange
            var request = new TestRequestWithMultipleRoles { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.IsInRoleAsync(request.UserId, "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync(request.UserId, "User")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None);

            // Assert
            result.Should().NotBeNull("user with any required role should receive a response");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullUserId_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Null user IDs should not be able to access the resource
            // Arrange
            var request = new TestRequest { UserId = null };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInIsInRoleAsync_ThrowsException()
        {
            // Business Context: Exceptions during role checks should propagate
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.IsInRoleAsync(request.UserId, "Admin")).ThrowsAsync(new Exception("Role check failed"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<Exception>().WithMessage("Role check failed");
        }

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInAuthorizeAsync_ThrowsException()
        {
            // Business Context: Exceptions during policy checks should propagate
            // Arrange
            var request = new TestRequestWithPolicy { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.AuthorizeAsync(request.UserId, "AdminPolicy")).ThrowsAsync(new Exception("Policy check failed"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new TestResponse()), CancellationToken.None))
                .Should().ThrowAsync<Exception>().WithMessage("Policy check failed");
        }

        #endregion
    }

    [Authorize(Roles = "Admin")]
    public class TestRequest : IRequest<TestResponse>
    {
        public string? UserId { get; set; }
    }

    [Authorize(Policy = "AdminPolicy")]
    public class TestRequestWithPolicy : IRequest<TestResponse>
    {
        public string? UserId { get; set; }
    }

    [Authorize(Roles = "Admin,User")]
    public class TestRequestWithMultipleRoles : IRequest<TestResponse>
    {
        public string? UserId { get; set; }
    }

    public class TestRequestWithoutAuthorization : IRequest<TestResponse>
    {
        public string? UserId { get; set; }
    }

    public class TestResponse
    {
    }
}