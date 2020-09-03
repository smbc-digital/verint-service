using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Moq;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Individual.Weighting;
using verint_service.Services;
using verint_service.Services.Property;
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

        private readonly IndividualService _service;

        public IndividualServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);
                
            _service = new IndividualService(_mockConnection.Object, new List<IIndividualWeighting>(), _mockPropertyService.Object, _mockLogger.Object);
        }

        [Fact]
        public void ResolveAsync_ShouldCallVerint_SearchPartyAsync_Once_WhenMatchingUserFound_OnInitalSearch()
        {

        }

        
        [Fact]
        public void ResolveAsync_ShouldCallVerint_SearchPartyAsync_WithEmailSearchCrtieria_WhenMatchingUserNotFound_OnInitalSearch()
        {

        }

        [Fact]
        public void ResolveAsync_ShouldCallVerint_SearchPartyAsync_WithAllPossibleSearchCrtieria_WhenMatchingUserNotFound_OnAnySearch()
        {

        }
    }
}
