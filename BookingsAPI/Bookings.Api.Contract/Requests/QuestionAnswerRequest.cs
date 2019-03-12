using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Requests
{
    public class QuestionAnswerRequest
    {
        public string QuestionKey { get; set; }
        public string Answer { get; set; }
        public string Notes { get; set; }
    }
}
