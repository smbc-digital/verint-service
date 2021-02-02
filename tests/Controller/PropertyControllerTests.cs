using Moq;
using System.Threading.Tasks;
using verint_service.Controllers;
using Xunit;
using verint_service.Services.Property;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace verint_service_tests.Controller
{
    public class PropertyControllerTests
    {
        private Mock<IPropertyService> _mockService = new Mock<IPropertyService>();
        private PropertyController _controller;
        public PropertyControllerTests()
        {
            _controller = new PropertyController(_mockService.Object);
        }

        [Fact]
        public async Task GetListPropertises_ShouldCallPropertyService()
        {
            _mockService
                .Setup(_ => _.GetPropertiesAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<StockportGovUK.NetStandard.Models.Verint.Address>());

            await _controller.GetProperties("test-postcode");

            _mockService
                .Verify(_ => _.GetPropertiesAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetAddressPropertyByUprn_ShouldCallPropertyService()
        {
            _mockService
                .Setup(_ => _.GetPropertyByUprnAsync(It.IsAny<string>()))
                .ReturnsAsync(new StockportGovUK.NetStandard.Models.Verint.Address());

            await _controller.GetPropertiesByUPRN("test-postcode");

            _mockService
                .Verify(_ => _.GetPropertyByUprnAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetAddressPropertyByUprn_ShouldCallPropertyService_InvalidUprn_ReturnBadRequest()
        {
            _mockService
                .Setup(_ => _.GetPropertyByUprnAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<StockportGovUK.NetStandard.Models.Verint.Address>(null));

            var response = await _controller.GetPropertiesByUPRN("test-postcode");

            Assert.IsType<BadRequestResult>(response);
        }
    }
}
