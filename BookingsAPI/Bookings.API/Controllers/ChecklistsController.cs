using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.API.Extensions;
using Bookings.API.Mappings;
using Bookings.API.Utilities;
using Bookings.API.Validations;
using Bookings.DAL.Commands;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Bookings.API.Mappings;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class ChecklistsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private const string ChecklistsEndpointBaseUrl = "hearings/";
        private const string AllChecklistsEndpointUrl = "participants/checklists";

        public ChecklistsController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Gets list of all submitted participant checklists including participant and hearing details.
        /// Ordered by checklist submission date, most recent checklist first.
        /// </summary>
        /// <param name="pageSize">Maximum number of items to retrieve in the page, maximum allowed 1000.</param>
        /// <param name="page">One-based index of page to retrieve.</param>
        /// <returns>The list of the participants questionnaire answers.</returns>
        [HttpGet(AllChecklistsEndpointUrl)]
        [SwaggerOperation(OperationId = "GetAllParticipantsChecklists")]
        [ProducesResponseType(typeof(ChecklistsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllParticipantsChecklists(int pageSize = 5, int page = 1)
        {
            var validationResult = PaginationValidator
             .WithMaxPageSize(1000)
             .Validate(new PaginatedRequest(page, pageSize));

            validationResult.AddTo(ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var query = new GetAllParticipantsChecklistsQuery();

            var participants = await _queryHandler.Handle<GetAllParticipantsChecklistsQuery, List<Participant>>(query);

            if (participants == null)
            {
                return NotFound();
            }

            var response = new PaginationBuilder<ChecklistsResponse, Participant>(MapChecklistsResponse)
               .ResourceUrl(ChecklistsEndpointBaseUrl + AllChecklistsEndpointUrl)
               .WithSourceItems(participants.AsQueryable())
               .PageSize(pageSize)
               .CurrentPage(page)
               .Build();


            var hearings = participants.GroupBy(p => p.HearingId)
                .Select(x => x.First().HearingId);

            response.Hearings = hearings.Select(x => new ChecklistHearingToResponseMapper().MapQuestionAnswerToResponse(x)).ToList();

            return Ok(response);

        }

        private ChecklistsResponse MapChecklistsResponse(List<Participant> participants)
        {
            return new ChecklistsResponse
            {
                Checklists = participants.Select(MapParticipantChecklist).ToList()
            };
        }

        private HearingParticipantCheckListResponse MapParticipantChecklist(Participant hearingParticipant)
        {
            var checklistAnswers = hearingParticipant.GetChecklistAnswers().OrderByDescending(c => c.CreatedAt).ToList();
            var participant = hearingParticipant.Person;
            var answers = new QuestionAnswerToResponseMapper().MapQuestionAnswerToResponse(checklistAnswers);

            return new HearingParticipantCheckListResponse
            {
                Title = participant.Title,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                Landline = participant.TelephoneNumber,
                Role = hearingParticipant.HearingRole.UserRole.Name.ToString(),
                //Mobile = participant.Mobile,
                HearingId = hearingParticipant.HearingId,
                ParticipantId = participant.Id,
                CompletedDate = checklistAnswers.Max(answer => answer.CreatedAt),
                QuestionAnswerResponses = answers
            };
        }
    }
}