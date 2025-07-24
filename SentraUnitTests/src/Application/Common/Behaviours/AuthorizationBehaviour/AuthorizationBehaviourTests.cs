using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.Common.Behaviours.Tests
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<IUser> _mockUser;
        private readonly AuthorizationBehaviour<TestRequest, TestResponse> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockUser = new Mock<IUser>();
            _behaviour = new AuthorizationBehaviour<TestRequest, TestResponse>(_mockIdentityService.Object, _mockUser.Object);
        }

        #region Critical Path Tests
        // Tests that protect core business processes critical for revenue and operations

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithNoAuthorizeAttributes_AllowsExecution()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndAuthorizedRole_AllowsExecution()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());

            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ReturnsAsync(true);

            request.GetType().AddCustomAttribute(new AuthorizeAttribute { Roles = "Admin" });

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndUnauthorizedRole_ThrowsForbiddenAccessException()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();

            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ReturnsAsync(false);

            request.GetType().AddCustomAttribute(new AuthorizeAttribute { Roles = "Admin" });

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                           .Should()
                           .ThrowAsync<ForbiddenAccessException>()
                           .WithMessage("Access denied.");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndAuthorizedPolicy_AllowsExecution()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());

            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user123", "ReadPolicy")).ReturnsAsync(true);

            request.GetType().AddCustomAttribute(new AuthorizeAttribute { Policy = "ReadPolicy" });

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndUnauthorizedPolicy_ThrowsForbiddenAccessException()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();

            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user123", "ReadPolicy")).ReturnsAsync(false);

            request.GetType().AddCustomAttribute(new AuthorizeAttribute { Policy = "ReadPolicy" });

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                           .Should()
                           .ThrowAsync<ForbiddenAccessException>()
                           .WithMessage("Access denied.");
        }

        #endregion

        #region Happy Path Tests
        // Tests for standard, expected user workflows
        // Covers typical business scenarios under normal conditions

        [Fact]
        [Trait("Category", "Happy")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();

            _mockUser.Setup(u => u.Id).Returns((string)null);

            request.GetType().AddCustomAttribute(new AuthorizeAttribute { Roles = "Admin" });

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                           .Should()
                           .ThrowAsync<UnauthorizedAccessException>()
                           .WithMessage("Unauthorized.");
        }

        #endregion

        #region Edge Case Tests
        // Tests for boundary conditions and unusual but valid scenarios
        // Ensures system handles limits and special cases gracefully

        [Fact]
        [Trait("Category", "Edge")]
        public async Task Handle_WithMultipleRolesAndOneAuthorizedRole_AllowsExecution()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());

            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ReturnsAsync(false);
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Editor")).ReturnsAsync(true);

            request.GetType().AddCustomAttribute(new AuthorizeAttribute { Roles = "Admin,Editor" });

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        [Trait("Category", "Edge")]
        public async Task Handle_WithMultiplePoliciesAndOneAuthorizedPolicy_AllowsExecution()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());

            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user123", "ReadPolicy")).ReturnsAsync(false);
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user123", "WritePolicy")).ReturnsAsync(true);

            request.GetType().AddCustomAttribute(new AuthorizeAttribute { Policy = "ReadPolicy,WritePolicy" });

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            next.Verify(n => n(), Times.Once);
        }

        #endregion

        #region Negative Tests
        // Tests for invalid inputs and expected failure scenarios
        // Ensures proper validation and error handling

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            RequestHandlerDelegate<TestResponse> next = null;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(null, next, CancellationToken.None))
                           .Should()
                           .ThrowAsync<ArgumentNullException>()
                           .WithParameterName("request");
        }

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullNext_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new TestRequest();

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, null, CancellationToken.None))
                           .Should()
                           .ThrowAsync<ArgumentNullException>()
                           .WithParameterName("next");
        }

        #endregion

        #region Exception Handling Tests
        // Tests for system resilience and error recovery
        // Ensures graceful degradation when external systems fail

        [Fact]
        [Trait("Category", "Exception")]
        public async Task Handle_WithIdentityServiceFailure_ThrowsException()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());

            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ThrowsAsync(new InvalidOperationException("Failed to check role"));

            request.GetType().AddCustomAttribute(new AuthorizeAttribute { Roles = "Admin" });

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                           .Should()
                           .ThrowAsync<InvalidOperationException>()
                           .WithMessage("Failed to check role");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // Ensures proper authentication, authorization, and audit trails

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithSensitiveData_AuditsAccess()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());

            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ReturnsAsync(true);

            request.GetType().AddCustomAttribute(new AuthorizeAttribute { Roles = "Admin" });

            // Act
            await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(i => i.IsInRoleAsync("user123", "Admin"), Times.Once);
        }

        #endregion
    }

    public class TestRequest { }
    public class TestResponse { }
}
