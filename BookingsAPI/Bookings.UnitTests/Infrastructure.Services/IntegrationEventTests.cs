﻿using System;
using System.Linq;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using Bookings.Infrastructure.Services.ServiceBusQueue;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Infrastructure.Services
{
    public class IntegrationEventTests
    {
        private ServiceBusQueueClientFake _serviceBusQueueClient;
        private IRaiseIntegrationEvent _raiseIntegrationEvent;

        [SetUp]
        public void Setup()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            _raiseIntegrationEvent = new RaiseIntegrationEvent(_serviceBusQueueClient);
        }

        [Test]
        public void should_publish_message_to_queue_when_HearingCancelledIntegrationEvent_is_raised()
        {
            var hearingCancelledEvent = new HearingCancelledIntegrationEvent(Guid.NewGuid());
            _raiseIntegrationEvent.Raise(hearingCancelledEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.Should().BeOfType<HearingCancelledIntegrationEvent>();
        }

        [Test]
        public void should_publish_message_to_queue_when_HearingDetailsUpdatedIntegrationEvent_is_raised()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.CaseType = new CaseType(1, "test");
            hearing.AddCase("1234", "test", true);

            var hearingDetailsUpdatedIntegrationEvent = new HearingDetailsUpdatedIntegrationEvent(hearing);
            _raiseIntegrationEvent.Raise(hearingDetailsUpdatedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.Should().BeOfType<HearingDetailsUpdatedIntegrationEvent>();
        }

        [Test]
        public void should_publish_message_to_queue_when_ParticipantAddedIntegrationEvent_is_raised()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.CaseType = new CaseType(1, "test");
            hearing.AddCase("1234", "test", true);
            var individuals = hearing.GetParticipants().Where(x => x is Individual).ToList();

            var individual1 = individuals.First();
            individual1.HearingRole = new HearingRole(1, "Claimant LIP") { UserRole = new UserRole(1, "Individual") };
            individual1.CaseRole = new CaseRole(1, "test");

            var participantAddedIntegrationEvent = new ParticipantAddedIntegrationEvent(hearing.Id, individual1);
            _raiseIntegrationEvent.Raise(participantAddedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.Should().BeOfType<ParticipantAddedIntegrationEvent>();
        }

        [Test]
        public void should_publish_message_to_queue_when_ParticipantRemovedIntegrationEvent_is_raised()
        {
            var participantRemovedIntegrationEvent = new ParticipantRemovedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid());
            _raiseIntegrationEvent.Raise(participantRemovedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.Should().BeOfType<ParticipantRemovedIntegrationEvent>();
        }

        [Test]
        public void should_publish_message_to_queue_when_HearingIsReadyForVideoIntegrationEvent_is_raised()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.CaseType = new CaseType(1, "test");
            hearing.AddCase("1234", "test", true);
            var individuals = hearing.GetParticipants().Where(x => x is Individual).ToList();

            var individual1 = individuals.First();
            individual1.HearingRole = new HearingRole(1, "Claimant LIP") { UserRole = new UserRole(1, "Individual") };
            individual1.CaseRole = new CaseRole(1, "test");

            var individual2 = individuals.Last();
            individual2.HearingRole = new HearingRole(2, "Defendant LIP") { UserRole = new UserRole(1, "Individual") };
            individual2.CaseRole = new CaseRole(2, "test2");

            var representatvie = hearing.GetParticipants().Single(x => x is Representative);
            representatvie.HearingRole = new HearingRole(5, "Solicitor"){ UserRole = new UserRole(2, "Solitcitor") } ;
            representatvie.CaseRole= new CaseRole(3, "test3");

            var hearingIsReadyForVideoIntegrationEvent = new HearingIsReadyForVideoIntegrationEvent(hearing);
            _raiseIntegrationEvent.Raise(hearingIsReadyForVideoIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.Should().BeOfType<HearingIsReadyForVideoIntegrationEvent>();
        }
    }
}