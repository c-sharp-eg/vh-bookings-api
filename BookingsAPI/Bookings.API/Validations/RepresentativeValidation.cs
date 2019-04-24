﻿using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class RepresentativeValidation : AbstractValidator<IRepresentativeInfoRequest>
    {
        public static readonly string NoSolicitorReference = "Solicitor Reference is required";
        public static readonly string NoRepresentee = "Representee is required";
        public static readonly string NoOrganisation = "Organisation is required";

        public RepresentativeValidation()
        {
            RuleFor(x => x.SolicitorsReference).NotEmpty().WithMessage(NoSolicitorReference);
            RuleFor(x => x.Representee).NotEmpty().WithMessage(NoRepresentee);
            RuleFor(x => x.OrganisationName).NotEmpty().WithMessage(NoOrganisation);
        }
    }
}
