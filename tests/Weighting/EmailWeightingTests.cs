using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Weighting
{
    public class EmailWeightingTests
    {
        private readonly Mock<ILogger<EmailWeighting>> _mockLogger = new Mock<ILogger<EmailWeighting>>();

        [Fact]
        public void Calculate_Should_Return_0_If_CustomerEmail_IsNull()
        {
            // Arrange 
            var weighting = new EmailWeighting(_mockLogger.Object);
            var individual = new FWTIndividual
            {
                ContactEmails = new FWTContactEmail[]
                {
                    new FWTContactEmail()
                    {
                        EmailAddress = "test@stockport.gov.uk"
                    }
                }
            };

            // Act
            var customer = new Customer();
            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_Should_Return_0_If_IndividualContactEmails_IsNull()
        {
            // Arrange 
            var weighting = new EmailWeighting(_mockLogger.Object);

            // Act
            var customer = new Customer()
            {
                Email = "test@stockport.gov.uk"
            };

            var individual = new FWTIndividual();

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_Should_Return_0_If_IndividualContactEmails_IsEmpty()
        {
            // Arrange 
            var weighting = new EmailWeighting(_mockLogger.Object);

            // Act
            var customer = new Customer()
            {
                Email = "test@stockport.gov.uk"
            };

            var individual = new FWTIndividual
            {
                ContactEmails = new FWTContactEmail[0]
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

                [Fact]
        public void Calculate_Should_Return_2_If_IndividualContactEmails_IsThereIsAMatchingEmail()
        {
            // Arrange 
            var weighting = new EmailWeighting(_mockLogger.Object);

            // Act
            var customer = new Customer()
            {
                Email = "test@stockport.gov.uk"
            };

            var individual = new FWTIndividual
            {
                ContactEmails = new FWTContactEmail[]
                {
                    new FWTContactEmail {
                        EmailAddress = "test@stockport.gov.uk"
                    } 
                }
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(2, result);
        }
    }
}