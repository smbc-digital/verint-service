using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Moq;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Individual.Weighting;
using verint_service.Services;
using verint_service.Services.Property;
using VerintWebService;
using Xunit;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service_tests.Builders;
using System.Threading.Tasks;

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
        public async Task ResolveAsync_ShouldCallVerint_SearchPartyAsync_Once_WhenMatchingUserFound_OnInitalSearch()
        {
            //Setup
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
            
            var result = await _service.ResolveAsync(customer);

            Assert.NotNull(result);
            _mockClient.Verify(_ => _.searchForPartyAsync(It.Is<FWTPartySearch>(x => x.SearchType == "individual" && x.Forename == "forename" && x.Name == "surname")), Times.Once);
            _mockClient.Verify(_ => _.retrieveIndividualAsync(It.IsAny<FWTObjectID>()), Times.Exactly(2));
        }
        
        [Fact]
        public async Task ResolveAsync_ShouldCallVerint_SearchPartyAsync_WithEmailSearchCrtieria_WhenMatchingUserNotFound_OnInitalSearch()
        {
            //Setup
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
            
            var result = await _service.ResolveAsync(customer);

            Assert.NotNull(result);
            _mockClient.Verify(_ => _.searchForPartyAsync(It.Is<FWTPartySearch>(x => x.SearchType == "individual" && x.EmailAddress == "email@test.com")), Times.Exactly(2));
            _mockClient.Verify(_ => _.retrieveIndividualAsync(It.IsAny<FWTObjectID>()), Times.Exactly(3));
        }

        [Fact]
        public void ResolveAsync_ShouldCreateCustomer_WhenNonFoundAfterAll_SearchPartyAsyncCalls_Performed()
        {

        }
    }
}
