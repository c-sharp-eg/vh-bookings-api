using System;
using System.Linq;
using Bookings.Domain;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;
using FizzWare.NBuilder;
using Bookings.Domain.Participants;

namespace Bookings.UnitTests.Domain.Checklist
{
   public  class AddChecklitToParticipantTest
    {
        [Test]
        public void should_throw_domain_exception_if_answering_duplicate_questions()
        {
            var hearing = new VideoHearingBuilder().Build();

            var repParticpant = hearing.GetParticipants().FirstOrDefault(x=>x.HearingRole.UserRole.Name=="Representative");
            var bandwidthQuestion = new ChecklistQuestion() { Key = "EQUIPMENT_BANDWIDTH",UserRole=new Bookings.Domain.RefData.UserRole(1,"Individual") };
            //var Checklist.New(parepParticpantrticipant, bandwidthQuestion);
            //var checklist = Checklist(repParticpant, new[] { bandwidthQuestion });
            //checklist.Answer(bandwidthQuestion.Key, "12mbit/s", null);

            //Assert.Catch<DomainRuleException>(() => checklist.Answer(bandwidthQuestion.Key, "13mbit/s", null));
        }

        [Test]
        public void should_throw_exception_when_validation_fails()
        {
            // ReSharper disable once ObjectCreationAsStatement
            //Action action = () => new Checklist(null, null, null, null);

            //action.Should().Throw<DomainRuleException>()
            //    .And.ValidationFailures.Should()
            //    .Contain(x => x.Name == "Title")
            //    .And.Contain(x => x.Name == "FirstName")
            //    .And.Contain(x => x.Name == "LastName")
            //    .And.Contain(x => x.Name == "Username");
        }
    }
}
