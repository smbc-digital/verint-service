using verint_service.Mappers;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Mappers
{
    public class FwtCaseAssociatedOrganisationToOrganisationTests
    {
        [Fact]
        public void MapToCustomer_ShouldMapSocialContacts()
        {
            // Arrange
            var baseOrg = new FWTOrganisation
            {
                SocialContacts = new []
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
                }
            };

            // Act
            var mappedOrg = baseOrg.MapToOrganisation();

            // Assert
            Assert.Equal("id 1", mappedOrg.SocialContacts[0].Value);
            Assert.Equal("channel 1", mappedOrg.SocialContacts[0].Type);
            Assert.Equal("id 2", mappedOrg.SocialContacts[1].Value);
            Assert.Equal("channel 2", mappedOrg.SocialContacts[1].Type);
        }

        [Fact]
        public void MapToCustomer_ShouldMapName()
        {
            // Arrange
            var baseOrg = new FWTOrganisation
            {
                Name = new []
                {
                    new FWTOrganisationName
                    {
                        FullName = "Organisation name"
                    }
                }
            };

            // Act
            var mappedOrg = baseOrg.MapToOrganisation();

            // Assert
            Assert.Equal("Organisation name", mappedOrg.Name);
        }

        [Fact]
        public void MapToCustomer_ShouldMapContactEmails()
        {
            // Arrange
            var baseOrg = new FWTOrganisation
            {
                ContactEmails = new[]
                {
                    new FWTContactEmail
                    {
                        EmailAddress = "test email 1"
                    },
                    new FWTContactEmail
                    {
                        EmailAddress = "test email 2"
                    }
                }
            };

            // Act
            var mappedOrg = baseOrg.MapToOrganisation();

            // Assert
            Assert.Equal("test email 1", mappedOrg.Email);
        }
    }
}
