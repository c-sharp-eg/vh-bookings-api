﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.API.Controllers;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Testing.Common.Assertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Controllers
{
    public class ParticipantsControllerTest
    {
        private ParticipantsController _controller;
        private Mock<IQueryHandler> _queryHandlerMock;

        [SetUp]
        public void Setup()
        {
            _queryHandlerMock = new Mock<IQueryHandler>();
            _controller = new ParticipantsController(_queryHandlerMock.Object);
        }

        [Test]
        public async Task Should_return_bad_request_when_username_is_not_a_valid_email()
        {
            const string username = "what";

            var result = await _controller.GetParticipantsByUsername(username);

            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(username), $"Please provide a valid {nameof(username)}");
        }

        [Test]
        public async Task Should_return_not_found_when_no_participants_found()
        {
            const string email = "real@email.com";

            var result = await _controller.GetParticipantsByUsername(email);

            result.Should().NotBeNull();
            var objectResult = result as NotFoundResult;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_mapped_participants()
        {
            const string email = "real@email.com";

            var caseRole = new CaseRole(1, "");
            var hearingRole = new HearingRole(1, "");

            var participants = new List<Participant>
            {
                new Individual
                (
                    new Person("mr", "test", "er", "test1@test.com"),
                    hearingRole,
                    caseRole
                ),
                new Individual
                (
                    new Person("mr", "test", "er", "test2@test.com"),
                    hearingRole,
                    caseRole
                )
            };

            participants[0].Person.Organisation = new Organisation("");
            participants[0].CaseRole = caseRole;
            participants[0].CaseRole.HearingRoles = new List<HearingRole>();
            participants[0].HearingRole = hearingRole;
            participants[0].HearingRole.UserRole = new UserRole(1, "");

            participants[1].Person.Organisation = new Organisation("");
            participants[1].CaseRole = caseRole;
            participants[1].CaseRole.HearingRoles = new List<HearingRole>();
            participants[1].HearingRole = hearingRole;
            participants[1].HearingRole.UserRole = new UserRole(1, "");


            _queryHandlerMock
                .Setup(x => x.Handle<GetParticipantsByUsernameQuery, List<Participant>>(It.IsAny<GetParticipantsByUsernameQuery>()))
                .ReturnsAsync(participants);

            var result = await _controller.GetParticipantsByUsername(email);

            result.Should().NotBeNull();
            var objectResult = result as OkObjectResult;
            var data = (IEnumerable<ParticipantResponse>)objectResult.Value;
            data.Count().Should().Be(2);
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}