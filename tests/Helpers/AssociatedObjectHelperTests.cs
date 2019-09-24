using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using verint_service.Helpers;
using verint_service.ModelBinders;
using verint_service.Models;
using Xunit;

namespace verint_service_tests.ModelBinders
{
    public class AssociatedObjectHelperTests
    {
        [Fact]
        public void AssociatedObjectHelper_returns_streetobjectcode_when_case_has_a_street()
        {
            // Arrange
            var testCase = new verint_service.Models.Case()
            {
                Street = new Street(){
                    USRN = "38102548",
                    Reference = "38102548",
                    Description = "Hibbert Lane"
                }
            };

            var helper = new AssociatedObjectHelper();

            // Act 
            var result = helper.GetAssociatedObject(testCase); 

            // Assert
            Assert.Equal(result.ObjectID.ObjectType, Common.StreetObjectType);
            Assert.Equal("38102548", result.ObjectID.ObjectReference[0]);
        }

        [Fact]
        public void AssociatedObjectHelper_returns_property0bjectcode_when_case_has_an_property()
        {
            // Arrange
            var testCase = new verint_service.Models.Case()
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

            var helper = new AssociatedObjectHelper();

            // Act 
            var result = helper.GetAssociatedObject(testCase); 

            // Assert
            Assert.Equal(result.ObjectID.ObjectType, Common.PropertyObjectType);
            Assert.Equal("1010035673111", result.ObjectID.ObjectReference[0]);
        }
        
        [Fact]
        public void AssociatedObjectHelper_returns_organisationobjectcode_when_case_has_an_organisation()
        {
            // Arrange
            var testCase = new verint_service.Models.Case()
            {
                Organisation = new Organisation
                {
                    Description = "Stockport Council",
                    Reference = "101002073523"
                }
            };

            var helper = new AssociatedObjectHelper();

            // Act 
            var result = helper.GetAssociatedObject(testCase); 

            // Assert
            Assert.Equal(result.ObjectID.ObjectType, Common.OrganisationObjectType);
            Assert.Equal("101002073523", result.ObjectID.ObjectReference[0]);
        }

        [Fact]
        public void AssociatedObjectHelper_returns_individualobjectcode_when_case_has_a_customer()
        {
            // Arrange
            var testCase = new verint_service.Models.Case()
            {
                Customer = new Customer
                {
                    CustomerReference = "101003278921",
                    Forename = "Test",
                    Surname = "Tester"
                }
            };

            var helper = new AssociatedObjectHelper();

            // Act 
            var result = helper.GetAssociatedObject(testCase); 

            // Assert
            Assert.Equal(result.ObjectID.ObjectType, Common.IndividualObjectType);
            Assert.Equal("101003278921", result.ObjectID.ObjectReference[0]);
        }
    }
}