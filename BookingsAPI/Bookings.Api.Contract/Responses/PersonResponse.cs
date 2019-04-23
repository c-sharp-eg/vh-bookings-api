using System;

namespace Bookings.Api.Contract.Responses
{
    public class PersonResponse
    {
        /// <summary>
        ///     Participant Id
        /// </summary>
        public Guid Id { get; set; } 
        
        /// <summary>
        ///     Participant Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Participant first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Participant middle name.
        /// </summary>
        public string MiddleNames { get; set; }

        /// <summary>
        ///     Participant last name.
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        ///     Participant contact email
        /// </summary>
        public string ContactEmail { get; set; }
        
        /// <summary>
        ///     Participant telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        ///     Participant username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Organisation of representative
        /// </summary>
        public string Organisation { get; set; }

        /// <summary>
        ///     House number of an Individual
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        ///     Street number of an Individual
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        ///     Postcode of an Individual
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        ///     City/Town of an Individual
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///     County of an Individual
        /// </summary>
        public string County { get; set; }
    }
}