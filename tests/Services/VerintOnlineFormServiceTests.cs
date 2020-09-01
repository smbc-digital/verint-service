using Moq;
using StockportGovUK.NetStandard.Models.Models.Verint.VerintOnlineForm;
using StockportGovUK.NetStandard.Models.Verint;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Case;
using verint_service.Services.VerintOnlineForm;
using VOFWebService;
using Xunit;

namespace verint_service_tests.Services
{
    public class VerintOnlineFormServiceTests
    {
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly Mock<IVOFClient> _mockVOFClient = new Mock<IVOFClient>();
        private readonly VerintOnlineFormService _verintOnlineFormService;
        private readonly Mock<ICaseService> _mockCaseService = new Mock<ICaseService>();

        private readonly VerintOnlineFormRequest model = new VerintOnlineFormRequest
        {
            VerintCase = new Case { CaseReference = "reference" },
            FormName = "test form",
            FormData = new Dictionary<string, string>
                {
                    {"key","value"}
                }
        };

        public VerintOnlineFormServiceTests()
        {
            _mockConnection
               .Setup(_ => _.VOFClient())
               .Returns(_mockVOFClient.Object);

            _mockCaseService
                .Setup(_ => _.Create(It.IsAny<Case>()))
                .ReturnsAsync(model.VerintCase.CaseReference);

            _verintOnlineFormService = new VerintOnlineFormService(_mockConnection.Object, _mockCaseService.Object);
        }

        [Fact]
        public async Task CreateVOFCase_ShouldCallCaseService()
        {
            // Arrange 
            _mockVOFClient
                .Setup(_ => _.CreateAsync(It.IsAny<CreateRequest>()))
                .ReturnsAsync(new CreateResponse1
                {
                    CreateResponse = new CreateResponse { @ref = "123456" }
                });

            _mockVOFClient
                .Setup(_ => _.UpdateAsync(It.IsAny<UpdateRequest>()))
                .ReturnsAsync(new UpdateResponse1 { UpdateResponse = new UpdateResponse { status = "success" } });

            // Act
            await _verintOnlineFormService.CreateVOFCase(model);

            // Assert
            _mockCaseService.Verify(_ => _.Create(It.IsAny<Case>()), Times.Once);
        }

        [Fact]
        public async Task CreateVOFCase_ShouldCallVOFConnectionToCreateRequest()
        {
            // Arrange
            _mockVOFClient
                .Setup(_ => _.CreateAsync(It.IsAny<CreateRequest>()))
                .ReturnsAsync(new CreateResponse1
                {
                    CreateResponse = new CreateResponse { @ref = "123456" }
                });

            _mockVOFClient
                .Setup(_ => _.UpdateAsync(It.IsAny<UpdateRequest>()))
                .ReturnsAsync(new UpdateResponse1 { UpdateResponse = new UpdateResponse { status = "success" } });

            // Act
            await _verintOnlineFormService.CreateVOFCase(model);

            // Assert
            _mockVOFClient.Verify(_ => _.CreateAsync(It.IsAny<CreateRequest>()), Times.Once);
        }

        [Fact]
        public async Task CreateVOFCase_ShouldThrowExceptionWhenRefIsNull()
        {
            // Arrange
            _mockVOFClient
                .Setup(_ => _.CreateAsync(It.IsAny<CreateRequest>()))
                .ReturnsAsync(new CreateResponse1
                {
                    CreateResponse = new CreateResponse()
                });

            _mockVOFClient
                .Setup(_ => _.UpdateAsync(It.IsAny<UpdateRequest>()))
                .ReturnsAsync(new UpdateResponse1 { UpdateResponse = new UpdateResponse { status = "success" } });

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => _verintOnlineFormService.CreateVOFCase(model));

            // Assert
            Assert.Equal("VerintOnlineFormService.CreateVOFCase: VerintOnlineForms-WebService.CreateAsync failed to create basic case.", result.Message);
        }

        [Fact]
        public async Task CreateVOFCase_ShouldThrowExceptionWhenStatusIsNotSuccess()
        {
            // Arrange
            _mockVOFClient
                .Setup(_ => _.CreateAsync(It.IsAny<CreateRequest>()))
                .ReturnsAsync(new CreateResponse1
                {
                    CreateResponse = new CreateResponse { @ref = "123456" }
                });

            _mockVOFClient
                .Setup(_ => _.UpdateAsync(It.IsAny<UpdateRequest>()))
                .ReturnsAsync(new UpdateResponse1 { UpdateResponse = new UpdateResponse { status = "not success" } });

            // Act
            var result = await Assert.ThrowsAsync<Exception>(() => _verintOnlineFormService.CreateVOFCase(model));

            // Assert
            Assert.Equal("VerintOnlineFormService.CreateVOFCase: VerintOnlineForms-WebService.UpdateAsync failed to update case details.", result.Message);
        }

        [Fact]
        public async Task CreateVOFCase_ShouldCallVOFConnectionToUpdateRequest()
        {
            // Arrange
            _mockVOFClient
                .Setup(_ => _.CreateAsync(It.IsAny<CreateRequest>()))
                .ReturnsAsync(new CreateResponse1
                {
                    CreateResponse = new CreateResponse { @ref = "123456" }
                });

            _mockVOFClient
                .Setup(_ => _.UpdateAsync(It.IsAny<UpdateRequest>()))
                .ReturnsAsync(new UpdateResponse1 { UpdateResponse = new UpdateResponse { status = "success" } });

            // Act
            await _verintOnlineFormService.CreateVOFCase(model);

            // Assert
            _mockVOFClient.Verify(_ => _.UpdateAsync(It.IsAny<UpdateRequest>()), Times.Once);
        }

        [Fact]
        public async Task CreateVOFCase_ShouldReturnVerintOnlineFormResponse()
        {
            // Arrange
            _mockVOFClient
                .Setup(_ => _.CreateAsync(It.IsAny<CreateRequest>()))
                .ReturnsAsync(new CreateResponse1
                {
                    CreateResponse = new CreateResponse { @ref = "123456" }
                });

            _mockVOFClient
                .Setup(_ => _.UpdateAsync(It.IsAny<UpdateRequest>()))
                .ReturnsAsync(new UpdateResponse1 { UpdateResponse = new UpdateResponse { status = "success" } });

            // Act
           var result = await _verintOnlineFormService.CreateVOFCase(model);

            // Assert
            Assert.Equal("reference", result.VerintCaseReference);
            Assert.Equal("123456", result.VerintOnlineFormReference);
        }

        [Fact]
        public async Task GetVOFCase_ShouldCallVOFConnectionToGetCase()
        {
            _mockVOFClient
                .Setup(_ => _.GetAsync(It.IsAny<GetRequest>()))
                .ReturnsAsync(new GetResponse1());

            await _verintOnlineFormService.GetVOFCase("test-ref");

            _mockVOFClient
                .Verify(_ => _.GetAsync(It.IsAny<GetRequest>()), Times.Once);

            _mockVOFClient
                .VerifyNoOtherCalls();

            _mockCaseService
                .VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetVOFCase_ShouldCallVOFConnectionWithCorrectReference()
        {
            var expectedRef = "test-ref";

            _mockVOFClient
                .Setup(_ => _.GetAsync(new GetRequest
                {
                    @ref = expectedRef
                }))
                .ReturnsAsync(new GetResponse1());

            await _verintOnlineFormService.GetVOFCase(expectedRef);

            _mockVOFClient
                .Verify(_ => _.GetAsync(It.Is<GetRequest>(x => x.@ref.Equals(expectedRef))), Times.Once);

            _mockVOFClient
                .VerifyNoOtherCalls();

            _mockCaseService
                .VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetVOFCase_ShouldReturnCase()
        {
            var expectedRef = "test-ref";

            _mockVOFClient
                .Setup(_ => _.GetAsync(It.IsAny<GetRequest>()))
                .ReturnsAsync(new GetResponse1());

            var result = await _verintOnlineFormService.GetVOFCase(expectedRef);

            Assert.NotNull(result);
            Assert.IsType<GetResponse1>(result);
        }
    };
}

