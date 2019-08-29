using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using StockportGovUK.NetStandard.Models.Models.Verint;
using verint_service.Config;
using verint_service.HttpClients;
using verint_service.Models;
using verint_service.Services.Event;
using Xunit;

namespace verint_service_tests.Services
{
    public class EventServiceTests
    {
        private readonly IEventService _eventService;

        private readonly Mock<IHttpClientWrapper> _httpClientWrapper = new Mock<IHttpClientWrapper>();
        private readonly Mock<IOptions<EventTypeConfiguration>> _mockEventTypeConfiguration = new Mock<IOptions<EventTypeConfiguration>>();

        public EventServiceTests()
        {
            _mockEventTypeConfiguration
                .Setup(_ => _.Value)
                .Returns(new EventTypeConfiguration
                {
                    PopulatedCloseCaseEvent = new List<CaseEventConfiguration>
                    {
                        new CaseEventConfiguration
                        {
                            Subject = "fostering",
                            Endpoint = "http://testurl.com"
                        }
                    },
                    ReclassifyCaseEvent = new List<CaseEventConfiguration>
                    {
                        new CaseEventConfiguration
                        {
                            Subject = "fostering",
                            Type = "Stage4",
                            AuthToken = "test",
                            Endpoint = "http://testurl.com"
                        }
                    }
                });

            _eventService = new EventService(_mockEventTypeConfiguration.Object, _httpClientWrapper.Object);
        }

        [Fact]
        public void HandleCaseEvent_ShouldCallHttpClientWrapper()
        {
            // Arrange
            var model = new CaseEventModel
            {
                EventCase = new EventCase
                {
                    Id = "test",
                    Classification =new EventClassification
                    {
                        Subject = "fostering"
                    }
                },
                EventType = EventCaseType.PopulatedCloseCaseEvent
            };

            // Act
            _eventService.HandleCaseEvent(model);

            // Assert
            _httpClientWrapper.Verify(_ => _.PostAsync(It.IsAny<string>(),model.EventCase), Times.Once);
        }

        [Fact]
        public void HandleCaseEvent_ShouldNotCallHttpClientWrapper()
        {
            // Arrange
            var model = new CaseEventModel
            {
                EventCase = new EventCase(),
                EventType = EventCaseType.None
            };

            // Act
            _eventService.HandleCaseEvent(model);

            // Assert
            _httpClientWrapper.VerifyNoOtherCalls();
        }

        [Fact]
        public void HandleCaseEvent_ShouldDoNothing()
        {
            // Act & Arrange
            _eventService.HandleCaseEvent(null);

            // Assert
            _httpClientWrapper.VerifyNoOtherCalls();
        }

        [Fact]
        public void HandlePopulatedCloseCaseEvent_ShouldCallGateway()
        {
            // Arrange
            var model = new CaseEventModel
            {
                EventCase = new EventCase
                {
                    Id = "test",
                    Classification = new EventClassification
                    {
                        Subject = "fostering",
                        Type = "Stage4"
                    }
                },
                EventType = EventCaseType.ReclassifyCaseEvent
            };

            // Act
            _eventService.HandleCaseEvent(model);

            // Assert
            _httpClientWrapper.Verify(_ => _.SetHttpClientSecurityHeader("test"), Times.Once);
            _httpClientWrapper.Verify(_ => _.PostAsync(It.IsAny<string>(), model.EventCase), Times.Once);
        }

        [Fact]
        public void HandlePopulatedCloseCaseEvent_ShouldDoNothing()
        {
            // Arrange
            var model = new CaseEventModel
            {
                EventCase = new EventCase
                {
                    Id = "negative test",
                    Classification = new EventClassification
                    {
                        Subject = "negative test",
                        Type = "negative test"
                    }
                },
                EventType = EventCaseType.ReclassifyCaseEvent
            };

            // Act
            _eventService.HandleCaseEvent(model);

            // Assert
            _httpClientWrapper.VerifyNoOtherCalls();
        }

    }
}

