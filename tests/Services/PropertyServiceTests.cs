using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Property;
using verint_service.Services.Street;
using verint_service.Utils.Consts;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Services
{
    public class PropertyServiceTests
    {
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly Mock<ILogger<PropertyService>> _mockLogger = new Mock<ILogger<PropertyService>>();
        private readonly Mock<IVerintClient> _mockClient = new Mock<IVerintClient>();
        private readonly PropertyService _service;

        public PropertyServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);

            _service = new PropertyService(_mockConnection.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SearchByPostCode_GetPropertiesAsync_ShouldReturnListOfAddress()
        {
            var propertySearchResults = new searchForPropertyResponse()
            {
                FWTObjectBriefDetailsList = new List<FWTObjectBriefDetails>
                {
                    new FWTObjectBriefDetails
                    {
                        ObjectID = new FWTObjectID { ObjectReference = new string[] { "test"} },
                        ObjectDescription = "test"
                    }
                }.ToArray()
            };
           
            _mockConnection
                .Setup(_ => _.Client().searchForPropertyAsync(It.Is<FWTPropertySearch>(_ => _.Postcode.Equals("sk2 5tl"))))
                .ReturnsAsync(propertySearchResults);

            _mockConnection
                .Setup(_ => _.Client().retrievePropertyAsync(It.IsAny<FWTObjectID>()))
                .ReturnsAsync(new retrievePropertyResponse() { FWTProperty = new FWTProperty { UPRN = "Test" } });

            var result = await _service.GetPropertiesAsync("sk2 5tl");

            // Assert
            Assert.IsType<List<StockportGovUK.NetStandard.Models.Verint.Address>>(result);
        }

        [Fact]
        public async Task SearchByPostCode_GetPropertiesAsync_ShouldReturnEmptyList()
        {
            var propertySearchResults = new searchForPropertyResponse()
            {
                FWTObjectBriefDetailsList = new List<FWTObjectBriefDetails>().ToArray()
            };

            _mockConnection
                .Setup(_ => _.Client().searchForPropertyAsync(It.Is<FWTPropertySearch>(_ => _.Postcode.Equals("sk2 5tl"))))
                .ReturnsAsync(propertySearchResults);

            var result = await _service.GetPropertiesAsync("sk2 5tl");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchByUprn_GetPropertyAsync_ShouldReturnAddress()
        {
            var propertySearchResults = new searchForPropertyResponse()
            {
                FWTObjectBriefDetailsList = new List<FWTObjectBriefDetails>
                {
                    new FWTObjectBriefDetails
                    {
                        ObjectID = new FWTObjectID { ObjectReference = new string[] { "test"} },
                        ObjectDescription = "test"
                    }
                }.ToArray()
            };

            _mockConnection
                .Setup(_ => _.Client().searchForPropertyAsync(It.Is<FWTPropertySearch>(_ => _.UPRN.Equals("100011489689"))))
                .ReturnsAsync(propertySearchResults);

            _mockConnection
               .Setup(_ => _.Client().retrievePropertyAsync(It.IsAny<FWTObjectID>()))
               .ReturnsAsync(new retrievePropertyResponse() { FWTProperty = new FWTProperty { UPRN = "Test", AddressLine1 = "Address 1" } });

            var result = await _service.GetPropertyByUprnAsync("100011489689");

            // Assert
            Assert.IsType<StockportGovUK.NetStandard.Models.Verint.Address>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SearchByUprn_GetPropertyAsync_ShouldReturnNull()
        {
            _mockConnection
                .Setup(_ => _.Client().searchForPropertyAsync(It.Is<FWTPropertySearch>(_ => _.UPRN.Equals("100011489689"))))
                .Returns(Task.FromResult<searchForPropertyResponse>(null));

            var result = await _service.GetPropertyByUprnAsync("100011489689");

            // Assert
            Assert.Null(result);
        }
    }
}
