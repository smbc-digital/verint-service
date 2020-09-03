using System;
using Microsoft.AspNetCore.Mvc;
using verint_service.Controllers;
using Moq;
using verint_service.Services.Case;
using verint_service.Services.Update;
using Xunit;
using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Verint.Update;
using VerintWebService;
using Microsoft.Extensions.Logging;
using verint_service.Services.Event;
using StockportGovUK.NetStandard.Models.Verint;
using System.Collections.Generic;
using verint_service.Models.CaseEvent;

namespace verint_service_tests.Controllers
{
    public class CaseControllerTests
    {
        private readonly CaseController _caseController;
        private readonly Mock<ICaseService> _mockCaseService = new Mock<ICaseService>();
        private readonly Mock<IUpdateService> _mockUpdateService = new Mock<IUpdateService>();
        private readonly Mock<ILogger<CaseController>> _mockLogger = new Mock<ILogger<CaseController>>();
        private readonly Mock<IEventService> _mockEventService = new Mock<IEventService> ();


        public CaseControllerTests()
        {
            _caseController = new CaseController(_mockCaseService.Object,_mockUpdateService.Object, _mockLogger.Object, _mockEventService.Object);
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
        public async Task Create_ShouldCallCreateService()
        {
            // Arrange
            var caseDetails = new Case
            {
                EventCode = 1234567,
                EventTitle = "test title",
                Description = "test description"
            };

            // Act
            await _caseController.Create(caseDetails);

            // Assert
            _mockCaseService.Verify(service => service.Create(caseDetails), Times.Once);
        }

        [Fact]
        public async Task UpdateIntegrationFormField_shouldCallUpdateService()
        {
            // Arrange
            var updateEntity = new IntegrationFormFieldsUpdateModel
            {
                CaseReference = "121212",
                IntegrationFormName = "Form_Name"
            };

            _mockUpdateService.Setup(_ => _.UpdateIntegrationFormFields(It.IsAny<IntegrationFormFieldsUpdateModel>()))
                .ReturnsAsync(new writeCaseEformDataResponse(0));

            // Act
            var result = await _caseController.UpdateIntegrationFormFields(updateEntity);


            // Assert
            _mockUpdateService.Verify(_ => _.UpdateIntegrationFormFields(It.IsAny<IntegrationFormFieldsUpdateModel>()), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateIntegrationFormFields_ShouldReturn_InternalServerErrorOnException()
        {
            // Arrange
            var updateEntity = new IntegrationFormFieldsUpdateModel
            {
                CaseReference = "121212",
                IntegrationFormName = "Form_Name"
            };

            _mockUpdateService.Setup(_ => _.UpdateIntegrationFormFields(It.IsAny<IntegrationFormFieldsUpdateModel>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _caseController.UpdateIntegrationFormFields(updateEntity);


            // Assert
            _mockUpdateService.Verify(_ => _.UpdateIntegrationFormFields(It.IsAny<IntegrationFormFieldsUpdateModel>()), Times.Once);
            Assert.IsType<StatusCodeResult>(result);
        }

        [Fact]
        public void CaseEventHandler_ShouldCall_HandleCaseEvent()
        {
            // Arrange
            var model = new CaseEventModel();

            // Act
            _caseController.CaseEventHandler(model);

            // Assert
            _mockEventService.Verify(_ => _.HandleCaseEvent(It.IsAny<CaseEventModel>()));
        }

        [Fact]
        public async Task UpdateCaseDescription_HappyPath()
        {
            var model = new Case
            {
                CaseReference = "1234",
                Description = "another test"
            };

            await _caseController.UpdateCaseDescription(model);

            _mockCaseService.Verify(service => service.UpdateDescription(model), Times.Once);
        }

        [Fact]
        public void UpdateCaseDescription_ExceptionOccurs()
        {
            var model = new Case
            {
                CaseReference = "1234",
                Description = "another test"
            };
            _mockCaseService.Setup(service => service.UpdateDescription(model))
                .ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _caseController.UpdateCaseDescription(model));
        }

        [Fact]
        public async Task AddNoteWithAttachments_ShouldReturnOk_OnSuccessfulServiceCall()
        {
            var model = new NoteWithAttachments
            {
                Attachments = new List<StockportGovUK.NetStandard.Models.FileManagement.File>(),
                AttachmentsDescription = "description",
                CaseRef = 123456789123
            };

            var result = await _caseController.AddNoteWithAttachments(model);

            _mockCaseService.Verify(service => service.CreateNotesWithAttachment(It.IsAny<NoteWithAttachments>()), Times.Once);
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task AddNoteWithAttachments_ShouldReturn500StatusCode_WhenServiceThrowsException()
        {
            var model = new NoteWithAttachments
            {
                Attachments = new List<StockportGovUK.NetStandard.Models.FileManagement.File>(),
                AttachmentsDescription = "description",
                CaseRef = 123456789123
            };

            _mockCaseService.Setup(service => service.CreateNotesWithAttachment(It.IsAny<NoteWithAttachments>()))
                .ThrowsAsync(new Exception());

            var result = await _caseController.AddNoteWithAttachments(model);

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
