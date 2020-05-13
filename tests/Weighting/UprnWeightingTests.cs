using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
using Xunit;




namespace verint_service_tests.Weighting
{
    public class UprnWeightingTests
    {
        private readonly Mock<ILogger<UprnWeighting>> _mockLogger = new Mock<ILogger<UprnWeighting>>();


        [Fact]
        public void Calculate_Should_Return_0_If_CustomerAddress_NotProvided()
        {
            
            var weighting = new UprnWeighting(_mockLogger.Object);
            var individual = new FWTIndividual
            {
                ContactPostals = new FWTContactPostal[] 
                { 
                    new FWTContactPostal
                    {
                        UPRN = "TestUprn"
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
        public void Calculate_Should_Return_0_If_IndividualContactPostal_IsNull()
        {
            // Arrange 
            var weighting = new UprnWeighting(_mockLogger.Object);
            var individual = new FWTIndividual();

            // Act
            var customer = new Customer()
            {
                Address = new Address
                {
                    UPRN = "TestUprn"
                }
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_Should_Return_2_If_AddressUprn_Match()
        {
            // Arrange 
            var weighting = new UprnWeighting(_mockLogger.Object);
            var individual = new FWTIndividual
            {
                ContactPostals = new FWTContactPostal[] 
                { 
                    new FWTContactPostal
                    {
                        UPRN = "TestUprn"
                    }
                }
            };

            // Act
            var customer = new Customer()
            {
                Address = new Address
                {
                    UPRN = "TestUprn"
                }
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Calculate_Should_Return_0_If_AddressUprn_DoesntMatch()
        {
            // Arrange 
            var weighting = new UprnWeighting(_mockLogger.Object);
            var individual = new FWTIndividual
            {
                ContactPostals = new FWTContactPostal[] 
                { 
                    new FWTContactPostal
                    {
                        UPRN = "TestUprn"
                    }
                }
            };

            // Act
            var customer = new Customer()
            {
                Address = new Address
                {
                    UPRN = "NoUprn"
                }
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }
    }
}