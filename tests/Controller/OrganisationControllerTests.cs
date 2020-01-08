using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using verint_service.Controllers;
using verint_service.Services.Organisation;
using Xunit;

namespace verint_service_tests.Controllers
{
    public class OrganisationControllerTests
    {
        private readonly OrganisationController _controller;
        private readonly Mock<ILogger<OrganisationController>> _mockLogger = new Mock<ILogger<OrganisationController>>();
        private readonly Mock<IOrganisationService> _mockOrganisationServiceService = new Mock<IOrganisationService>();

        public OrganisationControllerTests()
        {
            _controller = new OrganisationController(_mockLogger.Object, _mockOrganisationServiceService.Object);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_And_CallOrgansationService()
        {
            var result = await _controller.Search("searcthTerm");

            _mockOrganisationServiceService.Verify(_ => _.SearchByOrganisationAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldReturn_InternalServerError_When_ServiceThrowsException()
        {
            _mockOrganisationServiceService.Setup(_ => _.SearchByOrganisationAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

            var result = await _controller.Search("searcthTerm");

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
