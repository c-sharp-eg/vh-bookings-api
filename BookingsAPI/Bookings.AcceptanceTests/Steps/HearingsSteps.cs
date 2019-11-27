﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Models;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;
using Bookings.Api.Contract.Requests.Enums;
using UpdateBookingStatusRequest = Bookings.AcceptanceTests.Models.UpdateBookingStatusRequest;
using UpdateHearingRequest = Bookings.AcceptanceTests.Models.UpdateHearingRequest;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HearingsSteps
    {
        private const int CivilMoneyClaimsCaseType = 1;
        private readonly TestContext _context;
        private readonly HearingsEndpoints _endpoints = new ApiUriFactory().HearingsEndpoints;

        public HearingsSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get details for a given hearing request with a valid hearing id")]
        public void GivenIHaveAGetDetailsForAGivenHearingRequestWithAValidHearingId()
        {
            _context.Request = _context.Get(_endpoints.GetHearingDetailsById(_context.HearingId));
        }

        [Given(@"I have a valid book a new hearing request")]
        public void GivenIHaveAValidBookANewHearingRequest()
        {
            var bookNewHearingRequest = new CreateHearingRequestBuilder()
                .WithContext(_context)
                .Build();

            _context.Request = _context.Post(_endpoints.BookNewHearing(), bookNewHearingRequest);
        }

        [Given(@"I have a valid update hearing request")]
        public void GivenIHaveAValidUpdateHearingRequest()
        {
            var updateHearingRequest = UpdateHearingRequest.BuildRequest();
            _context.Request = _context.Put(_endpoints.UpdateHearingDetails(_context.HearingId), updateHearingRequest);
        }

        [Given(@"I have a valid get hearing by username request")]
        public void GivenIHaveAValidGetHearingByUsernameRequest()
        {
            _context.Request = _context.Get(_endpoints.GetHearingsByUsername(_context.Participants.First().Username));
        }

        [Given(@"I have a remove hearing request with a valid hearing id")]
        public void GivenIHaveARemoveHearingRequestWithAValidHearingId()
        {
            _context.Request = _context.Delete(_endpoints.RemoveHearing(_context.HearingId));
        }       

        [Then(@"hearing details should be retrieved")]
        public void ThenAHearingDetailsShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Json);
            model.Should().NotBeNull();
            _context.HearingId = model.Id;
            model.Should().BeEquivalentTo(_context.HearingRequest, o => o.Excluding(x => x.Participants));
            model.StreamingFlag.Should().Be(false);

            var expectedIndividuals = _context.HearingRequest.Participants.FindAll(x => x.HearingRoleName.Contains("Claimant") || x.HearingRoleName.Contains("Defendant"));
            var actualIndividuals = model.Participants.FindAll(x => x.HearingRoleName.Contains("Claimant") || x.HearingRoleName.Contains("Defendant"));
            expectedIndividuals.Should().BeEquivalentTo(actualIndividuals, o => o.ExcludingMissingMembers());

            var expectedRepresentatives = _context.HearingRequest.Participants.FindAll(x => x.HearingRoleName.Contains("Solicitor"));
            var actualRepresentatives = model.Participants.FindAll(x => x.HearingRoleName.Contains("Solicitor"));
            ParticipantsDetailsMatch(expectedRepresentatives, actualRepresentatives);

            var expectedJudge = _context.HearingRequest.Participants.FindAll(x => x.HearingRoleName.Contains("Judge"));
            var actualJudge = model.Participants.FindAll(x => x.HearingRoleName.Contains("Judge"));
            ParticipantsDetailsMatch(expectedJudge, actualJudge);
        }

        private static void ParticipantsDetailsMatch(IEnumerable<ParticipantRequest> expected, IEnumerable<ParticipantResponse> actual)
        {
            expected.Should().BeEquivalentTo(actual, o =>
            {
                o.Excluding(address => address.HouseNumber);
                o.Excluding(address => address.Street);
                o.Excluding(address => address.City);
                o.Excluding(address => address.County);
                o.Excluding(address => address.Postcode);
                o.ExcludingMissingMembers();
                return o;
            });
        }

        [Then(@"hearing details should be updated")]
        public void ThenHearingDetailsShouldBeUpdated()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Json);
            model.Should().NotBeNull();
            model.ScheduledDuration.Should().Be(100);
            model.ScheduledDateTime.Should().Be(DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45).ToUniversalTime());
            model.HearingVenueName.Should().Be("Manchester Civil and Family Justice Centre");
            model.OtherInformation.Should().Be("OtherInformation12345");
            model.HearingRoomName.Should().Be("HearingRoomName12345");
            model.QuestionnaireNotRequired.Should().Be(true);

            foreach (var participant in model.Participants)
            {
                participant.CaseRoleName.Should().NotBeNullOrEmpty();
                participant.ContactEmail.Should().NotBeNullOrEmpty();
                participant.DisplayName.Should().NotBeNullOrEmpty();
                participant.FirstName.Should().NotBeNullOrEmpty();
                participant.HearingRoleName.Should().NotBeNullOrEmpty();
                participant.Id.Should().NotBeEmpty();
                participant.LastName.Should().NotBeNullOrEmpty();
                participant.MiddleNames.Should().NotBeNullOrEmpty();
                participant.TelephoneNumber.Should().NotBeNullOrEmpty();
                participant.Title.Should().NotBeNullOrEmpty();
                participant.UserRoleName.Should().NotBeNullOrEmpty();

                if (participant.UserRoleName.Equals("Individual"))
                {
                    participant.HouseNumber.Should().NotBeNullOrEmpty();
                    participant.Street.Should().NotBeNullOrEmpty();
                    participant.City.Should().NotBeNullOrEmpty();
                    participant.County.Should().NotBeNullOrEmpty();
                    participant.Postcode.Should().NotBeNullOrEmpty();
                }

                if (participant.UserRoleName.Equals("Representative"))
                {
                    participant.HouseNumber.Should().BeNull();
                    participant.Street.Should().BeNull();
                    participant.City.Should().BeNull();
                    participant.County.Should().BeNull();
                    participant.Postcode.Should().BeNull();
                }
            }
        }

        [Then(@"the hearing no longer exists")]
        public void ThenTheHearingNoLongerExists()
        {
            _context.Request = _context.Get(_endpoints.GetHearingDetailsById(_context.HearingId));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Given(@"I have a valid book a new hearing for a case type (.*)")]
        public void GivenIHaveAValidBookANewHearingForACaseType(string caseType)
        {
            var request = new CreateHearingRequestBuilder().WithContext(_context).Build();
            request.ScheduledDateTime = DateTime.Now.AddDays(2);
            request.CaseTypeName = caseType;
            _context.Request = _context.Post(_endpoints.BookNewHearing(), request);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Json = _context.Response.Content;
            _context.Response.StatusCode.Should().Be(HttpStatusCode.Created);
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Json);
            model.Should().NotBeNull();
            _context.HearingId = model.Id;
        }

        [Given(@"I have a get details for a given hearing request with a valid case type")]
        public void GivenIHaveAGetDetailsForAGivenHearingRequestWithAValidCaseType()
        {

            _context.Request = _context.Get(_endpoints.GetHearingsByCaseType(CivilMoneyClaimsCaseType));
        }

        [Then(@"hearing details should be retrieved for the case type")]
        public void ThenHearingDetailsShouldBeRetrievedForTheCaseType()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<BookingsResponse>(_context.Json);
            model.PrevPageUrl.Should().Contain(model.Limit.ToString());
            var response = model.Hearings.SelectMany(u => u.Hearings).FirstOrDefault(x => x.ScheduledDateTime.Date == DateTime.UtcNow.AddDays(2).Date && x.HearingId == _context.HearingId);
            response.Should().NotBeNull();
            response.CaseTypeName.Should().NotBeNullOrEmpty();
            response.HearingTypeName.Should().NotBeNullOrEmpty();
            response.ScheduledDateTime.Should().BeAfter(DateTime.UtcNow);
            response.ScheduledDuration.Should().NotBe(0);
            response.JudgeName.Should().NotBeNullOrEmpty();
            response.CourtAddress.Should().NotBeNullOrEmpty();
            response.HearingName.Should().NotBeNullOrEmpty();
            response.HearingNumber.Should().NotBeNullOrEmpty();
            response.StreamingFlag.Should().Be(false);
        }

        [Given(@"I have a cancel hearing request with a valid hearing id")]
        public void GivenIHaveACancelHearingRequestWithAValidHearingId()
        {
            var updateHearingStatusRequest = UpdateBookingStatusRequest.BuildRequest(UpdateBookingStatus.Cancelled);
            _context.Request = _context.Patch(_endpoints.UpdateHearingDetails(_context.HearingId), updateHearingStatusRequest);
        }

        [Given(@"I have a created hearing request with a valid hearing id")]
        public void GivenIHaveACreatedHearingRequestWithAValidHearingId()
        {
            var updateHearingStatusRequest = UpdateBookingStatusRequest.BuildRequest(UpdateBookingStatus.Created);
            _context.Request = _context.Patch(_endpoints.UpdateHearingDetails(_context.HearingId), updateHearingStatusRequest);
        }

        [Then(@"hearing should be created")]
        public void ThenHearingShouldBeCreated()
        {
            _context.Request = _context.Get(_endpoints.GetHearingDetailsById(_context.HearingId));
            _context.Response = _context.Client().Execute(_context.Request);
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Response.Content);
            model.UpdatedBy.Should().NotBeNullOrEmpty();
            model.Status.Should().Be(Domain.Enumerations.BookingStatus.Created);
        }

        [Then(@"hearing should be cancelled")]
        public void ThenHearingShouldBeCancelled()
        {
            _context.Request = _context.Get(_endpoints.GetHearingDetailsById(_context.HearingId));
            _context.Response = _context.Client().Execute(_context.Request);
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Response.Content);
            model.UpdatedBy.Should().NotBeNullOrEmpty();
            model.Status.Should().Be(Domain.Enumerations.BookingStatus.Cancelled);
        }

        [Then(@"a list of hearing details should be retrieved")]
        public void ThenAListOfHearingDetailsShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingDetailsResponse>>(_context.Json);
            model.Should().NotBeNull();
            _context.HearingId = model.First().Id;

            foreach (var hearing in model)
            {
                hearing.CaseTypeName.Should().NotBeNullOrEmpty();
                foreach (var theCase in hearing.Cases)
                {
                    theCase.Name.Should().NotBeNullOrEmpty();
                    theCase.Number.Should().NotBeNullOrEmpty();
                }
                hearing.HearingTypeName.Should().NotBeNullOrEmpty();
                hearing.HearingVenueName.Should().NotBeNullOrEmpty();
                foreach (var participant in hearing.Participants)
                {
                    participant.CaseRoleName.Should().NotBeNullOrEmpty();
                    participant.ContactEmail.Should().NotBeNullOrEmpty();
                    participant.DisplayName.Should().NotBeNullOrEmpty();
                    participant.FirstName.Should().NotBeNullOrEmpty();
                    participant.HearingRoleName.Should().NotBeNullOrEmpty();
                    participant.Id.Should().NotBeEmpty();
                    participant.LastName.Should().NotBeNullOrEmpty();
                    participant.MiddleNames.Should().NotBeNullOrEmpty();
                    participant.TelephoneNumber.Should().NotBeNullOrEmpty();
                    participant.Title.Should().NotBeNullOrEmpty();
                    participant.UserRoleName.Should().NotBeNullOrEmpty();

                    if (participant.UserRoleName.Equals("Individual"))
                    {
                        participant.HouseNumber.Should().NotBeNullOrEmpty();
                        participant.Street.Should().NotBeNullOrEmpty();
                        participant.City.Should().NotBeNullOrEmpty();
                        participant.County.Should().NotBeNullOrEmpty();
                        participant.Postcode.Should().NotBeNullOrEmpty();
                    }

                    if (!participant.UserRoleName.Equals("Representative")) continue;
                    participant.HouseNumber.Should().BeNull();
                    participant.Street.Should().BeNull();
                    participant.City.Should().BeNull();
                    participant.County.Should().BeNull();
                    participant.Postcode.Should().BeNull();
                }
                hearing.ScheduledDateTime.Should().BeAfter(DateTime.MinValue);
                hearing.ScheduledDuration.Should().BePositive();
                hearing.HearingRoomName.Should().NotBeNullOrEmpty();
                hearing.OtherInformation.Should().NotBeNullOrEmpty();
                hearing.CreatedBy.Should().NotBeNullOrEmpty();
                hearing.StreamingFlag.Should().Be(false);
            }          
        }
    }
}