using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using verint_service.ModelBinders;
using verint_service.Models;
using Xunit;

namespace verint_service_tests.ModelBinders
{
    public class CaseEventModelBinderTests
    {
        private readonly CaseEventModelBinder _modelBinder = new CaseEventModelBinder();
        private readonly Mock<ModelBindingContext> _mockModelBindingContext = new Mock<ModelBindingContext>();
        private readonly Mock<HttpRequest> _mockHttpRequest = new Mock<HttpRequest>();
        private readonly Mock<ILogger<CaseEventModelBinder>> _mockLogger = new Mock<ILogger<CaseEventModelBinder>>();

        public CaseEventModelBinderTests()
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext
                .Setup(_ => _.Request)
                .Returns(_mockHttpRequest.Object);
            var mockServiceProvider = new Mock<IServiceProvider>();

            mockServiceProvider
                    .Setup(_ => _.GetService(typeof(ILogger<CaseEventModelBinder>)))
                    .Returns(_mockLogger.Object);

            mockHttpContext
                .Setup(_ => _.RequestServices)
                .Returns(mockServiceProvider.Object);

            _mockModelBindingContext
                .SetupGet(_ => _.HttpContext)
                .Returns(mockHttpContext.Object);
        }


        [Fact]
        public void BindModelAsync_ShouldThrowUnableToParseRequestBodyException()
        {
            // Arrange
            _mockHttpRequest
                .SetupGet(_ => _.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("")));

            // Act
            var result = _modelBinder.BindModelAsync(_mockModelBindingContext.Object);

            // Assert
            Assert.True(result.IsCompleted);
            _mockModelBindingContext
                .VerifySet(_ => _.Result = ModelBindingResult.Success(null), Times.Once);
        }

        [Fact]
        public void BindModelAsync_ShouldThrowEventTypeNotConfiguredException()
        {
            // Arrange
            _mockHttpRequest
                .SetupGet(_ => _.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("<Test></Test>")));

            // Act
            var result = _modelBinder.BindModelAsync(_mockModelBindingContext.Object);

            // Assert
            Assert.True(result.IsCompleted);
            _mockModelBindingContext
                .VerifySet(_ => _.Result = ModelBindingResult.Success(null), Times.Once);
            }

        [Fact]
        public void BindModelAsync_ShouldThrowUnableToSerializeException()
        {
            // Arrange
            var eventType = EventCaseType.PopulatedCloseCaseEvent.ToString();
            _mockHttpRequest
                .SetupGet(_ => _.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes($"<{eventType}></{eventType}>")));

            // Act
            var result = _modelBinder.BindModelAsync(_mockModelBindingContext.Object);

            // Assert
            Assert.True(result.IsCompleted);
            _mockModelBindingContext
                .VerifySet(_ => _.Result = ModelBindingResult.Success(null), Times.Once);
        }

        [Fact]
        public void BindModelAsync_ShouldThrowUnableToParseToEventCaseException()
        {
            // Arrange
            var eventType = EventCaseType.PopulatedCloseCaseEvent.ToString();
            _mockHttpRequest
                .SetupGet(_ => _.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes($"<{eventType}><case></case></{eventType}>")));

            // Act
            var result = _modelBinder.BindModelAsync(_mockModelBindingContext.Object);

            // Assert
            Assert.True(result.IsCompleted);
            _mockModelBindingContext
                .VerifySet(_ => _.Result = ModelBindingResult.Success(null), Times.Once);
        }

        [Fact]
        public void BindModelAsync_ShouldReturnResultOfCaseEventModel()
        {
            // Arrange
            var eventType = EventCaseType.PopulatedCloseCaseEvent.ToString();

            _mockHttpRequest
                .SetupGet(_ => _.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes($"<case:{eventType}  xmlns:case=\"http://www.kana.com/lagan/schemas/casemanagement\"><case:case></case:case></case:{eventType}>")));

            // Act
            var result = _modelBinder.BindModelAsync(_mockModelBindingContext.Object);

            // Assert
            Assert.True(result.IsCompleted);
        }
    }
}
