using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Helpers.VerintConnection;
using verint_service.Services;
using verint_service.Services.Property;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Services
{

    public class InteractionServiceTests
    {
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly Mock<IVerintClient> _mockClient = new Mock<IVerintClient>();

        private readonly Mock<IPropertyService> _mockPropertyService = new Mock<IPropertyService>();

        private readonly InteractionService _service;

        public InteractionServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);
                
            _service = new InteractionService(_mockConnection.Object, new IndividualService(_mockConnection.Object, new List<IIndividualWeighting>(), _mockPropertyService.Object ));
        }

        [Fact]
        public async Task CreateInteractionForIndividual_ShouldCall_Verint_createInteractionAsync()
        {
            // Arrange

            _mockClient
                .Setup(client => client.createInteractionAsync(It.IsAny<FWTInteractionCreate>()))
                .ReturnsAsync(new createInteractionResponse{
                    InteractionID = 987654321
                });

            var crmCase = new Case();

            // Act
            var result =  await _service.CreateInteraction(crmCase);

            // Assert
            _mockClient.Verify(client => client.createInteractionAsync(It.IsAny<FWTInteractionCreate>()), Times.Once);
            Assert.IsType<long>(result);
        }
    }
}
