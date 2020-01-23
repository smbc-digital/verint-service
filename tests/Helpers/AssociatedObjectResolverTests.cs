using StockportGovUK.NetStandard.Models.Verint;
using verint_service;
using verint_service.Helpers;
using Xunit;

namespace verint_service_tests.ModelBinders
{
    public class AssociatedObjectResolverTests
    {
        [Fact]
        public void AssociatedObjectResolver_returns_streetobjectcode_when_case_has_a_street()
        {
            // Arrange
            var testCase = new Case()
            {
                Street = new Street(){
                    USRN = "38102548",
                    Reference = "38102548",
                    Description = "Hibbert Lane"
                }
            };

            var helper = new AssociatedObjectResolver();

            // Act 
            var result = helper.Resolve(testCase); 

            // Assert
            Assert.Equal(result.ObjectID.ObjectType, Common.StreetObjectType);
            Assert.Equal("38102548", result.ObjectID.ObjectReference[0]);
        }

        [Fact]
        public void AssociatedObjectResolver_returns_property0bjectcode_when_case_has_an_property()
        {
            // Arrange
            var testCase = new Case()
            {
                Property = new Address
                {
                    AddressLine1 = "29 Hibbert Lane",
                    AddressLine2 = "Marple",
                    AddressLine3 = "Stockport",
                    Postcode = "SK6 7NZ",
                    Reference = "1010035673111"
                }
            };

            var helper = new AssociatedObjectResolver();

            // Act 
            var result = helper.Resolve(testCase); 

            // Assert
            Assert.Equal(result.ObjectID.ObjectType, Common.PropertyObjectType);
            Assert.Equal("1010035673111", result.ObjectID.ObjectReference[0]);
        }
        
        [Fact]
        public void AssociatedObjectResolver_returns_organisationobjectcode_when_case_has_an_organisation()
        {
            // Arrange
            var testCase = new Case()
            {
                Organisation = new Organisation
                {
                    Description = "Stockport Council",
                    Reference = "101002073523"
                }
            };

            var helper = new AssociatedObjectResolver();

            // Act 
            var result = helper.Resolve(testCase); 

            // Assert
            Assert.Equal(result.ObjectID.ObjectType, Common.OrganisationObjectType);
            Assert.Equal("101002073523", result.ObjectID.ObjectReference[0]);
        }

        [Fact]
        public void AssociatedObjectResolver_returns_individualobjectcode_when_case_has_a_customer()
        {
            // Arrange
            var testCase = new Case()
            {
                Customer = new Customer
                {
                    CustomerReference = "101003278921",
                    Forename = "Test",
                    Surname = "Tester"
                }
            };

            var helper = new AssociatedObjectResolver();

            // Act 
            var result = helper.Resolve(testCase); 

            // Assert
            Assert.Equal(result.ObjectID.ObjectType, Common.IndividualObjectType);
            Assert.Equal("101003278921", result.ObjectID.ObjectReference[0]);
        }
    }
}