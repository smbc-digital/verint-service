using System.Threading.Tasks;
using Moq;
using verint_service.Helpers.VerintConnection;
using verint_service.Services;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Services
{
    public class IndividualServiceTests
    {
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly Mock<IVerintClient> _mockClient = new Mock<IVerintClient>();
        private readonly IndividualService _service;

        public IndividualServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);
            _service = new IndividualService(_mockConnection.Object);
        }

        [Fact]
        public void FirstTest()
        {
            // // Arrange
            // var objectId = new FWTObjectID();

            // _mockClient
            //     .Setup(client => client.createInteractionAsync(It.IsAny<FWTInteractionCreate>()))
            //     .ReturnsAsync(new createInteractionResponse{
            //         InteractionID = 987654321
            //     });

            // // Act
            // var result =  await _service.CreateInteractionForIndividual(objectId);

            // // Assert
            // _mockClient.Verify(client => client.createInteractionAsync(It.IsAny<FWTInteractionCreate>()), Times.Once);
            // Assert.IsType<long>(result);
        }
    }
}