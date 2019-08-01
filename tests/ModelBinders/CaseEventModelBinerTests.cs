using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        public CaseEventModelBinderTests()
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext
                .Setup(_ => _.Request)
                .Returns(_mockHttpRequest.Object);

            _mockModelBindingContext
                .SetupGet(_ => _.HttpContext)
                .Returns(mockHttpContext.Object);
        }


        [Fact]
        public async Task BindModelAsync_ShouldThrowUnableToParseRequestBodyException()
        {
            // Arrange
            _mockHttpRequest
                .SetupGet(_ => _.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _modelBinder.BindModelAsync(_mockModelBindingContext.Object));
            Assert.Equal("Unable to parse request body", ex.Message);
        }

        [Fact]
        public async Task BindModelAsync_ShouldThrowEventTypeNotConfiguredException()
        {
            // Arrange
            _mockHttpRequest
                .SetupGet(_ => _.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("<Test></Test>")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _modelBinder.BindModelAsync(_mockModelBindingContext.Object));
            Assert.Equal("EventType not configured", ex.Message);
        }

        [Fact]
        public async Task BindModelAsync_ShouldThrowUnableToSerializeException()
        {
            // Arrange
            var eventType = EventCaseType.PopulatedCloseCaseEvent.ToString();
            _mockHttpRequest
                .SetupGet(_ => _.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes($"<{eventType}></{eventType}>")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _modelBinder.BindModelAsync(_mockModelBindingContext.Object));
            Assert.Equal("Unable to serialize case from xml response", ex.Message);
        }

        [Fact]
        public async Task BindModelAsync_ShouldThrowUnableToParseToEventCaseException()
        {
            // Arrange
            var eventType = EventCaseType.PopulatedCloseCaseEvent.ToString();
            _mockHttpRequest
                .SetupGet(_ => _.Body)
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes($"<{eventType}><case></case></{eventType}>")));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _modelBinder.BindModelAsync(_mockModelBindingContext.Object));
            Assert.Equal("Unable to parse serialized case into EventCase", ex.Message);
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
