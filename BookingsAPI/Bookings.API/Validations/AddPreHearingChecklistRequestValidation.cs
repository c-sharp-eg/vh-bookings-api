using Bookings.Api.Contract.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.API.Validations
{
    public class AddPreHearingChecklistRequestValidation : AbstractValidator<AddPreHearingChecklistRequest>
    {
        public static string MissingQuestionListErrorMessage => "Require list of questions with answers";

        public AddPreHearingChecklistRequestValidation()
        {
            RuleForEach(x => x.QuestionAnswers).NotEmpty().WithMessage(MissingQuestionListErrorMessage).SetValidator(new QuestionRequestValidation());
        }
    }
}
