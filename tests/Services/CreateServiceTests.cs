using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using verint_service.Helpers.VerintConnection;
using verint_service.Models;
using verint_service.Services.Case;
using verint_service.Services.Create;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Services
{

    public class CreateServiceTests
    {
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly Mock<IVerintClient> _mockClient = new Mock<IVerintClient>();
        private readonly CreateService _service;

        public CreateServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);
            _service = new CreateService(_mockConnection.Object);
        }

        [Fact]
        public async Task CreateCase_ShouldCall_Verint_createCaseDetailsAsync()
        {
            // Arrange
            var caseDetails = new Case
            {
                EventCode = 1234567,
                EventTitle = "test title",
                Description = "test description"
            };

            _mockClient
                .Setup(client => client.createCaseAsync(It.IsAny<FWTCaseCreate>()))
                .ReturnsAsync(It.IsAny<createCaseResponse>());

            // Act
            await _service.CreateCase(caseDetails);

            // Assert
            _mockClient.Verify(client => client.createCaseAsync(It.IsAny<FWTCaseCreate>()), Times.Once);
        }

        [Fact]
        public async Task CreateCase_ShouldThrowError()
        {
            // Arrange
            var caseDetails = new Case
            {
                EventCode = 1234567,
                EventTitle = "test title",
                Description = "test description"
            };

            _mockClient
                .Setup(client => client.createCaseAsync(It.IsAny<FWTCaseCreate>()))
                .Throws(new Exception());

            // Act
            await Assert.ThrowsAsync<Exception>(() => _service.CreateCase(caseDetails));

            // Assert
            _mockClient.Verify(_ => _.createCaseAsync(It.IsAny<FWTCaseCreate>()), Times.Once);
        }
    }
}
