using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.Tests.WeatherForecasts.Queries.GetWeatherForecasts
{
    public class WeatherForecastQueryHandlerTests
    {
        // Test data - varied and realistic
        private readonly List<WeatherForecast> _weatherForecasts = new()
        {
            new() { Date = DateTime.Parse("2023-01-01"), TemperatureC = 10, Summary = "Sunny" },
            new() { Date = DateTime.Parse("2023-01-02"), TemperatureC = -5, Summary = "Rainy" },
            new() { Date = DateTime.Parse("2023-01-03"), TemperatureC = 25, Summary = "Cloudy" }
        };

        // Mock declarations
        private readonly Mock<IWeatherForecastService> _mockWeatherForecastService;

        // System under test
        private readonly GetWeatherForecastsQueryHandler _sut;

        // Constructor with setup
        public WeatherForecastQueryHandlerTests()
        {
            _mockWeatherForecastService = new Mock<IWeatherForecastService>();
            _mockWeatherForecastService.Setup(service => service.GetForecasts())
                .ReturnsAsync(_weatherForecasts);

            _sut = new GetWeatherForecastsQueryHandler(_mockWeatherForecastService.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task GetWeatherForecasts_WithNoFilters_ReturnsAllForecasts()
        {
            // Arrange
            var query = new GetWeatherForecastsQuery();

            // Act
            var result = await _sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(forecast => forecast.Date == DateTime.Parse("2023-01-01") && forecast.TemperatureC == 10);
            result.Should().Contain(forecast => forecast.Date == DateTime.Parse("2023-01-02") && forecast.TemperatureC == -5);
            result.Should().Contain(forecast => forecast.Date == DateTime.Parse("2023-01-03") && forecast.TemperatureC == 25);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task GetWeatherForecasts_WithEmptyFilter_ReturnsAllForecasts()
        {
            // Arrange
            var query = new GetWeatherForecastsQuery { Filter = "" };

            // Act
            var result = await _sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(forecast => forecast.Date == DateTime.Parse("2023-01-01") && forecast.TemperatureC == 10);
            result.Should().Contain(forecast => forecast.Date == DateTime.Parse("2023-01-02") && forecast.TemperatureC == -5);
            result.Should().Contain(forecast => forecast.Date == DateTime.Parse("2023-01-03") && forecast.TemperatureC == 25);
        }

        [Fact]
        public async Task GetWeatherForecasts_WithDateRangeFilter_ReturnsFilteredForecasts()
        {
            // Arrange
            var query = new GetWeatherForecastsQuery
            {
                StartDate = DateTime.Parse("2023-01-01"),
                EndDate = DateTime.Parse("2023-01-02")
            };

            // Act
            var result = await _sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(forecast => forecast.Date == DateTime.Parse("2023-01-01") && forecast.TemperatureC == 10);
            result.Should().Contain(forecast => forecast.Date == DateTime.Parse("2023-01-02") && forecast.TemperatureC == -5);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task GetWeatherForecasts_WithInvalidStartDate_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetWeatherForecastsQuery { StartDate = DateTime.Parse("2023-01-04") };

            // Act
            var result = await _sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetWeatherForecasts_WithEndDateBeforeStartDate_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetWeatherForecastsQuery
            {
                StartDate = DateTime.Parse("2023-01-02"),
                EndDate = DateTime.Parse("2023-01-01")
            };

            // Act
            var result = await _sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task GetWeatherForecasts_WhenServiceThrowsException_ThrowsBusinessException()
        {
            // Arrange
            var query = new GetWeatherForecastsQuery();
            _mockWeatherForecastService.Setup(service => service.GetForecasts())
                .ThrowsAsync(new InvalidOperationException("Service error"));

            // Act & Assert
            var exception = await FluentActions.Invoking(async () => await _sut.Handle(query, CancellationToken.None))
                .Should().ThrowAsync<BusinessException>();

            exception.Which.Message.Should().Be("An error occurred while fetching weather forecasts.");
            exception.Which.InnerException.Should().BeOfType<InvalidOperationException>();
        }

        #endregion

        #region Helper Methods

        private static GetWeatherForecastsQuery CreateQuery(string filter = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            return new GetWeatherForecastsQuery { Filter = filter, StartDate = startDate, EndDate = endDate };
        }

        #endregion
    }
}