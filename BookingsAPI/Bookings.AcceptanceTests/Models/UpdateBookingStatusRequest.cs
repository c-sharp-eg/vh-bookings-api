﻿
namespace Bookings.AcceptanceTests.Models
{
    internal static class UpdateBookingStatusRequest
    {
        public static Api.Contract.Requests.UpdateBookingStatusRequest BuildRequest()
        {
            return new Api.Contract.Requests.UpdateBookingStatusRequest
            {
                UpdatedBy = "test",
                Status = Api.Contract.Requests.Enums.UpdateBookingStatus.Cancelled
            };
        }
    }
}