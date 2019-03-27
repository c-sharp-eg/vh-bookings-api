﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using Bookings.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetBookingsByCaseTypesQueryHandlerTest : DatabaseTestsBase
    {
        private GetBookingsByCaseTypesQueryHandler _handler;
        private List<Guid> _ids;
        private BookingsDbContext _context;

        private const string FinancialRemedy = "Financial Remedy";
        
        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetBookingsByCaseTypesQueryHandler(_context);
            _ids = new List<Guid>();
        }

        [Test]
        public async Task should_return_all_case_types_if_no_filter_is_given()
        {
            _ids.Add((await Hooks.SeedVideoHearing()).Id);
            _ids.Add((await Hooks.SeedVideoHearing(opts => opts.CaseTypeName = FinancialRemedy)).Id);
            
            // we have to (potentially) look through all the existing hearings to find these
            var query = new GetBookingsByCaseTypesQuery { Limit = _context.VideoHearings.Count() };
            var result = await _handler.Handle(query);

            var hearingIds = result.Select(hearing => hearing.Id).ToList();
            hearingIds.Should().Contain(_ids);

            var hearingTypes = result.Select(hearing => hearing.CaseType.Name).Distinct().ToList();
            hearingTypes.Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task should_only_return_filtered_case_types()
        {
            _ids.Add((await Hooks.SeedVideoHearing()).Id);
            var financialRemedyHearing = await Hooks.SeedVideoHearing(opt => opt.CaseTypeName = FinancialRemedy); 
            _ids.Add(financialRemedyHearing.Id);
            
            var query = new GetBookingsByCaseTypesQuery(new List<int>{financialRemedyHearing.CaseTypeId});
            var result = await _handler.Handle(query);

            var hearingIds = result.Select(hearing => hearing.Id).ToList();
            hearingIds.Should().Contain(financialRemedyHearing.Id);

            var hearingTypes = result.Select(hearing => hearing.CaseType.Name).Distinct().ToList();
            hearingTypes.Should().Equal(FinancialRemedy);
        }
        
        [Test]
        public async Task should_limit_hearings_returned()
        {
            _ids.Add((await Hooks.SeedVideoHearing()).Id);
            _ids.Add((await Hooks.SeedVideoHearing()).Id);
            var hearing = await Hooks.SeedVideoHearing();
            _ids.Add(hearing.Id);
            
            var query = new GetBookingsByCaseTypesQuery { Limit = 2 };
            var result = await _handler.Handle(query);

            result.Count.Should().Be(2);
        }
        
        [Test]
        public async Task should_return_different_hearings_for_each_new_page()
        {
            // When generating the guids they may end up being in order accidentally, therefor,
            // seed hearings until they end up in invalid order
            while (IdsAreInOrder(_ids) || _ids.Count < 3)
            {
                _ids.Add((await Hooks.SeedVideoHearing()).Id);
            }
            
            // And paging through the results
            string cursor = null;
            var allHearings = new List<VideoHearing>();
            while (true)
            {
                var result = await _handler.Handle(new GetBookingsByCaseTypesQuery { Limit = 1, Cursor = cursor });
                allHearings.AddRange(result);
                if (result.NextCursor == null) break;
                cursor = result.NextCursor;
            }
                        
            // They should all have different id's
            var ids = allHearings.Select(x => x.Id);
            ids.Distinct().Count().Should().Be(_context.VideoHearings.Count());
        }

        private static bool IdsAreInOrder(List<Guid> ids)
        {
            for (var i = 0; i < ids.Count - 1; ++i)
            {
                if (string.Compare(ids[i].ToString(), ids[i + 1].ToString(), StringComparison.Ordinal) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        [TearDown]
        public async Task TearDown()
        {
            foreach (var item in _ids)
            {
                TestContext.WriteLine($"Removing test hearing {item}");
                await Hooks.RemoveVideoHearing(item);
            }
        }
    }
}
