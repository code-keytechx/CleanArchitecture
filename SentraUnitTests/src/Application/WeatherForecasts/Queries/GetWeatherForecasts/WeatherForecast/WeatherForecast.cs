using System;
using Xunit;
using FluentAssertions;
using AutoFixture;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace CleanArchitecture.Application.Tests.WeatherForecasts.Queries.GetWeatherForecasts
{
    public class WeatherForecastTests
    {
        private readonly Fixture _fixture;

        public WeatherForecastTests()
        {
            _fixture = new Fixture();
        }

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public void WeatherForecast_WithValidData_CreatesInstance()
        {
            // Arrange
            var date = DateTime.Now;
            var temperatureC = 25;
            var summary = "Sunny";

            // Act
            var weatherForecast = new WeatherForecast
            {
                Date = date,
                TemperatureC = temperatureC,
                Summary = summary
            };

            // Assert
            weatherForecast.Date.Should().Be(date);
            weatherForecast.TemperatureC.Should().Be(temperatureC);
            weatherForecast.TemperatureF.Should().Be(77); // 32 + (25 / 0.5556) ≈ 77
            weatherForecast.Summary.Should().Be(summary);
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public void WeatherForecast_WithZeroTemperature_CreatesInstance()
        {
            // Arrange
            var date = DateTime.Now;
            var temperatureC = 0;
            var summary = "Freezing";

            // Act
            var weatherForecast = new WeatherForecast
            {
                Date = date,
                TemperatureC = temperatureC,
                Summary = summary
            };

            // Assert
            weatherForecast.Date.Should().Be(date);
            weatherForecast.TemperatureC.Should().Be(temperatureC);
            weatherForecast.TemperatureF.Should().Be(32); // 32 + (0 / 0.5556) = 32
            weatherForecast.Summary.Should().Be(summary);
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public void WeatherForecast_WithNegativeTemperature_CreatesInstance()
        {
            // Arrange
            var date = DateTime.Now;
            var temperatureC = -10;
            var summary = "Freezing";

            // Act
            var weatherForecast = new WeatherForecast
            {
                Date = date,
                TemperatureC = temperatureC,
                Summary = summary
            };

            // Assert
            weatherForecast.Date.Should().Be(date);
            weatherForecast.TemperatureC.Should().Be(temperatureC);
            weatherForecast.TemperatureF.Should().Be(14); // 32 + (-10 / 0.5556) ≈ 14
            weatherForecast.Summary.Should().Be(summary);
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public void WeatherForecast_WithNullSummary_CreatesInstance()
        {
            // Arrange
            var date = DateTime.Now;
            var temperatureC = 25;
            string? summary = null;

            // Act
            var weatherForecast = new WeatherForecast
            {
                Date = date,
                TemperatureC = temperatureC,
                Summary = summary
            };

            // Assert
            weatherForecast.Date.Should().Be(date);
            weatherForecast.TemperatureC.Should().Be(temperatureC);
            weatherForecast.TemperatureF.Should().Be(77); // 32 + (25 / 0.5556) ≈ 77
            weatherForecast.Summary.Should().Be(summary);
        }

        #endregion
    }
}