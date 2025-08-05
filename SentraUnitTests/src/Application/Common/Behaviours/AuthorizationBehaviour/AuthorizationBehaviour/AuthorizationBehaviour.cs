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
        private readonly AuthorizationBehaviour<MockRequest, MockResponse> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockUser = new Mock<IUser>();
            _mockIdentityService = new Mock<IIdentityService>();
            _behaviour = new AuthorizationBehaviour<MockRequest, MockResponse>(_mockUser.Object, _mockIdentityService.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthorizedUser_ReturnsResponse()
        {
            // Business Context: Authorized users should be able to proceed with their requests.
            // Arrange
            var request = new MockRequest { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.IsInRoleAsync(request.UserId, "Admin")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new MockResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("authorized user should receive a response");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthenticated users should not be able to proceed with their requests.
            // Arrange
            var request = new MockRequest { UserId = null };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new MockResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthorizedRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Users without the required role should not be able to proceed with their requests.
            // Arrange
            var request = new MockRequest { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.IsInRoleAsync(request.UserId, "Admin")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new MockResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthorizedPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Users without the required policy should not be able to proceed with their requests.
            // Arrange
            var request = new MockRequest { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.AuthorizeAsync(request.UserId, "AdminPolicy")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new MockResponse()), CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithNoAuthorizationAttributes_ReturnsResponse()
        {
            // Business Context: Requests without authorization attributes should proceed without checks.
            // Arrange
            var request = new MockRequestWithoutAuthorization { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new MockResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("request without authorization attributes should receive a response");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithMultipleRoles_ReturnsResponseIfAnyRoleMatches()
        {
            // Business Context: Requests with multiple roles should succeed if the user is in any of the roles.
            // Arrange
            var request = new MockRequest { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.IsInRoleAsync(request.UserId, "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(s => s.IsInRoleAsync(request.UserId, "User")).ReturnsAsync(true);

            // Act
            var response = await _behaviour.Handle(request, () => Task.FromResult(new MockResponse()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull("authorized user with any role should receive a response");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullUserId_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Requests with null user ID should fail due to unauthorized access.
            // Arrange
            var request = new MockRequest { UserId = null };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new MockResponse()), CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInIsInRoleAsync_ThrowsException()
        {
            // Business Context: Requests should fail gracefully if an exception occurs during role check.
            // Arrange
            var request = new MockRequest { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.IsInRoleAsync(request.UserId, "Admin")).ThrowsAsync(new Exception("Role check failed"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new MockResponse()), CancellationToken.None))
                .Should().ThrowAsync<Exception>();
        }

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInAuthorizeAsync_ThrowsException()
        {
            // Business Context: Requests should fail gracefully if an exception occurs during policy check.
            // Arrange
            var request = new MockRequest { UserId = "test-user-123" };
            _mockUser.Setup(u => u.Id).Returns(request.UserId);
            _mockIdentityService.Setup(s => s.AuthorizeAsync(request.UserId, "AdminPolicy")).ThrowsAsync(new Exception("Policy check failed"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, () => Task.FromResult(new MockResponse()), CancellationToken.None))
                .Should().ThrowAsync<Exception>();
        }

        #endregion

        // Helper classes for testing
        [Authorize(Roles = "Admin")]
        public class MockRequest : IRequest<MockResponse>
        {
            public string? UserId { get; set; }
        }

        [Authorize(Policy = "AdminPolicy")]
        public class MockRequestWithPolicy : IRequest<MockResponse>
        {
            public string? UserId { get; set; }
        }

        [Authorize(Roles = "Admin,User")]
        public class MockRequestWithMultipleRoles : IRequest<MockResponse>
        {
            public string? UserId { get; set; }
        }

        public class MockRequestWithoutAuthorization : IRequest<MockResponse>
        {
            public string? UserId { get; set; }
        }

        public class MockResponse { }
    }
}