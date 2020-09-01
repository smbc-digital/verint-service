using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.NetStandard.Models.Verint;
using System.Collections.Generic;
using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Organisation;
using verint_service.Services.Organisation.Weighting;
using verint_service.Services.Property;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Services
{
    public class OrganisationServiceTests
    {
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly Mock<IVerintClient> _mockClient = new Mock<IVerintClient>();
        private readonly Mock<IPropertyService> _mockPropertyService = new Mock<IPropertyService>();
        private readonly Mock<ILogger<OrganisationService>> _mockLogger = new Mock<ILogger<OrganisationService>>();
        private readonly OrganisationService _service;

        public OrganisationServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);

            _mockClient.Setup(_ => _.searchForPartyAsync(It.IsAny<FWTPartySearch>()))
                .ReturnsAsync(new searchForPartyResponse {  FWTObjectBriefDetailsList = new FWTObjectBriefDetails[0] });

            _mockPropertyService
                .Setup(_ => _.CheckUPRNForId(It.IsAny<Address>()))
                .ReturnsAsync("101002265507");


            _service = new OrganisationService(_mockConnection.Object, new List<IOrganisationWeighting>(), _mockPropertyService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SearchByOrganisationAsync_ShouldCall_searchForPartyAsync()
        {
            await _service.SearchByNameAsync("orgName");

            // Assert
            _mockClient.Verify(client => client.searchForPartyAsync(It.IsAny<FWTPartySearch>()), Times.Once);
        }

        [Fact]
        public async Task SearchByOrganisationAsync_ShouldReturnListOfOrganisations()
        {

            _mockClient.Setup(_ => _.searchForPartyAsync(It.IsAny<FWTPartySearch>()))
                .ReturnsAsync(new searchForPartyResponse { FWTObjectBriefDetailsList = new FWTObjectBriefDetails[1] { new FWTObjectBriefDetails { ObjectDescription = "test Org description", ObjectID = new FWTObjectID { ObjectReference = new string[1] { "010101010" } } } } });

            var results = await _service.SearchByNameAsync("orgName");

            // Assert
            Assert.Single(results);
        }
    }
}
