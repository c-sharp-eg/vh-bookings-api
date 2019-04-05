﻿using System;
using System.Linq;
using Bookings.Domain.Ddd;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;

namespace Bookings.Domain.Participants
{
    public abstract class Participant: Entity<Guid>
    {
        private readonly ValidationFailures _validationFailures = new ValidationFailures();

        protected Participant()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }

        protected Participant(Person person, HearingRole hearingRole, CaseRole caseRole) : this()
        {
            Person = person;
            PersonId = person.Id;
            HearingRoleId = hearingRole.Id;
            CaseRoleId = caseRole.Id;
        }

        public string DisplayName { get; set; }
        public int CaseRoleId { get; set; }
        public virtual CaseRole CaseRole { get; set; }
        public int HearingRoleId { get; set; }
        public virtual HearingRole HearingRole { get; set; }
        public Guid PersonId { get; protected set; }
        public virtual Person Person { get; protected set; }
        public Guid HearingId { get; protected set; }
        protected virtual Hearing Hearing { get; set; }   
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public void UpdateParticipantDetails(string title, string displayName, string telephoneNumber, string street, string houseNumber, string city, string county, string postcode, string organisationName)
        {
            ValidateArguments(displayName);

            if (HearingRole.UserRole.IsIndividual)
            {
                ValidateAddressDetails(houseNumber, street, city, county, postcode); 
            }

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }


            DisplayName = displayName;
            Person.Title = title;
            Person.TelephoneNumber = telephoneNumber;
            UpdatedDate = DateTime.UtcNow;
            if (HearingRole.UserRole.Name.Equals("Individual"))
            {
                var address = new Address(houseNumber,street,postcode,city,county);
                Person.UpdateAddress(address);
            }
            
            if (!string.IsNullOrEmpty(organisationName))
            {
                var organisation = new Organisation(organisationName);
                Person.UpdateOrganisation(organisation);
            }

        }

        private void ValidateArguments(string displayName)
        {
           if (string.IsNullOrEmpty(displayName))
            {
                _validationFailures.AddFailure("DisplayName", "DisplayName is required");
            }
        }

        private void ValidateAddressDetails(string houseNumber, string street, string city, string county, string postcode)
        {
            if (string.IsNullOrEmpty(houseNumber))
            {
                _validationFailures.AddFailure("HouseNumber", "HouseNumber is required");
            }
            if (string.IsNullOrEmpty(street))
            {
                _validationFailures.AddFailure("Street", "Street is required");
            }
            if (string.IsNullOrEmpty(city))
            {
                _validationFailures.AddFailure("City", "City is required");
            }
            if (string.IsNullOrEmpty(county))
            {
                _validationFailures.AddFailure("County", "County is required");
            }
            if (string.IsNullOrEmpty(postcode))
            {
                _validationFailures.AddFailure("Postcode", "Postcode is required");
            }
        }
    }
}