using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Responses
{
    public class QuestionAnswerResponse
    {
        public string QuestionKey { get; set; }
        public string Answer { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
