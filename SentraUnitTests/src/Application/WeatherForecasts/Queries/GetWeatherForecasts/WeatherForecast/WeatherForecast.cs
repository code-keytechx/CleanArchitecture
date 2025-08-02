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

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public void WeatherForecast_WithValidData_CalculatesTemperatureFCorrectly()
        {
            // Business Context: Incorrect temperature conversion can lead to incorrect weather forecasts
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
            weatherForecast.TemperatureF.Should().Be(77, "TemperatureF should be correctly calculated from TemperatureC");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public void WeatherForecast_WithValidData_SetsPropertiesCorrectly()
        {
            // Business Context: Ensure that all properties are set correctly
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
            weatherForecast.Summary.Should().Be(summary);
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public void WeatherForecast_WithZeroTemperatureC_CalculatesTemperatureFAs32()
        {
            // Business Context: Ensure that zero temperature in Celsius is correctly converted to Fahrenheit
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
            weatherForecast.TemperatureF.Should().Be(32, "TemperatureF should be 32 when TemperatureC is 0");
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public void WeatherForecast_WithNegativeTemperatureC_CalculatesTemperatureFCorrectly()
        {
            // Business Context: Ensure that negative temperatures in Celsius are correctly converted to Fahrenheit
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
            weatherForecast.TemperatureF.Should().Be(14, "TemperatureF should be correctly calculated from negative TemperatureC");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public void WeatherForecast_WithNullSummary_DoesNotThrow()
        {
            // Business Context: Ensure that null summary does not cause any issues
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
            weatherForecast.Summary.Should().BeNull("Summary should be null without throwing any exceptions");
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        // No exception handling tests needed for this class as it is a simple data model

        #endregion
    }
}