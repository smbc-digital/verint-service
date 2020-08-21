using System.Threading.Tasks;
using Moq;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Street;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Services
{
    public class StreetServiceTests
    {
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly Mock<IVerintClient> _mockClient = new Mock<IVerintClient>();
        private readonly StreetService _service;
        private const string USRN = "12345";
        private const string Name = "Name";
        private const string StreetName = "StreetName";
        private const string PrimaryLocality = "PrimaryLocality";
        private const string PostTownName = "PostTownName";

        public StreetServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);

            _mockClient
                .Setup(_ => _.retrieveStreetAsync(It.IsAny<FWTObjectID>()))
                .ReturnsAsync(new retrieveStreetResponse {  FWTStreet = new FWTStreet {
                    USRN = USRN,
                    BriefDetails = new FWTObjectBriefDetails { ObjectDescription = Name },
                    StreetName = StreetName,
                    PrimaryLocality = PrimaryLocality,
                    PostTownName = PostTownName
                }});

            _service = new StreetService(_mockConnection.Object);
        }

        [Fact]
        public async Task GetStreet_ShouldCall_retrieveStreetAsync()
        {
            await _service.GetStreet(Name);

            _mockClient.Verify(client => client.retrieveStreetAsync(It.IsAny<FWTObjectID>()), Times.Once);
        }

        [Fact]
        public async Task GetStreet_ShouldReturnStreet()
        {
            var result = await _service.GetStreet(Name);

            Assert.NotNull(result);
            Assert.Equal(USRN, result.USRN);
            Assert.Equal(Name, result.Name);
            Assert.Equal(StreetName, result.AddressLine1);
            Assert.Equal(PrimaryLocality, result.AddressLine2);
            Assert.Equal(PostTownName, result.AddressLine3);
        }

        [Fact]
        public async Task GetStreet_GivenBreifDetailsIsNull_ShouldReturnConcatName()
        {
            _mockClient
                .Setup(_ => _.retrieveStreetAsync(It.IsAny<FWTObjectID>()))
                .ReturnsAsync(new retrieveStreetResponse {  FWTStreet = new FWTStreet {
                    USRN = USRN,
                    BriefDetails = null,
                    StreetName = StreetName,
                    PrimaryLocality = PrimaryLocality,
                    PostTownName = PostTownName
                }});

            var result = await _service.GetStreet(Name);

            Assert.NotNull(result);
            Assert.Equal(USRN, result.USRN);
            Assert.Equal($"{StreetName}, {PrimaryLocality}, {PostTownName}, ", result.Name);
            Assert.Equal(StreetName, result.AddressLine1);
            Assert.Equal(PrimaryLocality, result.AddressLine2);
            Assert.Equal(PostTownName, result.AddressLine3);
        }

        [Fact]
        public async Task SearchByStreetAsync_ShouldCall_searchForStreetAsync()
        {
            _mockClient
                .Setup(_ => _.searchForStreetAsync(It.IsAny<FWTStreetSearch>()))
                .ReturnsAsync(new searchForStreetResponse {  FWTObjectBriefDetailsList = new FWTObjectBriefDetails[] {} });

            var result = await _service.SearchByStreetAsync(Name);

            _mockClient.Verify(client => client.searchForStreetAsync(It.IsAny<FWTStreetSearch>()), Times.Once);
        }

        [Fact]
        public async Task SearchByUsrnAsync_ShouldCall_searchForStreetAsync()
        {
            _mockClient
                .Setup(_ => _.searchForStreetAsync(It.IsAny<FWTStreetSearch>()))
                .ReturnsAsync(new searchForStreetResponse {  FWTObjectBriefDetailsList = new FWTObjectBriefDetails[] {} });

            var result = await _service.SearchByUsrnAsync(USRN);

            _mockClient.Verify(client => client.searchForStreetAsync(It.IsAny<FWTStreetSearch>()), Times.Once);
        }
    }
}
