using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using StockportGovUK.AspNetCore.Gateways.InthubGateway;
using verint_service.Models;
using verint_service.Services.Event;
using Xunit;

namespace verint_service_tests.Services
{
    public class EventServiceTests
    {
        private readonly Mock<IInthubGateway> _mockIntGateway = new Mock<IInthubGateway>();
        private readonly IEventService _eventService;


        public EventServiceTests()
        {
            _eventService = new EventService(_mockIntGateway.Object);
        }

        [Fact]
        public void HandleCaseEvent_ShouldCallEventGateway()
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
            _mockIntGateway.Verify(_ => _.UnmatchFosteringCase(model.EventCase.Id), Times.Once);
        }

        [Fact]
        public void HandleCaseEvent_ShouldNotCallEventGateway()
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
            _mockIntGateway.VerifyNoOtherCalls();
        }

        [Fact]
        public void HandleCaseEvent_ShouldDoNothing()
        {
            // Act & Arrange
            _eventService.HandleCaseEvent(null);

            // Assert
            _mockIntGateway.VerifyNoOtherCalls();
        }
    }
}

