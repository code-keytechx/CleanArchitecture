using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Xunit;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace CleanArchitecture.Application.Tests.WeatherForecasts.Queries.GetWeatherForecasts
{
    public class GetWeatherForecastsQueryHandlerTests
    {
        private readonly GetWeatherForecastsQueryHandler _handler;

        public GetWeatherForecastsQueryHandlerTests()
        {
            _handler = new GetWeatherForecastsQueryHandler();
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithValidRequest_ReturnsWeatherForecasts()
        {
            // Business Context: Ensuring that the weather forecasts are generated correctly
            // Arrange
            var request = new GetWeatherForecastsQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty("should return a list of weather forecasts");
            result.Should().HaveCount(5, "should return exactly 5 weather forecasts");
            result.Should().AllSatisfy(forecast =>
            {
                forecast.Date.Should().BeAfter(DateTime.Now, "each forecast date should be after the current date");
                forecast.TemperatureC.Should().BeInRange(-20, 55, "each forecast temperature should be between -20 and 55");
                forecast.Summary.Should().BeOneOf(GetWeatherForecastsQueryHandler.Summaries, "each forecast summary should be one of the predefined summaries");
            });
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithDefaultRequest_ReturnsFiveForecasts()
        {
            // Business Context: Ensuring that the default request returns the correct number of forecasts
            // Arrange
            var request = new GetWeatherForecastsQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().HaveCount(5, "should return exactly 5 weather forecasts");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithDefaultRequest_ReturnsDatesInOrder()
        {
            // Business Context: Ensuring that the dates in the forecasts are in chronological order
            // Arrange
            var request = new GetWeatherForecastsQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Select(forecast => forecast.Date).Should().BeInAscendingOrder("dates should be in ascending order");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        // No negative tests applicable as the method does not accept any parameters that can be invalid

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        // No exception handling tests applicable as the method does not throw exceptions

        #endregion
    }
}
