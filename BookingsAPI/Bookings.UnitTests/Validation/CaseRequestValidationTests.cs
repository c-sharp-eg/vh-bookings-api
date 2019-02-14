using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Validation
{
    public class CaseRequestValidationTests
    {
        private CaseRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new CaseRequestValidation();
        }

        [Test]
        public async Task should_pass_validation()
        {
            var request = BuildRequest();
            
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_case_name_error()
        {
            var request = BuildRequest();
            request.Name = string.Empty;
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CaseRequestValidation.NoCaseNameMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_case_number_error()
        {
            var request = BuildRequest();
            request.Number = string.Empty;
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == CaseRequestValidation.NoCaseNumberMessage)
                .Should().BeTrue();
        }

        private CaseRequest BuildRequest()
        {
            return new CaseRequest
            {
                Name = "A vs B",
                Number = "1234567890",
                IsLeadCase = true
            };
        }
    }
}