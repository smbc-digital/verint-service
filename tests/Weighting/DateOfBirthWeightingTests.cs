using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Weighting
{
    public class DateOfBirthWeightingTests
    {
        private readonly Mock<ILogger<DateOfBirthWeighting>> _mockLogger = new Mock<ILogger<DateOfBirthWeighting>>();

        [Fact]
        public void Calculate_Should_Return_0_If_CustomeDateOfBirth_NotProvided()
        {
            // Arrange 
            var weighting = new DateOfBirthWeighting(_mockLogger.Object);
            var individual = new FWTIndividual
            {
                DateOfBirthSpecified = true,
                DateOfBirth = new System.DateTime()
            };

            // Act
            var customer = new Customer();
            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_Should_Return_0_If_IndividualDateOfBirth_IsNotSpecified()
        {
            // Arrange 
            var weighting = new DateOfBirthWeighting(_mockLogger.Object);

            // Act
            var customer = new Customer()
            {
                DateOfBirth = new System.DateTime()
            };

            var individual = new FWTIndividual
            {
                DateOfBirthSpecified = false,
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_Should_Return_2_If_DateOfBirth_IsTheSame()
        {
            // Arrange 
            var weighting = new DateOfBirthWeighting(_mockLogger.Object);
            var customer = new Customer()
            {
                DateOfBirth = new System.DateTime(2001, 1, 1)
            };

            var individual = new FWTIndividual
            {
                DateOfBirthSpecified = true,
                DateOfBirth =  new System.DateTime(2001, 1, 1)
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Calculate_Should_Return_Neg10_If_DateOfBirth_IsNotTheSame()
        {
            // Arrange 
            var weighting = new DateOfBirthWeighting(_mockLogger.Object);
            var customer = new Customer()
            {
                DateOfBirth = new System.DateTime(2000, 1, 1)
            };

            var individual = new FWTIndividual
            {
                DateOfBirthSpecified = true,
                DateOfBirth =  new System.DateTime(2001, 1, 1)
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(-10, result);
        }
    }
}