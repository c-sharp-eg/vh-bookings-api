using Bookings.Api.Contract.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.API.Validations
{
    public class QuestionRequestValidation : AbstractValidator<QuestionAnswerRequest>
    {
        public static string MissingQuestionKeyErrorMessage => "Require the question key";
        public static string MissingAnswerErrorMessage => "Require the answer to the question";

        public QuestionRequestValidation()
        {
            RuleFor(x => x.QuestionKey).NotEmpty().WithMessage(MissingQuestionKeyErrorMessage);
            RuleFor(x => x.Answer).NotEmpty().WithMessage(MissingAnswerErrorMessage);
        }
    }
}
