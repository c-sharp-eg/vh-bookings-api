using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.Validations;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public interface IHearingService
    {
        /// <summary>
        /// Add a participant to a hearing service. This will re-use existing personnel entries before attempting to
        /// create a new one.
        /// </summary>
        /// <param name="hearing">Hearing to amend</param>
        /// <param name="participants">List of participants to add</param>
        /// <returns></returns>
        Task AddParticipantToService(VideoHearing hearing, List<NewParticipant> participants);
    }
    public class HearingService : IHearingService
    {
        private readonly BookingsDbContext _context;

        public HearingService(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task AddParticipantToService(VideoHearing hearing, List<NewParticipant> participants)
        {
            foreach (var participantToAdd in participants)
            {
                var existingPerson = await _context.Persons
                    .Include("Address")
                    .Include("Organisation")
                    .SingleOrDefaultAsync(x => x.Username == participantToAdd.Person.Username);
                
                switch (participantToAdd.HearingRole.UserRole.Name)
                {
                    case "Individual":
                        var individual = hearing.AddIndividual(existingPerson ?? participantToAdd.Person, participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName);

                        UpdateAddressAndOrganisationDetails(participantToAdd.Person, individual);
                        break;
                    case "Representative":
                    {
                            var representative = hearing.AddRepresentative(existingPerson ?? participantToAdd.Person, participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName,
                            participantToAdd.Reference, participantToAdd.Representee);
                            UpdateAddressAndOrganisationDetails(participantToAdd.Person, representative);
                        break;
                    }
                    case "Judge":
                       hearing.AddJudge(existingPerson ?? participantToAdd.Person, participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName);
                       break;
                    default:
                        throw new DomainRuleException(nameof(participantToAdd.HearingRole.UserRole.Name),
                            $"Role {participantToAdd.HearingRole.UserRole.Name} not recognised");
                }
            }
        }
        private void UpdateAddressAndOrganisationDetails(Person newPersonDetails, Participant participantToUpdate)
        {
            var newAddress = newPersonDetails.Address;
            var newOrganisation = newPersonDetails.Organisation;
            var existingPerson = participantToUpdate.Person;
            participantToUpdate.UpdateParticipantDetails(existingPerson.Title, participantToUpdate.DisplayName, existingPerson.TelephoneNumber,
                   newAddress?.Street, newAddress?.HouseNumber, newAddress?.City, newAddress?.County, newAddress?.Postcode, newOrganisation?.Name);
        }
    }
}