using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetPersonBySearchTermQueryHandlerTests : DatabaseTestsBase
    {
        private GetPersonBySearchTermQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetPersonBySearchTermQueryHandler(context);
        }

        [Test]
        public async Task Should_find_contact_by_email_case_insensitive()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var person = seededHearing.GetPersons().First();
            var contactEmail = person.ContactEmail;
            
            // Build a search term based on lower and upper case of the expected email
            var twoCharactersLowercase = contactEmail.Substring(0, 2).ToLower();
            var twoCharactersUppercase = contactEmail.Substring(2, 2).ToUpper(); 
            var searchTerm =  twoCharactersLowercase + twoCharactersUppercase;
            var matches  = await _handler.Handle(new GetPersonBySearchTermQuery(searchTerm));

            matches.Select(m => m.Id).Should().Contain(person.Id);
        }
    }
}