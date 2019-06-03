using Microsoft.AspNetCore.Mvc;
using verint_service.Controllers;
using Moq;
using verint_service.Services;
using Xunit;
using System.Threading.Tasks;

namespace verint_service_tests.Controllers
{
    public class CaseControllerTests
    {
        private readonly CaseController _caseController;
        private readonly Mock<ICaseService> _mockCaseService = new Mock<ICaseService>();

        public CaseControllerTests()
        {
            _caseController = new CaseController(_mockCaseService.Object);
        }

        [Fact]
        public async Task Get_ShouldCallCaseService()
        {
            //Arrange
            var caseId = "test case";

            //Act
            await _caseController.Get(caseId);

            //Assert
            _mockCaseService.Verify(service => service.GetCase(caseId), Times.Once);
        }

        [Fact]
        public async Task Get_ShouldReturnOkObjectResult()
        {
            //Arrange
            var caseId = "test case";

            //Act
            var response = await _caseController.Get(caseId);

            //Assert
            Assert.IsAssignableFrom<OkObjectResult>(response);
        }
    }
}
