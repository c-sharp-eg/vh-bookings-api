using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.API.Extensions;
using Bookings.API.Helpers;
using Bookings.API.Mappings;
using Bookings.API.Validations;
using Bookings.DAL.Commands;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class HearingParticipantsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;

        public HearingParticipantsController(IQueryHandler queryHandler,
            ICommandHandler commandHandler,
            IEventPublisher eventPublisher)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Get participants in a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param>
        /// <returns>List of participants</returns>
        [HttpGet("{hearingId}/participants", Name = "GetAllParticipantsInHearing")]
        [SwaggerOperation(OperationId = "GetAllParticipantsInHearing")]
        [ProducesResponseType(typeof(List<ParticipantResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllParticipantsInHearing(Guid hearingId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var query = new GetParticipantsInHearingQuery(hearingId);
            try
            {
                var participants = await _queryHandler.Handle<GetParticipantsInHearingQuery, List<Participant>>(query);

                var mapper = new ParticipantToResponseMapper();
                var response = participants.Select(x => mapper.MapParticipantToResponse(x)).ToList();

                return Ok(response);
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }

        }

        /// <summary>
        /// Get a single participant in a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param>
        /// <param name="participantId">The Id of the participant</param>
        /// <returns>The participant</returns>
        [HttpGet("{hearingId}/participants/{participantId}", Name = "GetParticipantInHearing")]
        [SwaggerOperation(OperationId = "GetParticipantInHearing")]
        [ProducesResponseType(typeof(ParticipantResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetParticipantInHearing(Guid hearingId, Guid participantId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
                return BadRequest(ModelState);
            }

            var query = new GetParticipantsInHearingQuery(hearingId);
            try
            {
                var participants = await _queryHandler.Handle<GetParticipantsInHearingQuery, List<Participant>>(query);
                var participant = participants.FirstOrDefault(x => x.Id == participantId);

                if (participant == null)
                {
                    return NotFound();
                }

                var mapper = new ParticipantToResponseMapper();
                var response = mapper.MapParticipantToResponse(participant);

                return Ok(response);
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Add participant(s) to a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param> 
        /// <param name="request">The participant information to add</param>
        /// <returns>The participant</returns>
        [HttpPost("{hearingId}/participants", Name = "AddParticipantsToHearing")]
        [SwaggerOperation(OperationId = "AddParticipantsToHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddParticipantsToHearing(Guid hearingId,
            [FromBody] AddParticipantsToHearingRequest request)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new AddParticipantsToHearingRequestValidation().Validate(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            var query = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var caseTypequery = new GetCaseTypeQuery(videoHearing.CaseType.Name);
            var caseType = await _queryHandler.Handle<GetCaseTypeQuery, CaseType>(caseTypequery);

            var individualRoles = caseType.CaseRoles.SelectMany(x => x.HearingRoles).Where(x => x.UserRole.IsIndividual).Select(x => x.Name).ToList();

            var addressValidationResult = AddressValidationHelper.ValidateAddress(individualRoles, request.Participants);

            if (!addressValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(addressValidationResult.Errors);
                return BadRequest(ModelState);
            }

            var representativeRoles = caseType.CaseRoles.SelectMany(x => x.HearingRoles).Where(x => x.UserRole.IsRepresentative).Select(x => x.Name).ToList();
            var reprensentatives = request.Participants.Where(x => representativeRoles.Contains(x.HearingRoleName)).ToList();

            var representativeValidationResult = RepresentativeValidationHelper.ValidateRepresentativeInfo(reprensentatives);

            if (!representativeValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(representativeValidationResult.Errors);
                return BadRequest(ModelState);
            }


            var mapper = new ParticipantRequestToNewParticipantMapper();
            var participants = request.Participants
                .Select(x => mapper.MapRequestToNewParticipant(x, videoHearing.CaseType)).ToList();
            var command = new AddParticipantsToVideoHearingCommand(hearingId, participants);

            try
            {
                await _commandHandler.Handle(command);
            }
            catch (DomainRuleException e)
            {
                ModelState.AddDomainRuleErrors(e.ValidationFailures);
                return BadRequest(ModelState);
            }

            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

            // ONLY publish this event when Hearing is set for ready for video
            if (hearing.Status == Domain.Enumerations.BookingStatus.Created)
            {
                await PublishParticipantsAddedEvent(participants, hearing);
            }

            return NoContent();
        }

        /// <summary>
        /// Remove a participant from a hearing
        /// </summary>
        /// <param name="hearingId">Id of hearing to look up</param>
        /// <param name="participantId">Id of participant to remove</param>
        /// <returns></returns>
        [HttpDelete("{hearingId}/participants/{participantId}", Name = "RemoveParticipantFromHearing")]
        [SwaggerOperation(OperationId = "RemoveParticipantFromHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveParticipantFromHearing(Guid hearingId, Guid participantId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound();
            }

            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
                return BadRequest(ModelState);
            }

            List<Participant> participants;
            var query = new GetParticipantsInHearingQuery(hearingId);
            try
            {
                participants = await _queryHandler.Handle<GetParticipantsInHearingQuery, List<Participant>>(query);
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }

            var participant = participants.FirstOrDefault(x => x.Id == participantId);
            if (participant == null)
            {
                return NotFound();
            }

            var command = new RemoveParticipantFromHearingCommand(hearingId, participant);
            await _commandHandler.Handle(command);

            // ONLY publish this event when Hearing is set for ready for video
            if (videoHearing.Status == Domain.Enumerations.BookingStatus.Created)
            {
                await _eventPublisher.PublishAsync(new ParticipantRemovedIntegrationEvent(hearingId, participantId));
            }

            return NoContent();
        }

        /// <summary>
        /// Update participant details
        /// </summary>
        /// <param name="hearingId">Id of hearing to look up</param>
        /// <param name="participantId">Id of participant to remove</param>
        /// <param name="request">The participant information to add</param>
        /// <returns></returns>
        [HttpPut("{hearingId}/participants/{participantId}", Name = "UpdateParticipantDetails")]
        [SwaggerOperation(OperationId = "UpdateParticipantDetails")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateParticipantDetails(Guid hearingId, Guid participantId, [FromBody]UpdateParticipantRequest request)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
                return BadRequest(ModelState);
            }

            var result = new UpdateParticipantRequestValidation().Validate(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound();
            }
            Participant participant = null;
            var participants = videoHearing.GetParticipants();
            if (participants != null)
            {
                participant = participants.SingleOrDefault(x => x.Id.Equals(participantId));
            }

            if (participant == null)
            {
                return NotFound();
            }

            if (participant.HearingRole.UserRole.IsIndividual)
            {
                var addressValidationResult = new AddressValidation().Validate(request);
                if (!addressValidationResult.IsValid)
                {
                    ModelState.AddFluentValidationErrors(result.Errors);
                    return BadRequest(ModelState);
                }
            }

            if (participant.HearingRole.UserRole.IsRepresentative)
            {
                var repValidationResult = new RepresentativeValidation().Validate(request);
                if (!repValidationResult.IsValid)
                {
                    ModelState.AddFluentValidationErrors(result.Errors);
                    return BadRequest(ModelState);
                }
            }

            var mapper = new UpdateParticipantRequestToNewAddressMapper();
            var address = mapper.MapRequestToNewAddress(request);

            var representativeMapper = new UpdateParticipantRequestToNewRepresentativeMapper();
            var representative = representativeMapper.MapRequestToNewRepresentativeInfo(request);

            var updateParticipantCommand = new UpdateParticipantCommand(participantId, request.Title,
                request.DisplayName, request.TelephoneNumber, address,
                request.OrganisationName, videoHearing, representative);

            await _commandHandler.Handle(updateParticipantCommand);

            var updatedParticipant = updateParticipantCommand.UpdatedParticipant;

            var participantMapper = new ParticipantToResponseMapper();

            ParticipantResponse response = null;
            if (updatedParticipant != null)
            {
                response = participantMapper.MapParticipantToResponse(updatedParticipant);
            }

            // ONLY publish this event when Hearing is set for ready for video
            if (videoHearing.Status == Domain.Enumerations.BookingStatus.Created)
            { 
                await _eventPublisher.PublishAsync(new ParticipantUpdatedIntegrationEvent(hearingId, updatedParticipant));
            }

            return Ok(response);
        }

        /// <summary>
        /// Updates suitability answers for the participant
        /// </summary>
        /// <param name="hearingId">Id of hearing</param>
        /// <param name="participantId">Id of participant</param>
        /// <param name="answers">A list of suitability answers to update</param>
        /// <returns>Http status</returns>
        [HttpPut("{hearingId}/participants/{participantId}/suitability-answers", Name = "UpdateSuitabilityAnswers")]
        [SwaggerOperation(OperationId = "UpdateSuitabilityAnswers")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateSuitabilityAnswers(Guid hearingId, Guid participantId, [FromBody]List<SuitabilityAnswersRequest> answers)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
                return BadRequest(ModelState);
            }

            // Reject any requests with duplicate keys
            var duplicateKeyFound = answers.GroupBy(x => x.Key).Any(g => g.Count() > 1);
            if (duplicateKeyFound)
            {
                ModelState.AddModelError(nameof(participantId), $"Request '{nameof(answers)}' cannot contain duplicate keys.");
                return BadRequest(ModelState);
            }

            var suitabilityAnswers = answers.Select(answer => new SuitabilityAnswer(answer.Key, answer.Answer, answer.ExtendedAnswer))
                                .ToList();

            var command = new UpdateSuitabilityAnswersCommand(hearingId, participantId, suitabilityAnswers);

            try
            {
                await _commandHandler.Handle(command);
            }
            catch (DomainRuleException e)
            {
                ModelState.AddDomainRuleErrors(e.ValidationFailures);
                return BadRequest(ModelState);
            }
            catch (HearingNotFoundException)
            {
                return NotFound("Hearing not found");
            }
            catch (ParticipantNotFoundException)
            {
                return NotFound("Participant not found");
            }

            return NoContent();
        }

        private async Task PublishParticipantsAddedEvent(IEnumerable<NewParticipant> newParticipants, Hearing hearing)
        {
            var participants = hearing.GetParticipants().Where(x => newParticipants.Any(y => y.Person.Username == x.Person.Username));
            if (participants != null && participants.Any())
            {
                await _eventPublisher.PublishAsync(new ParticipantsAddedIntegrationEvent(hearing.Id, participants));
            }
        }
    }
}
