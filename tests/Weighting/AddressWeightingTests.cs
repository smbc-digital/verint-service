using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Services.Individual.Weighting;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Weighting
{
    public class AddressWeightingTests
    {
        private readonly Mock<ILogger<AddressWeighting>> _mockLogger = new Mock<ILogger<AddressWeighting>>();

        [Fact]
        public void Calculate_Should_Return_0_If_CustomerAddress_IsNull()
        {
            // Arrange 
            var weighting = new AddressWeighting(_mockLogger.Object);
            var individual = new FWTIndividual()
            {
                ContactPostals = new FWTContactPostal [0]
            };

            // Act
            var customer = new Customer();

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_Should_Return_0_If_ContactPostals_IsNull()
        {
            // Arrange 
            var weighting = new AddressWeighting(_mockLogger.Object);
            var individual = new FWTIndividual();

            // Act
            var customer = new Customer(){
                Address = new Address()
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

        
        [Fact]
        public void Calculate_Should_Return_0_If_UprnIsNotNull()
        {
            // Arrange 
            var weighting = new AddressWeighting(_mockLogger.Object);
            var individual = new FWTIndividual()
            {
                ContactPostals = new FWTContactPostal[0]
            };

            // Act
            var customer = new Customer(){
                Address = new Address()
                {
                    UPRN = "12345678"
                }
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_Should_Return_1_If_PostcodeAndNumberMatch()
        {
            // Arrange 
            var weighting = new AddressWeighting(_mockLogger.Object);
            var individual = new FWTIndividual()
            {
                ContactPostals = new FWTContactPostal[]
                {
                    new FWTContactPostal {
                        AddressNumber = "1",
                        Postcode = "SK1 3XE"

                    }
                }
            };

            // Act
            var customer = new Customer(){
                Address = new Address()
                {
                    Postcode = "SK1 3XE",
                    Number = "1"
                }
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Calculate_Should_Return_1_If_AddressLine1_City_Postcode_AreAMatch()
        {
            // Arrange 
            var weighting = new AddressWeighting(_mockLogger.Object);
            var individual = new FWTIndividual()
            {
                ContactPostals = new FWTContactPostal[]
                {
                    new FWTContactPostal {
                        AddressLine = new string[2] {
                            "Address line 1",
                            null
                        },
                        City = "city",
                        Postcode = "sk11aa"
                    }
                }
            };

            // Act
            var customer = new Customer(){
                Address = new Address()
                {
                    AddressLine1 = "Address line 1",
                    City = "city",
                    Postcode = "sk11aa"
                }
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Calculate_Should_Return_1_If_FullAddressMatch()
        {
            // Arrange 
            var weighting = new AddressWeighting(_mockLogger.Object);
            var individual = new FWTIndividual()
            {
                ContactPostals = new FWTContactPostal[]
                {
                    new FWTContactPostal {
                        AddressLine = new string[2] {
                            "Test Line 1",
                            "Test Line 2"
                        },
                        City = "Stockport",
                        AddressNumber = "1",
                        Postcode = "SK1 3XE"
                    }
                }
            };

            // Act
            var customer = new Customer(){
                Address = new Address()
                {
                    AddressLine1 = "Test Line 1",
                    AddressLine2 = "Test Line 2",
                    City = "Stockport",
                    Number = "1",
                    Postcode = "SK1 3XE"
                }
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(1, result);
        }
    }
}