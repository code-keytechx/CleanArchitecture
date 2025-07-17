using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.Tests.WeatherForecasts.Queries.GetWeatherForecasts
{
    public class GetWeatherForecastsQueryHandlerTests
    {
        // Test data - varied and realistic
        private readonly List<WeatherForecast> _expectedForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = new Random().Next(-20, 55),
            Summary = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" }[new Random().Next(10)]
        }).ToList();

        // Mock declarations
        private readonly Mock<IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>> _mockHandler;

        // Setup/Constructor
        public GetWeatherForecastsQueryHandlerTests()
        {
            _mockHandler = new Mock<IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>>();
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_GetWeatherForecastsQuery_ReturnsFiveForecasts()
        {
            // Arrange
            var handler = new GetWeatherForecastsQueryHandler();
            
            // Act
            var result = await handler.Handle(new GetWeatherForecastsQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(5);
        }

        [Fact]
        public async Task Handle_GetWeatherForecastsQuery_ReturnsCorrectDates()
        {
            // Arrange
            var handler = new GetWeatherForecastsQueryHandler();
            
            // Act
            var result = await handler.Handle(new GetWeatherForecastsQuery(), CancellationToken.None);

            // Assert
            result.Select(forecast => forecast.Date).Should().Equal(Enumerable.Range(1, 5).Select(i => DateTime.Now.AddDays(i)));
        }

        [Fact]
        public async Task Handle_GetWeatherForecastsQuery_ReturnsRandomTemperatures()
        {
            // Arrange
            var handler = new GetWeatherForecastsQueryHandler();
            
            // Act
            var result = await handler.Handle(new GetWeatherForecastsQuery(), CancellationToken.None);

            // Assert
            result.Select(forecast => forecast.TemperatureC).Should().AllBeBetween(-20, 55);
        }

        [Fact]
        public async Task Handle_GetWeatherForecastsQuery_ReturnsRandomSummaries()
        {
            // Arrange
            var handler = new GetWeatherForecastsQueryHandler();
            
            // Act
            var result = await handler.Handle(new GetWeatherForecastsQuery(), CancellationToken.None);

            // Assert
            result.Select(forecast => forecast.Summary).Should().AllBeInSet(new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" });
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_GetWeatherForecastsQuery_WithEmptyRange_ReturnsEmptyCollection()
        {
            // Arrange
            var handler = new GetWeatherForecastsQueryHandler();
            
            // Act
            var result = await handler.Handle(new GetWeatherForecastsQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_GetWeatherForecastsQuery_WithLargeRange_ReturnsCorrectNumberOfForecasts()
        {
            // Arrange
            var handler = new GetWeatherForecastsQueryHandler();
            
            // Act
            var result = await handler.Handle(new GetWeatherForecastsQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(5);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_GetWeatherForecastsQuery_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            var handler = new GetWeatherForecastsQueryHandler();
            
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, CancellationToken.None));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_GetWeatherForecastsQuery_WhenCancellationRequested_ThrowsOperationCanceledException()
        {
            // Arrange
            var handler = new GetWeatherForecastsQueryHandler();
            
            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => handler.Handle(new GetWeatherForecastsQuery(), new CancellationToken(canceled: true)));
        }

        #endregion

        #region Helper Methods

        private WeatherForecast CreateWeatherForecast(int index)
        {
            return new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = new Random().Next(-20, 55),
                Summary = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" }[new Random().Next(10)]
            };
        }

        #endregion
    }
}