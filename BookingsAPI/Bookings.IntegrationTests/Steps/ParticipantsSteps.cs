﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.DAL;
using Bookings.Domain;
using Bookings.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;
using Testing.Common.Builders.Api.Request;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class ParticipantsSteps : StepsBase
    {
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;

        public ParticipantsSteps(Contexts.TestContext apiTestContext) : base(apiTestContext)
        {
        }

        [Given(@"I have a get participants in a hearing request with a (.*) hearing id")]
        [Given(@"I have a get participants in a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAGetParticipantsInAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            Guid hearingId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
                    ApiTestContext.NewHearingId = seededHearing.Id;
                    hearingId = ApiTestContext.NewHearingId;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                    break;
                }
                case Scenario.Nonexistent:
                    hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            ApiTestContext.Uri = _endpoints.GetAllParticipantsInHearing(hearingId);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have an add participant to a hearing request with a (.*) hearing id")]
        [Given(@"I have an add participant to a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAnAddParticipantToAHearingRequestWithAHearingId(Scenario scenario)
        {
            var request = BuildRequest();
            ApiTestContext.Participants = request.Participants;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            Guid hearingId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
                    ApiTestContext.NewHearingId = seededHearing.Id;
                    hearingId = ApiTestContext.NewHearingId;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                    break;
                }
                case Scenario.Nonexistent:
                    hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            ApiTestContext.Uri = _endpoints.AddParticipantsToHearing(hearingId);
            ApiTestContext.HttpMethod = HttpMethod.Post;
        }

        [When(@"I send the same request but with a new hearing id")]
        public async Task WhenISendTheSameRequestButWithANewHearingId()
        {
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            ApiTestContext.OldHearingId = ApiTestContext.NewHearingId;
            ApiTestContext.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            seededHearing.GetParticipants().Count.Should().Be(4);
            ApiTestContext.Uri = _endpoints.AddParticipantsToHearing(ApiTestContext.NewHearingId);
            ApiTestContext.ResponseMessage = await SendPostRequestAsync(ApiTestContext);
        }

        [Given(@"I have an add participants in a hearing request with an invalid participant")]
        public async Task GivenIHaveAnAddParticipantToAHearingRequestWithAParticipantId()
        {
            var request = new InvalidRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request.BuildRequest());
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            ApiTestContext.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            ApiTestContext.Uri = _endpoints.AddParticipantsToHearing(ApiTestContext.NewHearingId);
            ApiTestContext.HttpMethod = HttpMethod.Post;
        }

        [Given(@"I have a get a single participant in a hearing request with a (.*) hearing id")]
        [Given(@"I have a get a single participant in a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAGetASingleParticipantInAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            Guid hearingId;
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            ApiTestContext.NewHearingId = seededHearing.Id;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    hearingId = ApiTestContext.NewHearingId;
                    break;
                }
                case Scenario.Nonexistent:
                    hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            ApiTestContext.Uri = _endpoints.GetParticipantInHearing(hearingId, seededHearing.GetParticipants()[0].Id);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get a single participant in a hearing request with a (.*) participant id")]
        [Given(@"I have a get a single participant in a hearing request with an (.*) participant id")]
        public async Task GivenIHaveAGetASingleParticipantInAHearingRequestWithAParticipantId(Scenario scenario)
        {
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            ApiTestContext.NewHearingId = seededHearing.Id;
            Guid participantId;
            switch (scenario)
            {
                case Scenario.Nonexistent:
                    participantId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    participantId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            ApiTestContext.Uri = _endpoints.GetParticipantInHearing(ApiTestContext.NewHearingId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a remove participant from a hearing request with a (.*) hearing id")]
        [Given(@"I have a remove participant from a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveARemoveParticipantFromAHearingWithAValidHearingId(Scenario scenario)
        {
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            ApiTestContext.NewHearingId = seededHearing.Id;
            ApiTestContext.Participant = seededHearing.GetParticipants().First();
            ApiTestContext.HttpMethod = HttpMethod.Delete;

            var participantId = seededHearing.GetParticipants().First().Id;
            Guid hearingId;

            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        hearingId = ApiTestContext.NewHearingId;
                        break;
                    }
                case Scenario.Nonexistent:
                    {
                        hearingId = Guid.NewGuid();
                        break;
                    }
                case Scenario.Invalid:
                    {
                        hearingId = Guid.Empty;
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            ApiTestContext.Uri = _endpoints.RemoveParticipantFromHearing(hearingId, participantId);
        }

        [Given(@"I have a remove participant from a hearing request with a (.*) participant id")]
        [Given(@"I have a remove participant from a hearing request with an (.*) participant id")]
        public async Task GivenIHaveARemoveParticipantFromAHearingWithAParticipantId(Scenario scenario)
        {
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            ApiTestContext.NewHearingId = seededHearing.Id;
            ApiTestContext.HttpMethod = HttpMethod.Delete;

            Guid participantId;
            switch (scenario)
            {
                case Scenario.Nonexistent:
                {
                    participantId = Guid.NewGuid();
                    break;
                }
                case Scenario.Invalid:
                {
                    participantId = Guid.Empty;
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            ApiTestContext.Uri = _endpoints.RemoveParticipantFromHearing(ApiTestContext.NewHearingId, participantId);
        }

        [Then(@"a list of hearing participants should be retrieved")]
        public async Task ThenAListOfHearingParticipantsShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ParticipantResponse>>(json);
            CheckParticipantData(model);
        }

        [Given(@"I have an update suitability answers request with a (.*) hearing id and (.*) participant id")]
        [Given(@"I have an update suitability answers request with an (.*) hearing id and (.*) participant id")]
        public async Task GivenIHaveAnUpdateSuitabilityAnswersRequest(Scenario hearingScenario, Scenario participantScenario)
        {
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            ApiTestContext.NewHearingId = seededHearing.Id;
            Guid participantId;
            Guid hearingId;

            switch (hearingScenario)
            {
                case Scenario.Valid:
                    {
                        hearingId = ApiTestContext.NewHearingId;
                        break;
                    }
                case Scenario.Nonexistent:
                    {
                        hearingId = Guid.NewGuid();
                        break;
                    }
                case Scenario.Invalid:
                    {
                        hearingId = Guid.Empty;
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(hearingScenario), hearingScenario, null);
            }
            switch (participantScenario)
            {
                case Scenario.Valid:
                    {
                        participantId = seededHearing.GetParticipants().First().Id;
                        break;
                    }
                case Scenario.Nonexistent:
                    {
                        participantId = Guid.NewGuid();
                        break;
                    }
                case Scenario.Invalid:
                    {
                        participantId = Guid.Empty;
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(participantScenario), participantScenario, null);
            }

            ApiTestContext.Uri = _endpoints.UpdateSuitabilityAnswers(hearingId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Put;
            var request = UpdateSuitabilityAnswersRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        private static void CheckParticipantData(IReadOnlyCollection<ParticipantResponse> model)
        {
            model.Should().NotBeNull();
            foreach (var participant in model)
            {
                participant.ContactEmail.Should().NotBeNullOrEmpty();
                participant.FirstName.Should().NotBeNullOrEmpty();
                participant.Id.Should().NotBeEmpty();
                participant.LastName.Should().NotBeNullOrEmpty();
                participant.MiddleNames.Should().NotBeNullOrEmpty();
                participant.TelephoneNumber.Should().NotBeNullOrEmpty();
                participant.Title.Should().NotBeNullOrEmpty();
                participant.UserRoleName.Should().NotBeNullOrEmpty();
                participant.Username.Should().NotBeNullOrEmpty();
                participant.CaseRoleName.Should().NotBeNullOrEmpty();
                participant.HearingRoleName.Should().NotBeNullOrEmpty();
            }
        }

        [Then(@"a hearing participant should be retrieved")]
        public async Task ThenAHearingParticipantShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = new List<ParticipantResponse> { ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ParticipantResponse>(json) };
            CheckParticipantData(model);
        }

        [Then(@"the participant should be (.*)")]
        public void ThenTheParticipantShouldBeAddedOrRemoved(string state)
        {
            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(ApiTestContext.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings
                    .Include("Participants.Person").AsNoTracking()
                    .Single(x => x.Id == ApiTestContext.NewHearingId);
            }
            if (state.Equals("added"))
            {
                hearingFromDb.GetParticipants().Count.Should().BeGreaterThan(3);
                foreach (var participantRequest in ApiTestContext.Participants)
                {
                    hearingFromDb.GetParticipants().Any(x => x.Person.Username == participantRequest.Username).Should()
                        .BeTrue();
                }
            }
            if (state.Equals("removed"))
            {               
                hearingFromDb.GetParticipants().Any(x => x.Id == ApiTestContext.Participant.Id).Should().BeFalse();
            }
        }

        [Given(@"I have an update participant in a hearing request with a (.*) hearing id")]
        [Given(@"I have an update participant in a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAnUpdateParticipantInAHearingRequestWithANonexistentHearingId(Scenario scenario)
        {
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            var participantId = seededHearing.GetParticipants().FirstOrDefault().Id;
            var updateParticipantRequest = new UpdateParticipantRequestBuilder().Build();
            Guid hearingId;

            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(updateParticipantRequest);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        ApiTestContext.NewHearingId = seededHearing.Id;
                        hearingId = ApiTestContext.NewHearingId;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        break;
                    }
                case Scenario.Nonexistent:
                    hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            ApiTestContext.Uri = _endpoints.UpdateParticipantDetails(hearingId,participantId);
            ApiTestContext.HttpMethod = HttpMethod.Put;
        }

        [Given(@"I have an update participant in a hearing request with a invalid solicitors reference")]
        public async Task GivenIHaveAnUpdateParticipantInAHearingRequestWithAInvalidSolicitorsReference()
        {
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            var participantId = seededHearing.GetParticipants().FirstOrDefault(x=>x.HearingRole.UserRole.IsRepresentative).Id;
            var updateParticipantRequest = new UpdateParticipantRequestBuilder().Build();
            var hearingId = seededHearing.Id;
            updateParticipantRequest.SolicitorsReference = string.Empty;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(updateParticipantRequest);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            ApiTestContext.Uri = _endpoints.UpdateParticipantDetails(hearingId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Put;
        }


        [Given(@"I have an update participant in a hearing request with a invalid address")]
        public async Task GivenIHaveAnUpdateParticipantInAHearingRequestWithAInvalidAddress()
        {
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            var participantId = seededHearing.GetParticipants().FirstOrDefault(x => x.HearingRole.UserRole.IsIndividual).Id;
            var updateParticipantRequest = new UpdateParticipantRequestBuilder().Build();
            var hearingId = seededHearing.Id;
            updateParticipantRequest.Street = string.Empty;
            updateParticipantRequest.HouseNumber = string.Empty;
            updateParticipantRequest.Postcode = string.Empty;
            updateParticipantRequest.City = string.Empty;
            updateParticipantRequest.County = string.Empty;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(updateParticipantRequest);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            ApiTestContext.Uri = _endpoints.UpdateParticipantDetails(hearingId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Put;
        }


        private static AddParticipantsToHearingRequest BuildRequest()
        {
            var newParticipant = new ParticipantRequestBuilder("Defendant", "Defendant LIP").Build();

            return new AddParticipantsToHearingRequest
            {
                Participants = new List<ParticipantRequest> { newParticipant }
            };
        }



        private List<SuitabilityAnswersRequest> UpdateSuitabilityAnswersRequest()
        {
            var answers = new List<SuitabilityAnswersRequest>();
            answers.Add(new SuitabilityAnswersRequest {
                Key = "_Key",
                Answer ="_Answer",
                ExtendedAnswer = "_ExtendedAnswer"
            });
            return answers;
        }





        [TearDown]
        public async Task TearDown()
        {
            if (ApiTestContext.NewHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {ApiTestContext.NewHearingId}");
                await TestDataManager.RemoveVideoHearing(ApiTestContext.NewHearingId);
            }
            if (ApiTestContext.OldHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {ApiTestContext.OldHearingId}");
                await TestDataManager.RemoveVideoHearing(ApiTestContext.OldHearingId);
            }
        }
    }
}
