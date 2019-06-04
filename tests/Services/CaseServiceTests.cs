using Moq;
using System;
using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using verint_service.Services;
using Xunit;

namespace verint_service_tests.Services
{
    public class CaseServiceTests
    {
        private readonly FLWebInterfaceClientTest _client = new FLWebInterfaceClientTest();
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly CaseService _service;

        public CaseServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_client);

            _service = new CaseService(_mockConnection.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void GetCase_ShouldThrowException_WhenCaseIdIsNullOrWhiteSpace(string caseId)
        {
            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => _service.GetCase(caseId).Wait());
            Assert.Equal("Null or empty references are not allowed", ex.InnerException.Message);
        }

        [Fact(Skip = "Skipping to test pipeline pushes to server correctly")]
        public async Task GetCase_ShouldReturnCaseWithOrganisationDetails()
        {
            // Act
            var response = await _service.GetCase("baseCase1");

            // Assert
            Assert.Equal("MockOrganisation", response.Organisation.Name);

        }
    }
}
