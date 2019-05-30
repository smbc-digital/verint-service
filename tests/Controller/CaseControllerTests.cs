using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using verint_service.Controllers;
using Moq;
using verint_service.Models;
using verint_service.Services;
using Xunit;

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
        public void Get_ShouldCallCaseService()
        {
            //Arrange
            var caseId = "test case";

            //Act
            _caseController.Get(caseId);

            //Assert
            _mockCaseService.Verify(service => service.GetCase(caseId), Times.Once);
        }

        [Fact]
        public void Get_ShouldReturnOkObjectResult()
        {
            //Arrange
            var caseId = "test case";

            //Act
            var response = _caseController.Get(caseId);

            //Assert
            Assert.IsAssignableFrom<OkObjectResult>(response);
        }
    }
}
