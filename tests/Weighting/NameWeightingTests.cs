using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Weighting
{
    public class NameWeightingTests
    {
        private readonly Mock<ILogger<NameWeighting>> _mockLogger = new Mock<ILogger<NameWeighting>>();


        [Fact]
        public void Calculate_Should_Return_0_If_IndividualName_IsNull()
        {
            // Arrange 
            var weighting = new NameWeighting(_mockLogger.Object);
            var individual = new FWTIndividual();

            // Act
            var customer = new Customer()
            {
                Forename = "Test",
                Surname = "Test"
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Calculate_Should_Return_1_If_Forename_Matches()
        {
            // Arrange 
            var weighting = new NameWeighting();
            var individual = new FWTIndividual()
            {
                Name = new FWTIndividualName []
                {
                    new FWTIndividualName
                    {
                        Forename = new string[] { "Test" }
                    }
                }
            };

            // Act
            var customer = new Customer()
            {
                Forename = "Test",
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Calculate_Should_Return_Minus10_If_Forename_DoesNotMatch()
        {
            // Arrange 
            var weighting = new NameWeighting();
            var individual = new FWTIndividual()
            {
                Name = new FWTIndividualName []
                {
                    new FWTIndividualName
                    {
                        Forename = new string[] { "Test" }
                    }
                }
            };

            // Act
            var customer = new Customer()
            {
                Forename = "NonMatch",
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(-10, result);
        }

        [Fact]
        public void Calculate_Should_Return_1_If_Surname_Matches()
        {
            // Arrange 
            var weighting = new NameWeighting();
            var individual = new FWTIndividual()
            {
                Name = new FWTIndividualName []
                {
                    new FWTIndividualName
                    {
                        Surname = "Test" 
                    }
                }
            };

            // Act
            var customer = new Customer()
            {
                Surname = "Test",
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Calculate_Should_Return_Minus10_If_Surname_Does_Not_Match()
        {
            // Arrange 
            var weighting = new NameWeighting();
            var individual = new FWTIndividual()
            {
                Name = new FWTIndividualName []
                {
                    new FWTIndividualName
                    {
                        Surname = "Test" 
                    }
                }
            };

            // Act
            var customer = new Customer()
            {
                Surname = "NonMatch",
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(-10, result);
        }

        [Fact]
        public void Calculate_Should_Return_1_If_Title_Matches()
        {
            // Arrange 
            var weighting = new NameWeighting();
            var individual = new FWTIndividual()
            {
                Name = new FWTIndividualName []
                {
                    new FWTIndividualName
                    {
                        Title = "Mr" 
                    }
                }
            };

            // Act
            var customer = new Customer()
            {
                Title = "Mr",
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Calculate_Should_Return_1_If_Initials_Matches()
        {
            // Arrange 
            var weighting = new NameWeighting();
            var individual = new FWTIndividual()
            {
                Name = new FWTIndividualName []
                {
                    new FWTIndividualName
                    {
                        Initials = "A T" 
                    }
                }
            };

            // Act
            var customer = new Customer()
            {
                Initials = "A T" 
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Calculate_Should_Return_Maximum_Of_2_If_All_Matches()
        {
            // Arrange 
            var weighting = new NameWeighting();
            var individual = new FWTIndividual()
            {
                Name = new FWTIndividualName []
                {
                    new FWTIndividualName
                    {
                        Forename = new string[] { "Test" },
                        Surname = "Tester",
                        Title = "Mr",
                        Initials = "A T" 
                    }
                }
            };

            // Act
            var customer = new Customer()
            {
                Title = "Mr",
                Forename = "Test",
                Surname = "Tester",
                Initials = "A T" 
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Calculate_Should_Return_Minus20_If_No_Matches()
        {
            // Arrange 
            var weighting = new NameWeighting();
            var individual = new FWTIndividual()
            {
                Name = new FWTIndividualName []
                {
                    new FWTIndividualName
                    {
                        Forename = new string[] { "Test" },
                        Surname = "Tester",
                        Title = "Mr",
                        Initials = "A T" 
                    }
                }
            };

            // Act
            var customer = new Customer()
            {
                Title = "Mrs",
                Forename = "Testing",
                Surname = "TesterTon",
                Initials = "A M" 
            };

            var result = weighting.Calculate(individual, customer);

            // Assert
            Assert.Equal(-20, result);
        }
    }
}