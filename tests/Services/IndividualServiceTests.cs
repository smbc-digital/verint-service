using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Helpers.VerintConnection;
using verint_service.Services;
using verint_service.Services.Individual;
using verint_service.Services.Individual.Weighting;
using verint_service.Services.Property;
using verint_service_tests.Builders;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Services
{
    public class IndividualServiceTests
    {
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly Mock<IVerintClient> _mockClient = new Mock<IVerintClient>();
        private readonly Mock<IPropertyService> _mockPropertyService = new Mock<IPropertyService>();
        private readonly Mock<ILogger<IndividualService>> _mockLogger = new Mock<ILogger<IndividualService>>();
        private readonly Mock<IIndividualWeighting> _mockIndividualWeighting = new Mock<IIndividualWeighting>();
        private readonly IEnumerable<IIndividualWeighting> _individualWeightings;

        private readonly IndividualService _service;

        public IndividualServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);

            _individualWeightings = new List<IIndividualWeighting>
            {
                _mockIndividualWeighting.Object
            };

            _service = new IndividualService(_mockConnection.Object, _individualWeightings, _mockPropertyService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ResolveAsync_ShouldCallVerint_SearchPartyAsync_Once_WhenMatchingUserFound_OnInitialSearch()
        {
            // Arrange
             _mockIndividualWeighting.Setup(_ => _.Calculate(It.IsAny<FWTIndividual>(), It.IsAny<Customer>()))
                .Returns(2);

            var userSearchResponse = new FWTObjectBriefDetails 
            {
                ObjectID = new FWTObjectID()
            };

            _mockClient.Setup(_ => _.searchForPartyAsync(It.IsAny<FWTPartySearch>()))
                .ReturnsAsync(new searchForPartyResponse{ FWTObjectBriefDetailsList = new FWTObjectBriefDetails[1]{ userSearchResponse } });

            _mockClient.Setup(_ => _.retrieveIndividualAsync(It.IsAny<FWTObjectID>()))
                .ReturnsAsync(new retrieveIndividualResponse{ FWTIndividual = new FWTIndividual { BriefDetails = new FWTObjectBriefDetails { ObjectID = new FWTObjectID() } } });

            var customer = new CustomerBuilder()
                .Build();
            
            // Act
            var result = await _service.ResolveAsync(customer);

            // Assert
            Assert.NotNull(result);
            _mockClient.Verify(_ => _.searchForPartyAsync(It.Is<FWTPartySearch>(x => x.SearchType == "individual" && x.Forename == "forename" && x.Name == "surname")), Times.Once);
            _mockClient.Verify(_ => _.retrieveIndividualAsync(It.IsAny<FWTObjectID>()), Times.Exactly(1));
        }
        
        [Fact]
        public async Task ResolveAsync_ShouldCallVerint_SearchPartyAsync_WithEmailSearchCriteria_WhenMatchingUserNotFound_OnInitialSearch()
        {
            // Arrange
             _mockIndividualWeighting.SetupSequence(_ => _.Calculate(It.IsAny<FWTIndividual>(), It.IsAny<Customer>()))
                .Returns(0)
                .Returns(2);

            var userSearchResponse = new FWTObjectBriefDetails 
            {
                ObjectID = new FWTObjectID()
            };

            _mockClient.Setup(_ => _.searchForPartyAsync(It.IsAny<FWTPartySearch>()))
                .ReturnsAsync(new searchForPartyResponse{ FWTObjectBriefDetailsList = new FWTObjectBriefDetails[1]{ userSearchResponse } });

            _mockClient.Setup(_ => _.retrieveIndividualAsync(It.IsAny<FWTObjectID>()))
                .ReturnsAsync(new retrieveIndividualResponse{ FWTIndividual = new FWTIndividual { BriefDetails = new FWTObjectBriefDetails { ObjectID = new FWTObjectID() } } });

            var customer = new CustomerBuilder()
                .WithEmail("email@test.com")
                .Build();
            
            // Act
            var result = await _service.ResolveAsync(customer);

            // Assert
            Assert.NotNull(result);
            _mockClient.Verify(_ => _.searchForPartyAsync(It.Is<FWTPartySearch>(x => x.SearchType == "individual" && x.EmailAddress == "email@test.com")), Times.Exactly(2));
            _mockClient.Verify(_ => _.retrieveIndividualAsync(It.IsAny<FWTObjectID>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ResolveAsync_ShouldCreateCustomer_WhenNonFoundAfterAll_SearchPartyAsyncCalls_Performed()
        {
            // Arrange
            _mockClient.Setup(_ => _.searchForPartyAsync(It.IsAny<FWTPartySearch>()))
                .ReturnsAsync(new searchForPartyResponse { FWTObjectBriefDetailsList = new FWTObjectBriefDetails[0] });

            _mockClient.Setup(_ => _.createIndividualAsync(It.IsAny<FWTIndividual>()))
                .ReturnsAsync(new createIndividualResponse{ FLNewIndividualID = new FWTObjectID() });
           
            var customer = new CustomerBuilder()
                .WithForename("forename")
                .WithSurname("surname")
                .WithEmail("email@test.com")
                .WithTelephone("12345")
                .WithAddress(new Address{ Postcode = "sk11aa", Number = "123" })
                .Build();

            // Act
            await _service.ResolveAsync(customer);

            // Assert
            _mockConnection.Verify(_ => _.Client().createIndividualAsync(It.IsAny<FWTIndividual>()), Times.Once);
            _mockClient.Verify(_ => _.searchForPartyAsync(It.Is<FWTPartySearch>(x => x.SearchType == "individual" && x.PhoneNumber == "12345")), Times.Exactly(2));
            _mockClient.Verify(_ => _.searchForPartyAsync(It.Is<FWTPartySearch>(x => x.SearchType == "individual" && x.EmailAddress == "email@test.com")), Times.Exactly(2));
            _mockClient.Verify(_ => _.searchForPartyAsync(It.Is<FWTPartySearch>(x => x.SearchType == "individual" && x.AddressNumber == "123" && x.Postcode == "sk11aa")), Times.Exactly(2));
            _mockClient.Verify(_ => _.createIndividualAsync(It.IsAny<FWTIndividual>()), Times.Once);
        }

        [Fact]
        public async Task UpdateIndividual_ShouldCallVerintConnection_AndUpdateIndividual()
        {
            // Arrange
            var individual = new FWTIndividual
            {
                BriefDetails = new FWTObjectBriefDetails
                {
                    ObjectID = new FWTObjectID
                    {
                        ObjectReference = new []{"objRef"},
                        ObjectType = "objType"
                    }
                }
            };

            var customer = new CustomerBuilder()
                .WithSurname("surname")
                .WithForename("forename")
                .WithEmail("forename.surname@gmail.com")
                .Build();

            _mockClient
                .Setup(_ => _.retrieveIndividualAsync(individual.BriefDetails.ObjectID))
                .ReturnsAsync(new retrieveIndividualResponse
                {
                    FWTIndividual = individual
                });

            // Act
            await _service.UpdateIndividual(individual, customer);

            // Assert
            _mockConnection.Verify(_ => _.Client().updateIndividualAsync(It.IsAny<FWTIndividualUpdate>()), Times.Once);
        }

        [Fact]
        public async Task UpdateIndividual_ShouldNotCallVerintConnection_UpdateIndividualAsync_IfUpdateIsNotRequired()
        {
            var individual = new FWTIndividual
            {
                BriefDetails = new FWTObjectBriefDetails
                {
                    ObjectID = new FWTObjectID
                    {
                        ObjectReference = new[] { "objRef" },
                        ObjectType = "objType"
                    }
                }
            };
            
            _mockClient
                .Setup(_ => _.retrieveIndividualAsync(individual.BriefDetails.ObjectID))
                .ReturnsAsync(new retrieveIndividualResponse
                {
                    FWTIndividual = individual
                });

            // Act
            await _service.UpdateIndividual(individual, new Customer());

            // Assert
            _mockConnection.Verify(_ => _.Client().updateIndividualAsync(It.IsAny<FWTIndividualUpdate>()), Times.Never);
        }
    }
}