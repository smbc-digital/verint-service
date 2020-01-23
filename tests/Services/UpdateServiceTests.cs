using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using StockportGovUK.NetStandard.Models.Verint;
using StockportGovUK.NetStandard.Models.Verint.Update;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Update;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Services
{
    public class UpdateServiceTests
    {
        private readonly Mock<IVerintClient> _mockClient = new Mock<IVerintClient>();
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly UpdateService _service;

        public UpdateServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);

            _service = new UpdateService(_mockConnection.Object);
        }

        [Fact]
        public async Task UpdateIntegrationFormField_ShouldCallVerintConnection()
        {
            // Arrange
            var entity = new IntegrationFormFieldsUpdateModel();

            // Act
            await _service.UpdateIntegrationFormFields(entity);

            // Assert
            _mockClient.Verify(_ => _.writeCaseEformDataAsync(It.IsAny<FWTCaseEformData>()), Times.Once);
        }


        [Fact]
        public async Task UpdateIntegrationFormField_ShouldReturnError()
        {
            // Arrange
            var entity = new IntegrationFormFieldsUpdateModel();
            _mockClient
                .Setup(_ => _.writeCaseEformDataAsync(It.IsAny<FWTCaseEformData>()))
                .ThrowsAsync(new Exception());

            // Act
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateIntegrationFormFields(entity));

            // Assert
            _mockClient.Verify(_ => _.writeCaseEformDataAsync(It.IsAny<FWTCaseEformData>()), Times.Once);
        }

        [Fact]
        public async Task UpdateIntegrationFormField_ShouldUpdateFields()
        {
            // Arrange
            var writeCasecallback = new FWTCaseEformData();
            var entity = new IntegrationFormFieldsUpdateModel
            {
                CaseReference = "12344",
                IntegrationFormFields = new List<IntegrationFormField>
                {
                    new IntegrationFormField
                    {
                        FormFieldName = "field",
                        FormFieldValue = "updatedValue"
                    },
                    new IntegrationFormField
                    {
                        FormFieldName = "fieldname2",
                        FormFieldValue = "newValue"
                    },
                }
            };

            _mockClient
                .Setup(_ => _.writeCaseEformDataAsync(It.IsAny<FWTCaseEformData>()))
                .ReturnsAsync(new writeCaseEformDataResponse())
                .Callback<FWTCaseEformData>(form => writeCasecallback = form);

            // Act
            await _service.UpdateIntegrationFormFields(entity);

            // Assert
            Assert.Equal(entity.IntegrationFormFields[0].FormFieldValue, writeCasecallback.EformData[0].FieldValue);
            Assert.Equal(entity.IntegrationFormFields[0].FormFieldName, writeCasecallback.EformData[0].FieldName);

            Assert.Equal(entity.IntegrationFormFields[1].FormFieldValue, writeCasecallback.EformData[1].FieldValue);
            Assert.Equal(entity.IntegrationFormFields[1].FormFieldName, writeCasecallback.EformData[1].FieldName);
        }

    }
}
