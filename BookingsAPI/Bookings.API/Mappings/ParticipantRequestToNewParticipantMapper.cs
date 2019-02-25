using System.Linq;
using Bookings.Api.Contract.Requests;
using Bookings.DAL.Commands;
using Bookings.Domain;
using Bookings.Domain.RefData;

namespace Bookings.API.Mappings
{
    public class ParticipantRequestToNewParticipantMapper
    {
        public NewParticipant MapRequestToParticipant(ParticipantRequest requestParticipant, CaseType caseType)
        {
            var caseRole = caseType.CaseRoles.SingleOrDefault(x => x.Name == requestParticipant.CaseRoleName);
            var hearingRole = caseRole?.HearingRoles.SingleOrDefault(x => x.Name == requestParticipant.HearingRoleName);

            var person = new Person(requestParticipant.Title, requestParticipant.FirstName, requestParticipant.LastName,
                requestParticipant.Username)
            {
                MiddleNames = requestParticipant.MiddleNames,
                ContactEmail = requestParticipant.ContactEmail,
                TelephoneNumber = requestParticipant.TelephoneNumber
            };

            return new NewParticipant
            {
                Person = person,
                CaseRole = caseRole,
                HearingRole = hearingRole,
                DisplayName = requestParticipant.DisplayName,
                Representee = requestParticipant.Representee,
                SolicitorsReference = requestParticipant.SolicitorsReference,
            };
        }
    }
}