using Bookings.DAL;
using Bookings.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetHearingVenuesQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetHearingVenuesQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingVenuesQueryHandler(context);
        }
        
        
        [Test]
        public void should_return_list_of_hearing_venues()
        {
            var venues = _handler.Handle(new GetHearingVenuesQuery());
            venues.Should().NotBeEmpty();
        }
    }
}