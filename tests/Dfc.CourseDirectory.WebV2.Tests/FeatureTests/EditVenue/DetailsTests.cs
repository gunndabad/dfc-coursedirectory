﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.CourseDirectory.WebV2.Features.EditVenue;
using FluentAssertions;
using FluentAssertions.Execution;
using FormFlow;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Dfc.CourseDirectory.WebV2.Tests.FeatureTests.EditVenue
{
    public class DetailsTests : MvcTestBase
    {
        public DetailsTests(CourseDirectoryApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task Get_ValidRequestNoExistingFormFlowInstance_RendersExpectedOutputFromDatabase()
        {
            // Arrange
            var providerId = await TestData.CreateProvider();
            var venueId = await TestData.CreateVenue(
                providerId,
                venueName: "Test Venue",
                email: "test-venue@provider.com",
                telephone: "02079460000",
                website: "provider.com/test-venue",
                addressLine1: "Test Venue line 1",
                town: "Town",
                postcode: "AB1 2DE");

            var request = new HttpRequestMessage(HttpMethod.Get, $"venues/{venueId}");

            // Act
            var response = await HttpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using (new AssertionScope())
            {
                var doc = await response.GetDocument();
                doc.GetSummaryListValueWithKey("Location name").Should().Be("Test Venue");
                Assert.Equal(
                    "Test Venue line 1\nTown\nAB1 2DE",
                    doc.GetSummaryListValueWithKey("Address"));
                doc.GetSummaryListValueWithKey("Email").Should().Be("test-venue@provider.com");
                doc.GetSummaryListValueWithKey("Phone").Should().Be("020 7946 0000");
                doc.GetSummaryListValueWithKey("Website").Should().Be("provider.com/test-venue");
            }
        }

        [Fact]
        public async Task Get_ValidRequestWithExistingFormFlowInstance_RendersExpectedOutputFromState()
        {
            // Arrange
            var providerId = await TestData.CreateProvider();
            var venueId = await TestData.CreateVenue(providerId);

            var formFlowInstance = await CreateFormFlowInstance(venueId);
            formFlowInstance.UpdateState(state =>
            {
                state.Name = "Updated name";
                state.Email = "updated@provider.com";
                state.PhoneNumber = "02079460000";
                state.Website = "updated-provider.com";
                state.AddressLine1 = "Updated line 1";
                state.AddressLine2 = "Updated line 2";
                state.Town = "Updated town";
                state.County = "Updated county";
                state.Postcode = "UP1 D8D";
                state.Latitude = 42;
                state.Longitude = 42;
                state.NewAddressIsOutsideOfEngland = false;
            });

            var request = new HttpRequestMessage(HttpMethod.Get, $"venues/{venueId}");

            // Act
            var response = await HttpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using (new AssertionScope())
            {
                var doc = await response.GetDocument();
                doc.GetSummaryListValueWithKey("Location name").Should().Be("Updated name");
                Assert.Equal(
                    "Updated line 1\nUpdated line 2\nUpdated town\nUpdated county\nUP1 D8D",
                    doc.GetSummaryListValueWithKey("Address"));
                doc.GetSummaryListValueWithKey("Email").Should().Be("updated@provider.com");
                doc.GetSummaryListValueWithKey("Phone").Should().Be("020 7946 0000");
                doc.GetSummaryListValueWithKey("Website").Should().Be("updated-provider.com");
                Assert.Null(doc.GetElementByTestId("outsideofengland-notification"));
            }
        }

        [Fact]
        public async Task Get_NewAddressOutsideOfEngland_RendersExpectedOutput()
        {
            // Arrange
            var providerId = await TestData.CreateProvider();
            var venueId = await TestData.CreateVenue(providerId);

            var formFlowInstance = await CreateFormFlowInstance(venueId);
            formFlowInstance.UpdateState(state =>
            {
                state.AddressLine1 = "82 George Square";
                state.AddressLine2 = "";
                state.Town = "Glasgow";
                state.County = "";
                state.Postcode = "G2 1DU";
                state.Latitude = 55.861038M;
                state.Longitude = -4.245402M;
                state.NewAddressIsOutsideOfEngland = true;
            });

            var request = new HttpRequestMessage(HttpMethod.Get, $"venues/{venueId}");

            // Act
            var response = await HttpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var doc = await response.GetDocument();
            Assert.NotNull(doc.GetElementByTestId("outsideofengland-notification"));
        }

        private async Task<FormFlowInstance<EditVenueFlowModel>> CreateFormFlowInstance(Guid venueId)
        {
            var state = await Factory.Services.GetRequiredService<EditVenueFlowModelFactory>()
                .CreateModel(venueId);

            return CreateFormFlowInstanceForRouteParameters(
                key: "EditVenue",
                routeParameters: new Dictionary<string, object>()
                {
                    { "venueId", venueId }
                },
                state);
        }
    }
}
