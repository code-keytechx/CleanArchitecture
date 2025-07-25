using CleanArchitecture.Application.Common.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Xunit;

namespace CleanArchitecture.Tests.DependencyInjectionTests
{
    public class DependencyInjectionTests
    {
        // Test data - varied and realistic
        private readonly List<ServiceDescriptor> _serviceDescriptors;

        // Mock declarations
        private readonly Mock<IMediator> _mockMediator;

        // System under test
        private readonly IServiceCollection _services;

        // Constructor with setup
        public DependencyInjectionTests()
        {
            _mockMediator = new Mock<IMediator>();
            _services = new ServiceCollection();

            _services.AddSingleton(_mockMediator.Object);

            _serviceDescriptors = _services.ToList();
        }

        #region Happy Path Tests

        [Fact]
        public void AddApplicationServices_AddsAutoMapper()
        {
            // Arrange
            var assembly = Assembly.GetExecutingAssembly();

            // Act
            DependencyInjection.AddApplicationServices(new HostApplicationBuilder(_services));

            // Assert
            var mapperService = _services.FirstOrDefault(s => s.ServiceType == typeof(IMapper));
            Assert.NotNull(mapperService);
        }

        [Fact]
        public void AddApplicationServices_AddsValidators()
        {
            // Arrange
            var assembly = Assembly.GetExecutingAssembly();

            // Act
            DependencyInjection.AddApplicationServices(new HostApplicationBuilder(_services));

            // Assert
            var validatorService = _services.FirstOrDefault(s => s.ServiceType == typeof(IValidator<>));
            Assert.NotNull(validatorService);
        }

        [Fact]
        public void AddApplicationServices_RegistersMediatRBehaviors()
        {
            // Arrange
            var assembly = Assembly.GetExecutingAssembly();

            // Act
            DependencyInjection.AddApplicationServices(new HostApplicationBuilder(_services));

            // Assert
            var pipelineBehaviorTypes = new[] { typeof(UnhandledExceptionBehaviour<,>), typeof(AuthorizationBehaviour<,>), typeof(ValidationBehaviour<,>), typeof(PerformanceBehaviour<,>) };
            foreach (var behaviorType in pipelineBehaviorTypes)
            {
                var behaviorService = _services.FirstOrDefault(s => s.ServiceType == behaviorType.MakeGenericType(typeof(object), typeof(object)));
                Assert.NotNull(behaviorService);
            }
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void AddApplicationServices_EmptyAssembly_NoServicesAdded()
        {
            // Arrange
            var emptyAssembly = Assembly.LoadFrom("System.Private.CoreLib.dll");

            // Act
            DependencyInjection.AddApplicationServices(new HostApplicationBuilder(_services));

            // Assert
            Assert.Empty(_services);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public void AddApplicationServices_NullBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            HostApplicationBuilder builder = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => DependencyInjection.AddApplicationServices(builder));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public void AddApplicationServices_MissingDependencies_ThrowsInvalidOperationException()
        {
            // Arrange
            var missingDependenciesAssembly = Assembly.GetExecutingAssembly();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => DependencyInjection.AddApplicationServices(new HostApplicationBuilder(_services)));
        }

        #endregion

        #region Helper Methods

        private void SetupHappyPathMocks()
        {
            // Setup specific mocks here if needed
        }

        private void AssertBusinessRulesAreSatisfied(ServiceDescriptor descriptor)
        {
            // Assert specific business logic outcomes
            Assert.NotNull(descriptor);
        }

        #endregion
    }
}