using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Responses
{
    /// <summary>
    /// Describes if a participant must submit a checklist or not
    /// </summary>
    public class ChecklistRequiredResponse
    {
        public static readonly ChecklistRequiredResponse NotRequired = new ChecklistRequiredResponse(false);
        public static readonly ChecklistRequiredResponse Required = new ChecklistRequiredResponse(true);

        protected ChecklistRequiredResponse(bool isRequired) => IsRequired = isRequired;

        public ChecklistRequiredResponse() : this(false) { }

        /// <summary>
        /// If submitting a checklist is required for the participant
        /// </summary>
        public bool IsRequired { get; set; }
    }
}
