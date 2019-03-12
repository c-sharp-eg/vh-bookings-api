using Bookings.Domain.Ddd;
using System;
using System.Collections.Generic;
using System.Text;
using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;


namespace Bookings.Domain
{
    public class ChecklistQuestion : Entity<int>
    {

       /// <summary>
       /// User Role
       /// </summary>
        public virtual UserRole UserRole { get; set; }

        /// <summary>
        /// A unique identifying string for this question belonging to the given role
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Descrition of the Question
        /// </summary>
        public string Description { get; set; }
    }
}
