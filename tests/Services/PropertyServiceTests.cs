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
            var FWTObjectBriefDetailsList = new List<FWTObjectBriefDetails>
            {
                new FWTObjectBriefDetails
                {
                    ObjectID = new FWTObjectID { ObjectReference = new string[] { "test"} },
                    ObjectDescription = "test"
                }
            };
            
            var propertySearchResults = new searchForPropertyResponse()
            {
                FWTObjectBriefDetailsList = FWTObjectBriefDetailsList.ToArray()
            };

            var fWTObjectID = new FWTObjectID
            {
                ObjectReference = new[] { "test" },
                ObjectType = VerintConstants.PropertyObjectType
            };

            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);

            _mockConnection
                .Setup(_ => _.Client().searchForPropertyAsync(It.Is<FWTPropertySearch>(_ => _.Postcode.Equals("sk2 5tl"))))
                .ReturnsAsync(propertySearchResults);

            _mockConnection
                .Setup(_ => _.Client().retrievePropertyAsync(It.IsAny<FWTObjectID>()))
                .ReturnsAsync(new retrievePropertyResponse() { FWTProperty = new FWTProperty { UPRN = "Test" } });

            _service = new PropertyService(_mockConnection.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SearchByPostCode_GetPropertiesAsync_ShouldReturnListOfAddress()
        {
            var result = await _service.GetPropertiesAsync("sk2 5tl");

            // Assert
            Assert.IsType<List<StockportGovUK.NetStandard.Models.Verint.Address>>(result);
        }

        [Fact]
        public async Task SearchByPostCode_GetPropertiesAsync_ShouldReturnEmptyList()
        {
            var FWTObjectBriefDetailsList = new List<FWTObjectBriefDetails>();

            var propertySearchResults = new searchForPropertyResponse()
            {
                FWTObjectBriefDetailsList = FWTObjectBriefDetailsList.ToArray()
            };

            _mockConnection
                .Setup(_ => _.Client().searchForPropertyAsync(It.Is<FWTPropertySearch>(_ => _.Postcode.Equals("sk2 5tl"))))
                .ReturnsAsync(propertySearchResults);

            var result = await _service.GetPropertiesAsync("sk2 5tl");

            // Assert
            Assert.Empty(result);
        }
    }
}
