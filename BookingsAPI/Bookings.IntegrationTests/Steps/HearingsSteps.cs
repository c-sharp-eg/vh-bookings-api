﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Requests.Enums;
using Bookings.Api.Contract.Responses;
using Bookings.DAL;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Bookings.IntegrationTests.Contexts;
using Bookings.IntegrationTests.Helper;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class HearingsSteps : StepsBase
    {
        private readonly Contexts.TestContext _apiTestContext;
        private readonly HearingsEndpoints _endpoints = new ApiUriFactory().HearingsEndpoints;
        private Guid _hearingId;

        public HearingsSteps(Contexts.TestContext apiTestContext)
        {
            _apiTestContext = apiTestContext;
            _hearingId = Guid.Empty;
        }

        [Given(@"I have a get details for a given hearing request with a (.*) hearing id")]
        [Given(@"I have a get details for a given hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAGetDetailsForGivenHearingRequest(Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        _hearingId = seededHearing.Id;
                        break;
                    }
                case Scenario.Nonexistent:
                    _hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    _hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetHearingDetailsById(_hearingId);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a (.*) book a new hearing request")]
        [Given(@"I have an (.*) book a new hearing request")]
        public void GivenIHaveAValidBookANewHearingRequest(Scenario scenario)
        {
            var request = BuildRequest();
            if (scenario == Scenario.Invalid)
            {
                request.Cases = new List<CaseRequest>();
                request.Participants = new List<ParticipantRequest>();
                request.ScheduledDuration = -100;
                request.CaseTypeName = string.Empty;
                request.HearingTypeName = string.Empty;
                request.HearingVenueName = string.Empty;
                request.ScheduledDateTime = DateTime.Today.AddDays(-5);
            }
            if (scenario == Scenario.Valid)
            {
                request.ScheduledDateTime = DateTime.Today.AddDays(1);
            }
            CreateTheNewHearingRequest(request);
        }

        [Given(@"I have a book a new hearing request with an invalid (.*)")]
        public void GivenIHaveABookANewHearingRequestWithAnInvalidHearingType(string invalidType)
        {
            var request = BuildRequest();
            switch (invalidType)
            {
                case "case type":
                    {
                        request.CaseTypeName = "Random";
                        break;
                    }
                case "hearing type":
                    {
                        request.HearingTypeName = "A nonexistent hearing type";
                        break;
                    }
                case "venue name":
                    {
                        request.HearingVenueName = "Random";
                        break;
                    }
                case "address":

                    {
                        var individualRoles = _apiTestContext.TestDataManager.GetIndividualHearingRoles;
                        foreach (ParticipantRequest participantRequest in request.Participants)
                        {
                            if (individualRoles.Contains(participantRequest.HearingRoleName))
                            {
                                participantRequest.County = string.Empty;
                                participantRequest.Street = string.Empty;
                                participantRequest.City = string.Empty;
                                participantRequest.HouseNumber = string.Empty;
                                participantRequest.Postcode = string.Empty;
                            }
                        }


                        break;
                    }
                default: throw new ArgumentOutOfRangeException(invalidType, invalidType, null);
            }
            CreateTheNewHearingRequest(request);
        }

        [Given(@"I have a update hearing request with a nonexistent hearing id")]
        public void GivenIHaveAUpdateHearingRequestWithANonexistentHearingId()
        {
            _hearingId = Guid.NewGuid();
            _apiTestContext.UpdateHearingRequest = BuildUpdateHearingRequestRequest();
            UpdateTheHearingRequest();
        }

        [Given(@"I have a (.*) update hearing request")]
        [Given(@"I have an (.*) update hearing request")]
        public async Task GivenIHaveAUpdateHearingRequest(Scenario scenario)
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _hearingId = seededHearing.Id;
            _apiTestContext.UpdateHearingRequest = BuildUpdateHearingRequestRequest();
            if (scenario == Scenario.Invalid)
            {
                _apiTestContext.UpdateHearingRequest.HearingVenueName = string.Empty;
                _apiTestContext.UpdateHearingRequest.ScheduledDuration = 0;
                _apiTestContext.UpdateHearingRequest.ScheduledDateTime = DateTime.Now.AddDays(-5);
            }
            UpdateTheHearingRequest();
        }

        [Given(@"I have a update hearing request with an invalid (.*)")]
        public async Task GivenIHaveAUpdateHearingRequestWithANonexistentHearingId(string invalidType)
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _hearingId = invalidType.Equals("hearing id") ? Guid.Empty : seededHearing.Id;
            _apiTestContext.UpdateHearingRequest = BuildUpdateHearingRequestRequest();
            if (invalidType.Equals("venue"))
            {
                _apiTestContext.UpdateHearingRequest.HearingVenueName = "Random";
            }
            UpdateTheHearingRequest();
        }

        [Given(@"I have a (.*) remove hearing request")]
        [Given(@"I have an (.*) remove hearing request")]
        public async Task GivenIHaveAValidRemoveHearingRequest(Scenario scenario)
        {
            await SetHearingIdForGivenScenario(scenario);
            _apiTestContext.Uri = _endpoints.RemoveHearing(_hearingId);
            _apiTestContext.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a (.*) get hearings by username request")]
        [Given(@"I have an (.*) get hearings by username request")]
        public async Task GivenIHaveAGetHearingByUsernameRequest(Scenario scenario)
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.NewHearingId = seededHearing.Id;
            _hearingId = seededHearing.Id;
            string username;
            switch (scenario)
            {
                case Scenario.Valid:
                    username = seededHearing.GetParticipants().First().Person.Username;
                    break;
                case Scenario.Nonexistent:
                    username = "madeupusername@nonexistent.com";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _apiTestContext.Uri = _endpoints.GetHearingsByUsername(username);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a request to the get booked hearings endpoint")]
        public async Task GivenIHaveARequestToTheGetBookedHearingsEndpoint()
        {
            await _apiTestContext.TestDataManager.SeedVideoHearing();
            await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.Uri = _endpoints.GetHearingsByAnyCaseType();
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid hearing request")]
        public async Task GivenIHaveAValidHearingRequest()
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.NewHearingId = seededHearing.Id;
            _hearingId = seededHearing.Id;
        }

        [Given(@"set the booking status to (.*)")]
        [When(@"set the booking status to (.*)")]
        public void SetTheBookingStatus(UpdateBookingStatus bookingStatus)
        {
            UpdateTheHearingStatus(bookingStatus);
        }

        [Given(@"I have an empty status in a hearing status request")]
        public async Task GivenIHaveAnEmptyStatusHearingStatusRequest()
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.NewHearingId = seededHearing.Id;
            UpdateTheHearingStatus(null);
        }

        [Given(@"I have an empty username in a hearing status request")]
        public async Task GivenIHaveAnEmptyUsernameHearingStatusRequest()
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.NewHearingId = seededHearing.Id;
            _hearingId = seededHearing.Id;
            UpdateTheHearingStatus(UpdateBookingStatus.Cancelled, null);
        }

        [Given(@"I have a request to the second page of booked hearings")]
        public async Task GivenIHaveARequestToTheSecondPageOfBookedHearings()
        {
            await _apiTestContext.TestDataManager.SeedVideoHearing();
            await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.Uri = _endpoints.GetHearingsByAnyCaseType(1);
            _apiTestContext.HttpMethod = HttpMethod.Get;
            var response = await SendGetRequestAsync(_apiTestContext);
            var json = await response.Content.ReadAsStringAsync();
            var bookings = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<BookingsResponse>(json);

            _apiTestContext.Uri = _endpoints.GetHearingsByAnyCaseTypeAndCursor(bookings.NextCursor);
        }

        [Given(@"I have a request to the get booked hearings endpoint with a limit of one")]
        public async Task GivenIHaveARequestToTheGetBookedHearingsEndpointWithALimitOfOne()
        {
            await _apiTestContext.TestDataManager.SeedVideoHearing();
            await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.Uri = _endpoints.GetHearingsByAnyCaseType(1);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a request to the get booked hearings endpoint filtered on a (.*) case type")]
        [Given(@"I have a request to the get booked hearings endpoint filtered on an (.*) case type")]
        public async Task GivenIHaveARequestToTheGetBookedHearingsEndpointFilteredOnAValidCaseType(Scenario scenario)
        {
            await _apiTestContext.TestDataManager.SeedVideoHearing();

            int caseType;
            switch (scenario)
            {
                case Scenario.Valid:
                    caseType = 1;
                    break;
                case Scenario.Invalid:
                    caseType = 99;
                    break;
                default:
                    throw new InvalidOperationException("Unexpected type of case type: " + scenario);
            }

            _apiTestContext.Uri = _endpoints.GetHearingsByCaseType(caseType);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a (.*) hearing cancellation request")]
        public async Task GivenIHaveAHearingCancellationRequest(Scenario scenario)
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.NewHearingId = seededHearing.Id;
            switch (scenario)
            {
                case Scenario.Valid:
                    NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                    _hearingId = seededHearing.Id;
                    break;
                case Scenario.Invalid:
                    _hearingId = Guid.Empty;
                    break;
                case Scenario.Nonexistent:
                    _hearingId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            UpdateTheHearingStatus(UpdateBookingStatus.Cancelled);
        }

        [Then(@"hearing details should be retrieved")]
        public async Task ThenAHearingDetailsShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var response = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(json);
            response.Should().NotBeNull();
            AssertHearingDetailsResponse(response);
        }

        [Then(@"a list of hearing details should be retrieved")]
        public async Task ThenAListOfHearingDetailsShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var response = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingDetailsResponse>>(json);
            response.Should().NotBeNull();
            foreach (var hearingDetailsResponse in response)
            {
                AssertHearingDetailsResponse(hearingDetailsResponse);
            }
        }      

        [Then(@"hearing details should be updated")]
        public async Task ThenHearingDetailsShouldBeUpdated()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(json);

            model.ScheduledDuration.Should().Be(_apiTestContext.UpdateHearingRequest.ScheduledDuration);
            model.HearingVenueName.Should().Be(_apiTestContext.UpdateHearingRequest.HearingVenueName);
            model.ScheduledDateTime.Should().Be(_apiTestContext.UpdateHearingRequest.ScheduledDateTime.ToUniversalTime());
            model.HearingRoomName.Should().Be(_apiTestContext.UpdateHearingRequest.HearingRoomName);
            model.OtherInformation.Should().Be(_apiTestContext.UpdateHearingRequest.OtherInformation);

            var updatedCases = model.Cases;
            var caseRequest = _apiTestContext.UpdateHearingRequest.Cases.FirstOrDefault();
            updatedCases.First().Name.Should().Be(caseRequest.Name);
            updatedCases.First().Number.Should().Be(caseRequest.Number);
        }

        [Then(@"the hearing should be removed")]
        public void ThenTheHearingShouldBeRemoved()
        {
            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(_apiTestContext.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings.AsNoTracking().SingleOrDefault(x => x.Id == _hearingId);
            }
            hearingFromDb.Should().BeNull();
        }        

        [Then(@"the response should contain a list of booked hearings")]
        public async Task ThenTheResponseShouldContainAListOfBookedHearings()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<BookingsResponse>(json);
            model.Hearings.Count.Should().BeGreaterThan(0);

            var aHearing = model.Hearings.First().Hearings.First();
            aHearing.HearingNumber.Should().NotBeNullOrEmpty();
            aHearing.HearingName.Should().NotBeNullOrEmpty();
        }
       
        [Then(@"the response should contain a list of one booked hearing")]
        public async Task ThenTheResponseShouldContainAListOfOneBookedHearing()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<BookingsResponse>(json);
            model.Hearings.Count.Should().Be(1);
        }           
        
        [Then(@"hearing status should be (.*)")]
        public void ThenHearingDetailsShouldBeX(UpdateBookingStatus bookingStatus)
        {
            ThenHearingBookingStatusIs(GetMappedBookingStatus(bookingStatus));
        }
        
        [Then(@"hearing status should be unchanged")]
        public void ThenHearingDetailsShouldBeUnchanged()
        {
            ThenHearingBookingStatusIs(BookingStatus.Booked);
        }

        [Then(@"the response should be an empty list")]
        public async Task ThenTheResponseShouldBeAnEmptyList()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            json.Should().BeEquivalentTo("[]");
        }

        [Given(@"I have a hearing with suitability answers for a given hearing request with a (.*) hearing id")]
        [Given(@"I have a hearing with suitability answers a given hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAHearingWithSuitabilityAnswersForGivenHearingRequest(Scenario scenario)
        {
            await SetHearingIdForGivenScenario(scenario);
            _apiTestContext.Uri = _endpoints.GetSuitabilityAnswers(_hearingId);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Then(@"hearing suitability answers should be retrieved")]
        public async Task ThenHearingSuitabilityAnswersShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingSuitabilityAnswerResponse>>(json);
            model[0].Should().NotBeNull();
            model[0].ParticipantId.Should().NotBeEmpty();
            model[0].ScheduledAt.Should().BeAfter(DateTime.MinValue);
            model[0].UpdatedAt.Should().BeAfter(DateTime.MinValue);
            model[0].CreatedAt.Should().BeAfter(DateTime.MinValue);
            model[0].Answers.Should().NotBeNull();
            var firstAnswer = model[0].Answers.First();
            firstAnswer.Key.Should().NotBeEmpty();
            firstAnswer.Answer.Should().NotBeEmpty();
            firstAnswer.ExtendedAnswer.Should().NotBeEmpty();
        }


        private void ThenHearingBookingStatusIs(BookingStatus status)
        {
            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(_apiTestContext.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings.AsNoTracking().SingleOrDefault(x => x.Id == _apiTestContext.NewHearingId);
            }

            hearingFromDb.Should().NotBeNull();

            hearingFromDb.Status.Should().Be(status);
        }

        private BookingStatus GetMappedBookingStatus(UpdateBookingStatus status)
        {
            switch (status)
            {
                case Api.Contract.Requests.Enums.UpdateBookingStatus.Created:
                    return BookingStatus.Created;
                case Api.Contract.Requests.Enums.UpdateBookingStatus.Cancelled:
                    return BookingStatus.Cancelled;
                default:
                    break;
            }
            throw new ArgumentException("Invalid booking status type");
        }

        private void AssertHearingDetailsResponse(HearingDetailsResponse model)
        {
            _hearingId = model.Id;

            model.CaseTypeName.Should().NotBeNullOrEmpty();
            foreach (var theCase in model.Cases)
            {
                theCase.Name.Should().NotBeNullOrEmpty();
                theCase.Number.Should().NotBeNullOrEmpty();
            }

            model.HearingTypeName.Should().NotBeNullOrEmpty();
            model.HearingVenueName.Should().NotBeNullOrEmpty();
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
            }

            model.ScheduledDateTime.Should().BeAfter(DateTime.MinValue);
            model.ScheduledDuration.Should().BePositive();
            model.HearingRoomName.Should().NotBeNullOrEmpty();
            model.OtherInformation.Should().NotBeNullOrEmpty();
            model.CreatedBy.Should().NotBeNullOrEmpty();

            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(_apiTestContext.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings.AsNoTracking().SingleOrDefault(x => x.Id == _hearingId);
            }

            hearingFromDb.Should().NotBeNull();
        }

        [TearDown]
        public async Task TearDown()
        {
            await Hooks.ClearSeededHearings();
        }

        private static BookNewHearingRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(5).All()
                .With(x => x.ContactEmail = Faker.Internet.Email())
                .With(x => x.Username = Faker.Internet.Email())
                .Build().ToList();
            participants[0].CaseRoleName = "Claimant";
            participants[0].HearingRoleName = "Claimant LIP";

            participants[1].CaseRoleName = "Claimant";
            participants[1].HearingRoleName = "Solicitor";

            participants[2].CaseRoleName = "Defendant";
            participants[2].HearingRoleName = "Defendant LIP";

            participants[3].CaseRoleName = "Defendant";
            participants[3].HearingRoleName = "Solicitor";

            participants[4].CaseRoleName = "Judge";
            participants[4].HearingRoleName = "Judge";
            var cases = Builder<CaseRequest>.CreateListOfSize(2).Build().ToList();
            cases[0].IsLeadCase = true;

            var createdBy = "UserAdmin";

            return Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Civil Money Claims")
                .With(x => x.HearingTypeName = "Application to Set Judgment Aside")
                .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
                .With(x => x.Participants = participants)
                .With(x => x.Cases = cases)
                .With(x => x.CreatedBy = createdBy)
                .Build();
        }

        private static UpdateHearingRequest BuildUpdateHearingRequestRequest()
        {
            var caseList = new List<CaseRequest>();
            caseList.Add(new CaseRequest()
            {
                Name = "CaseName",
                Number = "CaseNumber"
            });
            return new UpdateHearingRequest
            {
                ScheduledDateTime = DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45),
                ScheduledDuration = 100,
                HearingVenueName = "Manchester Civil and Family Justice Centre",
                OtherInformation = "OtherInfo",
                HearingRoomName = "20",
                UpdatedBy = "admin@hearings.reform.hmcts.net",
                Cases = caseList
            };
        }

        private void CreateTheNewHearingRequest(BookNewHearingRequest request)
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _apiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _apiTestContext.Uri = _endpoints.BookNewHearing();
            _apiTestContext.HttpMethod = HttpMethod.Post;
        }

        private void UpdateTheHearingRequest()
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(_apiTestContext.UpdateHearingRequest);
            _apiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _apiTestContext.Uri = _endpoints.UpdateHearingDetails(_hearingId);
            _apiTestContext.HttpMethod = HttpMethod.Put;
        }

        private void UpdateTheHearingStatus(UpdateBookingStatus? status, string updatedBy = "testuser")
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(new UpdateBookingStatusRequest {
                Status = (UpdateBookingStatus)status,
                UpdatedBy = updatedBy
            });
            _apiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _apiTestContext.Uri = _endpoints.UpdateHearingDetails(_hearingId);
            _apiTestContext.HttpMethod = HttpMethod.Patch;
        }

        private async Task SetHearingIdForGivenScenario(Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        _hearingId = seededHearing.Id;
                        break;
                    }
                case Scenario.Nonexistent:
                    _hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    _hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
        }
    }
}
