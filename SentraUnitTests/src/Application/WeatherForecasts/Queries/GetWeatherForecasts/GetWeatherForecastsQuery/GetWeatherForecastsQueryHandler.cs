using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using FluentAssertions;
using MediatR;
using Xunit;

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
        public async Task Handle_WithValidRequest_ReturnsFiveForecasts()
        {
            // Business Context: Ensuring the core functionality of retrieving weather forecasts works as expected
            // Arrange
            var request = new GetWeatherForecastsQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().HaveCount(5, "five forecasts should be returned");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithValidRequest_ReturnsForecastsWithValidDates()
        {
            // Business Context: Ensuring the dates in the forecasts are correctly calculated
            // Arrange
            var request = new GetWeatherForecastsQuery();
            var today = DateTime.Now;

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().AllSatisfy(forecast =>
            {
                forecast.Date.Should().BeAfter(today, "each forecast date should be after today");
                forecast.Date.Should().BeBefore(today.AddDays(6), "each forecast date should be before today plus 5 days");
            });
        }

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithValidRequest_ReturnsForecastsWithValidTemperatures()
        {
            // Business Context: Ensuring the temperatures in the forecasts are within the expected range
            // Arrange
            var request = new GetWeatherForecastsQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().AllSatisfy(forecast =>
            {
                forecast.TemperatureC.Should().BeGreaterOrEqualTo(-20, "temperature should be at least -20C");
                forecast.TemperatureC.Should().BeLessOrEqualTo(55, "temperature should be at most 55C");
            });
        }

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithValidRequest_ReturnsForecastsWithValidSummaries()
        {
            // Business Context: Ensuring the summaries in the forecasts are within the expected set
            // Arrange
            var request = new GetWeatherForecastsQuery();
            var expectedSummaries = new HashSet<string>
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().AllSatisfy(forecast =>
            {
                expectedSummaries.Should().Contain(forecast.Summary, "summary should be one of the expected values");
            });
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithValidRequest_ReturnsForecastsWithUniqueDates()
        {
            // Business Context: Ensuring the dates in the forecasts are unique
            // Arrange
            var request = new GetWeatherForecastsQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Select(forecast => forecast.Date.Date).Should().OnlyHaveUniqueItems("each forecast date should be unique");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        // No negative tests applicable as the method does not accept any parameters that can be invalid

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        // No exception handling tests applicable as the method does not throw any exceptions

        #endregion
    }
}