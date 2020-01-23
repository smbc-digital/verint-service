using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Weighting
{
    public class TelephoneWeightingTests
    {
        [Fact]
        public void Calculate_Should_Return_0_If_CustomerTelephone_IsNull()
        {
            // Arrange 
            var weighting = new TelephoneWeighting();
            var individual = new FWTIndividual
            {
                ContactPhones = new FWTContactPhone[]
                {
                    new FWTContactPhone()
                    {
                        Number = "0161 474 5432"
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
        public void Calculate_Should_Return_0_If_IndividualPhones_IsNull()
        {
            // Arrange 
            var weighting = new TelephoneWeighting();

            // Act
            var customer = new Customer()
            {
                Telephone = "0161 474 5432"
            };

            var individual = new FWTIndividual();

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_Should_Return_0_If_IndividualContactPhone_IsEmpty()
        {
            // Arrange 
            var weighting = new TelephoneWeighting();

            // Act
            var customer = new Customer()
            {
                Telephone = "0161 474 5432"
            };

            var individual = new FWTIndividual
            {
                ContactPhones = new FWTContactPhone[0]
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

                [Fact]
        public void Calculate_Should_Return_1_If_IndividualContactPhones_ThereIsAMatchingPhones()
        {
            // Arrange 
            var weighting = new TelephoneWeighting();

            // Act
            var customer = new Customer()
            {
                Telephone = "0161 474 5432"
            };

            var individual = new FWTIndividual
            {
                ContactPhones = new FWTContactPhone[]
                {
                    new FWTContactPhone {
                        Number = "0161 474 5432"
                    }
                }
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(1, result);
        }
    }
}