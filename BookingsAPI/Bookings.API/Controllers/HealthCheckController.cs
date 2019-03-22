using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("HealthCheck")]
    [ApiController]
    public class HealthCheckController : Controller
    {
        private readonly IQueryHandler _queryHandler;

        public HealthCheckController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Just some new test code to check the conditions
        /// </summary>
        /// <returns></returns>
        public IActionResult SomeNewCode()
        {
            var random = new Random().Next();
            if (random < 200)
            {
                return Ok("Result is less than 200");
            }
            
            if (random < 400)
            {
                return Ok("Result is less than 400");
            }
            
            if (random < 600)
            {
                return Ok("Result is less than 600");
            }
            
            return Ok(random);
        }
        

        /// <summary>
        ///     Run a health check of the service
        /// </summary>
        /// <returns>Error if fails, otherwise OK status</returns>
        [HttpGet("health")]
        [SwaggerOperation(OperationId = "CheckServiceHealth")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CheckServiceHealth()
        {
            try
            {
                var query = new GetHearingVenuesQuery();
                var hearingVenues = await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(query);

                if (!hearingVenues.Any())
                    throw new DataException("Could not retrieve ref data during service health check");
            }
            catch (Exception ex)
            {
                var data = new
                {
                    ex.Message,
                    ex.Data
                };
                return StatusCode((int) HttpStatusCode.InternalServerError, data);
            }

            return Ok();
        }
    }
}