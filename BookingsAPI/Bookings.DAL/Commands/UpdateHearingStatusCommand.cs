using System;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Bookings.Domain.Validations;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class UpdateHearingStatusCommand : ICommand
    {
        public Guid HearingId { get; set; }
        public string Status { get; set; }
        public string UpdatedBy { get; set; }
        public UpdateHearingStatusCommand(Guid hearingId, string status, string updatedBy)
        {
            HearingId = hearingId;
            Status = status;
            UpdatedBy = updatedBy;
        }
    }

    public class UpdateHearingStatusCommandHandler : ICommandHandler<UpdateHearingStatusCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateHearingStatusCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateHearingStatusCommand command)
        {
            var hearing = await _context.VideoHearings.SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var bookingStatus = Enum.Parse<BookingStatus>(command.Status);
            hearing.UpdateStatus(bookingStatus, command.UpdatedBy);
            await _context.SaveChangesAsync();
        }
    }
}