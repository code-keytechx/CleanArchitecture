using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Tests.Unit.Common.Security
{
    public class AuthorizeAttributeTests
    {
        // Test data - varied and realistic
        private readonly List<AuthorizeAttribute> _testAttributes = new()
        {
            new AuthorizeAttribute(),
            new AuthorizeAttribute { Roles = "Admin" },
            new AuthorizeAttribute { Policy = "CanEdit" },
            new AuthorizeAttribute { Roles = "Admin,Editor", Policy = "CanDelete" }
        };

        // Mock declarations
        // None needed for this simple attribute class

        // Setup/Constructor
        public AuthorizeAttributeTests() { }

        #region Happy Path Tests

        [Fact]
        public void AuthorizeAttribute_CreatedWithoutParameters_HasEmptyRolesAndPolicy()
        {
            // Arrange
            var attribute = new AuthorizeAttribute();

            // Act & Assert
            attribute.Roles.Should().BeEmpty();
            attribute.Policy.Should().BeEmpty();
        }

        [Fact]
        public void AuthorizeAttribute_CreatedWithRoles_HasCorrectRoles()
        {
            // Arrange
            var attribute = new AuthorizeAttribute { Roles = "Admin" };

            // Act & Assert
            attribute.Roles.Should().Be("Admin");
            attribute.Policy.Should().BeEmpty();
        }

        [Fact]
        public void AuthorizeAttribute_CreatedWithPolicy_HasCorrectPolicy()
        {
            // Arrange
            var attribute = new AuthorizeAttribute { Policy = "CanEdit" };

            // Act & Assert
            attribute.Roles.Should().BeEmpty();
            attribute.Policy.Should().Be("CanEdit");
        }

        [Fact]
        public void AuthorizeAttribute_CreatedWithBothRolesAndPolicy_HasCorrectValues()
        {
            // Arrange
            var attribute = new AuthorizeAttribute { Roles = "Admin,Editor", Policy = "CanDelete" };

            // Act & Assert
            attribute.Roles.Should().Be("Admin,Editor");
            attribute.Policy.Should().Be("CanDelete");
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void AuthorizeAttribute_RolesWithWhitespace_SpacesRemoved()
        {
            // Arrange
            var attribute = new AuthorizeAttribute { Roles = " Admin , Editor " };

            // Act & Assert
            attribute.Roles.Should().Be("Admin,Editor");
            attribute.Policy.Should().BeEmpty();
        }

        [Fact]
        public void AuthorizeAttribute_PolicyWithWhitespace_SpacesRemoved()
        {
            // Arrange
            var attribute = new AuthorizeAttribute { Policy = " CanEdit " };

            // Act & Assert
            attribute.Roles.Should().BeEmpty();
            attribute.Policy.Should().Be("CanEdit");
        }

        [Fact]
        public void AuthorizeAttribute_RolesWithCommaAtEnd_Removed()
        {
            // Arrange
            var attribute = new AuthorizeAttribute { Roles = "Admin," };

            // Act & Assert
            attribute.Roles.Should().Be("Admin");
            attribute.Policy.Should().BeEmpty();
        }

        [Fact]
        public void AuthorizeAttribute_PolicyWithCommaAtEnd_Removed()
        {
            // Arrange
            var attribute = new AuthorizeAttribute { Policy = "CanEdit," };

            // Act & Assert
            attribute.Roles.Should().BeEmpty();
            attribute.Policy.Should().Be("CanEdit");
        }

        #endregion

        #region Negative Tests

        [Fact]
        public void AuthorizeAttribute_RolesWithNull_ThrowsArgumentNullException()
        {
            // Arrange
            Action action = () => new AuthorizeAttribute { Roles = null! };

            // Act & Assert
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "roles");
        }

        [Fact]
        public void AuthorizeAttribute_PolicyWithNull_ThrowsArgumentNullException()
        {
            // Arrange
            Action action = () => new AuthorizeAttribute { Policy = null! };

            // Act & Assert
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "policy");
        }

        [Fact]
        public void AuthorizeAttribute_RolesWithEmptyString_ThrowsArgumentException()
        {
            // Arrange
            Action action = () => new AuthorizeAttribute { Roles = "" };

            // Act & Assert
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == "roles");
        }

        [Fact]
        public void AuthorizeAttribute_PolicyWithEmptyString_ThrowsArgumentException()
        {
            // Arrange
            Action action = () => new AuthorizeAttribute { Policy = "" };

            // Act & Assert
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == "policy");
        }

        #endregion

        #region Exception Tests

        [Fact]
        public void AuthorizeAttribute_RolesWithSpecialCharacters_ThrowsArgumentException()
        {
            // Arrange
            Action action = () => new AuthorizeAttribute { Roles = "Admin!@#" };

            // Act & Assert
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == "roles");
        }

        [Fact]
        public void AuthorizeAttribute_PolicyWithSpecialCharacters_ThrowsArgumentException()
        {
            // Arrange
            Action action = () => new AuthorizeAttribute { Policy = "CanEdit$%^&*()" };

            // Act & Assert
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == "policy");
        }

        #endregion

        #region Helper Methods

        private static void ValidateRoleSyntax(string roles)
        {
            roles.Split(',').Should().AllMatch(role => !string.IsNullOrWhiteSpace(role));
        }

        private static void ValidatePolicySyntax(string policy)
        {
            policy.Should().NotContainAny("!@#$%^&*()");
        }

        #endregion
    }
}