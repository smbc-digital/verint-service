using System;
using System.Linq;
using Moq;
using verint_service.Helpers.VerintConnection;
using verint_service.Services;
using verint_service.Utils.Mappers;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Mappers
{
    public class FwtCaseAssociatedIndividualToCustomerTests
    {
        [Fact]
        public void MapToCustomer_ShouldMapUser()
        {
            // Arrange
            var baseIndividual = CreateBaseIndividual();

            // Act
            var mappedIndividual = baseIndividual.MapToCustomer();

            // Assert
            Assert.Equal(baseIndividual.Name.First().Forename.First(), mappedIndividual.Forename);
            Assert.Equal(baseIndividual.Name.First().Surname, mappedIndividual.Surname);
        }

        [Fact]
        public void MapToCustomer_ShouldMapPreferredAddress()
        {
            // Arrange
            var baseIndividual = CreateBaseIndividual();
            baseIndividual.ContactPostals = new FWTContactPostal[]
            {
                new FWTContactPostal
                {
                    Preferred = true,
                    UPRN = "1234",
                    AddressLine = new []
                    {
                        "Line 1",
                        "Line 2",
                        "Line 3"
                    },
                    City = "City",
                    AddressNumber = "1234",
                },
                new FWTContactPostal
                {
                    Preferred = false
                }
            };

            // Act 
            var mappedIndividual = baseIndividual.MapToCustomer();

            // Assert
            Assert.Equal("Line 1", mappedIndividual.Address.AddressLine1);
            Assert.Equal("Line 2", mappedIndividual.Address.AddressLine2);
            Assert.Equal("Line 3", mappedIndividual.Address.AddressLine3);
            Assert.Equal("1234", mappedIndividual.Address.UPRN);
            Assert.Equal("City", mappedIndividual.Address.City);
            Assert.Equal("1234", mappedIndividual.Address.Number);
        }

        [Fact]
        public void MapToCustomer_ShouldMapFirstAddress()
        {
            // Arrange
            var baseIndividual = CreateBaseIndividual();
            baseIndividual.ContactPostals = new FWTContactPostal[]
            {
                new FWTContactPostal
                {
                    Preferred = true,
                    UPRN = "1234",
                    AddressLine = new []
                    {
                        "Line 1",
                        "Line 2",
                        "Line 3"
                    },
                    City = "City",
                    AddressNumber = "1234",
                },
            };

            // Act 
            var mappedIndividual = baseIndividual.MapToCustomer();

            // Assert
            Assert.Equal("Line 1", mappedIndividual.Address.AddressLine1);
            Assert.Equal("Line 2", mappedIndividual.Address.AddressLine2);
            Assert.Equal("Line 3", mappedIndividual.Address.AddressLine3);
            Assert.Equal("1234", mappedIndividual.Address.UPRN);
            Assert.Equal("City", mappedIndividual.Address.City);
            Assert.Equal("1234", mappedIndividual.Address.Number);
        }

        [Fact]
        public void MapToCustomer_ShouldMapSocialContacts()
        {
            // Arrange
            var baseIndividual = CreateBaseIndividual();
            baseIndividual.SocialContacts = new[]
            {
                new FWTSocialContact
                {
                    SocialID = "id 1",
                    SocialChannel = "channel 1"
                },
                new FWTSocialContact
                {
                    SocialID = "id 2",
                    SocialChannel = "channel 2"
                }
            };

            // Act
            var mappedIndividual = baseIndividual.MapToCustomer();

            // Assert
            Assert.Equal("id 1", mappedIndividual.SocialContacts[0].Value);
            Assert.Equal("channel 1", mappedIndividual.SocialContacts[0].Type);
            Assert.Equal("id 2", mappedIndividual.SocialContacts[1].Value);
            Assert.Equal("channel 2", mappedIndividual.SocialContacts[1].Type);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapToCustomer_ShouldMapEmailAddress(bool preferred)
        {
            // Arrange
            var baseIndividual = CreateBaseIndividual();
            baseIndividual.ContactEmails = new []
            {
                new FWTContactEmail
                {
                    Preferred = preferred,
                    EmailAddress = "email@mail.com"
                } 
            };

            // Act
            var mappedIndividual = baseIndividual.MapToCustomer();

            // Assert
            Assert.Equal("email@mail.com", mappedIndividual.Email);
        }

        private FWTIndividual CreateBaseIndividual()
        {
            return new FWTIndividual
            {
                Name = new []
                {
                    new FWTIndividualName
                    {
                        Surname = "test surname",
                        Forename = new []
                        {
                           "test forename 1",
                           "test forename 2"
                        }
                    }
                }
            };
        }
    }
}
