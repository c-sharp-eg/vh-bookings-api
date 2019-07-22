using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.API.Controllers;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Controllers
{
    
    public class CheckServiceHealthTests
    {
        private HealthCheckController _controller;
        private Mock<IQueryHandler> _quryHandlerMock;
        [SetUp]
        public void Setup()
        {
            _quryHandlerMock = new Mock<IQueryHandler>();
            _controller = new HealthCheckController(_quryHandlerMock.Object);
        }
        
        [Test]
        public async Task should_return_server_error_when_service_is_unhealthy()
        {
            _quryHandlerMock
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(new List<HearingVenue>());

            var result = await _controller.CheckServiceHealth();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            var response = (BookingsApiHealthResponse) typedResult.Value;
            response.DatabaseHealth.Successful.Should().BeFalse();
            response.DatabaseHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }
        
        [Test]
        public async Task should_return_ok_when_service_is_healthy()
        {
            _quryHandlerMock
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(new RefDataBuilder().HearingVenues);

            var result = await _controller.CheckServiceHealth();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.OK);
            var response = (BookingsApiHealthResponse) typedResult.Value;
            response.DatabaseHealth.Successful.Should().BeTrue();
            response.DatabaseHealth.ErrorMessage.Should().BeNullOrWhiteSpace();
            response.DatabaseHealth.Data.Should().BeNullOrEmpty();
        }
    }
}