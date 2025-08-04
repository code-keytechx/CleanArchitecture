using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace CleanArchitecture.Tests.Behaviors
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<IUser> _user;
        private readonly AuthorizationBehaviour<object, object> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _user = new Mock<IUser>();
            _behaviour = new AuthorizationBehaviour<object, object>(_httpContextAccessor.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithAuthenticatedUser_PassesThrough()
        {
            // Arrange
            _user.SetupGet(u => u.Id).Returns("authenticated_user_id");

            // Act
            Func<Task> act = () => _behaviour.Handle(null, CancellationToken.None);

            // Assert
            await act.Should().NotThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithNullUserId_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _user.SetupGet(u => u.Id).Returns((string)null);

            // Act
            Func<Task> act = () => _behaviour.Handle(null, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                      .WithMessage("Unauthorized Access");
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithEmptyUserId_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _user.SetupGet(u => u.Id).Returns("");

            // Act
            Func<Task> act = () => _behaviour.Handle(null, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                      .WithMessage("Unauthorized Access");
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenHttpContextIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            _httpContextAccessor.SetupGet(hca => hca.HttpContext).Returns((HttpContext)null);

            // Act
            Func<Task> act = () => _behaviour.Handle(null, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                      .WithParameterName("_httpContextAccessor.HttpContext");
        }

        #endregion

        #region Helper Methods

        private HttpContext CreateHttpContext(IUser user)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(user.Claims.ToArray()));
            return httpContext;
        }

        #endregion
    }
}