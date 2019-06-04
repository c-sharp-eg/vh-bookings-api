using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.IntegrationTests.Helper;
using Faker;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public class PersonSteps : StepsBase
    {
        private readonly PersonEndpoints _endpoints = new ApiUriFactory().PersonEndpoints;
        private string _username;

        public PersonSteps(Contexts.TestContext apiTestContext) : base(apiTestContext)
        {
            _username = string.Empty;
        }

        [Given(@"I have a get person by username request with a (.*) username")]
        [Given(@"I have a get person by username request with an (.*) username")]
        public async Task GivenIHaveAGetPersonByUsernameRequest(Scenario scenario)
        {
            await SetUserNameForGivenScenario(scenario);
            ApiTestContext.Uri = _endpoints.GetPersonByUsername(_username);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get (.*) by contact email request with a (.*) contact email")]
        [Given(@"I have a get (.*) by contact email request with an (.*) contact email")]
        public async Task GivenIHaveAGetPersonByContactEmailRequest(string personType, Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
                        ApiTestContext.NewHearingId = seededHearing.Id;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        if (personType.Equals("individual"))
                        {
                            _username = seededHearing.GetParticipants().First(p => p.HearingRole.UserRole.IsIndividual).Person.ContactEmail;
                        }
                        else
                        {
                            _username = seededHearing.GetParticipants().First().Person.ContactEmail;
                        }
                        break;
                    }
                case Scenario.Nonexistent:
                    _username = Internet.Email();
                    break;
                case Scenario.Invalid:
                    _username = "invalid contact email";
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            ApiTestContext.Uri = _endpoints.GetPersonByContactEmail(_username);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }


        [Given(@"I have a get person by contact email search term request")]
        public async Task GivenIHaveAGetPersonByContactEmailSearchTermRequest()
        {
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            ApiTestContext.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var email = seededHearing.GetParticipants().First().Person.ContactEmail;
            var searchTerm = email.Substring(0, 3);
            ApiTestContext.Uri = _endpoints.GetPersonBySearchTerm(searchTerm);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get person by contact email search term request that case insensitive")]
        public async Task GivenIHaveAGetPersonByContactEmailSearchTermRequestThatCaseInsensitive()
        {
            var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing();
            ApiTestContext.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var email = seededHearing.GetParticipants().First().Person.ContactEmail;
            var searchTerm = email.Substring(0, 3).ToUpperInvariant();
            ApiTestContext.Uri = _endpoints.GetPersonBySearchTerm(searchTerm);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get person suitability answers by username request with an (.*) username")]
        public async Task GivenIHaveAGetPersonSuitabilityAnswersByUsernameRequest(Scenario scenario)
        {
            await SetUserNameForGivenScenario(scenario, true, true);
            ApiTestContext.Uri = _endpoints.GetPersonSuitabilityAnswers(_username);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get person without suitability answers by username request with an (.*) username")]
        public async Task GivenIHaveAGetPersonWithoutSuitabilityAnswersByUsernameRequest(Scenario scenario)
        {
            await SetUserNameForGivenScenario(scenario, false);
            ApiTestContext.Uri = _endpoints.GetPersonSuitabilityAnswers(_username);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Then(@"person details should be retrieved")]
        public async Task ThenThePersonDetailsShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<PersonResponse>(json);
            model.Should().NotBeNull();
            ValidatePersonData(model);
        }

        private static void ValidatePersonData(PersonResponse model)
        {
            model.Id.Should().NotBeEmpty();
            model.ContactEmail.Should().NotBeNullOrEmpty();
            model.FirstName.Should().NotBeNullOrEmpty();
            model.LastName.Should().NotBeNullOrEmpty();
            model.MiddleNames.Should().NotBeNullOrEmpty();
            model.TelephoneNumber.Should().NotBeNullOrEmpty();
            model.Title.Should().NotBeNullOrEmpty();
            model.Username.Should().NotBeNullOrEmpty();
        }

        [Then(@"person address should be retrieved")]
        public async Task ThenPersonAddressShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<PersonResponse>(json);
            model.Should().NotBeNull();
            model.Organisation.Should().NotBeNullOrEmpty();
            model.Id.Should().NotBeEmpty();
            model.HouseNumber.Should().NotBeNullOrEmpty();
            model.Street.Should().NotBeNullOrEmpty();
            model.City.Should().NotBeNullOrEmpty();
            model.County.Should().NotBeNullOrEmpty();
            model.Postcode.Should().NotBeNullOrEmpty();
        }


        [Then(@"persons details should be retrieved")]
        public async Task ThenPersonsDetailsShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<PersonResponse>>(json);
            model[0].Should().NotBeNull();
            ValidatePersonData(model[0]);
        }

        [Then(@"'(.*)' suitability answers should be retrieved")]
        public async Task ThenPersonsSuitabilityAnswersShouldBeRetrieved(string scenario)
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<PersonSuitabilityAnswerResponse>>(json);

            model[0].Should().NotBeNull();
            model[0].HearingId.Should().NotBeEmpty();
            model[0].HearingId.Should().Be(ApiTestContext.NewHearingId);
            model[0].ParticipantId.Should().NotBeEmpty();
            model[0].ParticipantId.Should().Be(ApiTestContext.Participant.Id);
            model[0].ScheduledAt.Should().BeAfter(DateTime.MinValue);
            if(scenario == "with")
            {
                model[0].Answers.Should().NotBeNull();
                model[0].Answers.Count.Should().Be(2);
                model[0].UpdatedAt.Should().BeAfter(DateTime.MinValue);
            }
            else
            {
                model[0].Answers.Count.Should().Be(0);
                model[0].UpdatedAt.Should().Be(DateTime.MinValue);
            }
            
        }

        private async Task SetUserNameForGivenScenario(Scenario scenario, bool hasSuitability = false, bool addSuitabilityAnswer = false)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        var seededHearing = await ApiTestContext.TestDataManager.SeedVideoHearing(addSuitabilityAnswer);
                        ApiTestContext.NewHearingId = seededHearing.Id;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        var participants = seededHearing.GetParticipants();
                        if(hasSuitability)
                        {
                            _username = participants.First(p => p.SuitabilityAnswers.Any()).Person.Username;
                        }
                        else
                        {
                            _username = participants.First().Person.Username;
                        }
                        ApiTestContext.Participant = participants.First(p => p.Person.Username.Equals(_username));
                        break;
                    }
                case Scenario.Nonexistent:
                    _username = Internet.Email();
                    break;
                case Scenario.Invalid:
                    _username = "invalid username";
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

        }
    }
}