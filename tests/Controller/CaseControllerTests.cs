using System;
using Microsoft.AspNetCore.Mvc;
using verint_service.Controllers;
using Moq;
using verint_service.Models;
using verint_service.Services.Case;
using verint_service.Services.Update;
using Xunit;
using System.Threading.Tasks;
using VerintWebService;

namespace verint_service_tests.Controllers
{
    public class CaseControllerTests
    {
        private readonly CaseController _caseController;
        private readonly Mock<ICaseService> _mockCaseService = new Mock<ICaseService>();
        private readonly  Mock<IUpdateService> _mockUpdateService = new Mock<IUpdateService>();

        public CaseControllerTests()
        {
            _caseController = new CaseController(_mockCaseService.Object,_mockUpdateService.Object);
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


        [Fact]
        public async Task UpdateIntegrationFormField_shouldCallUpdateService()
        {
            // Arrange
            var updateEntity = new IntegrationFormFieldsUpdateEntity
            {
                CaseReference = "121212",
                IntegrationFormName = "Form_Name"
            };

            _mockUpdateService.Setup(_ => _.UpdateIntegrationFormFields(It.IsAny<IntegrationFormFieldsUpdateEntity>()))
                .ReturnsAsync(new writeCaseEformDataResponse(0));

            // Act
            var result = await _caseController.UpdateIntegrationFormFields(updateEntity);


            // Assert
            _mockUpdateService.Verify(_ => _.UpdateIntegrationFormFields(It.IsAny<IntegrationFormFieldsUpdateEntity>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateIntegrationFormFields_ShouldReturn_InternalServerErrorOnException()
        {
            // Arrange
            var updateEntity = new IntegrationFormFieldsUpdateEntity
            {
                CaseReference = "121212",
                IntegrationFormName = "Form_Name"
            };

            _mockUpdateService.Setup(_ => _.UpdateIntegrationFormFields(It.IsAny<IntegrationFormFieldsUpdateEntity>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _caseController.UpdateIntegrationFormFields(updateEntity);


            // Assert
            _mockUpdateService.Verify(_ => _.UpdateIntegrationFormFields(It.IsAny<IntegrationFormFieldsUpdateEntity>()), Times.Once);
            Assert.IsType<StatusCodeResult>(result);
        }
    }
}
