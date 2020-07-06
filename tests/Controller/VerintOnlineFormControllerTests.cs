using Moq;
using System.Threading.Tasks;
using verint_service.Controllers;
using verint_service.Services.VerintOnlineForm;
using Xunit;

namespace verint_service_tests.Controller
{
    public class VerintOnlineFormControllerTests
    {
        private Mock<IVerintOnlineFormService> _mockService = new Mock<IVerintOnlineFormService>();
        private VerintOnlineFormController _controller;
        public VerintOnlineFormControllerTests()
        {
            _controller = new VerintOnlineFormController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ShouldCallVerintOnlineFormService()
        {
            _ = await _controller.Create(new VerintOnlineFormController.VerintOnlineFormRequest());

            _mockService
                .Verify(_ => _.CreateVOFCase(It.IsAny<VerintOnlineFormController.VerintOnlineFormRequest>()), Times.Once);

            _mockService
                .VerifyNoOtherCalls();
        }
    }
}
