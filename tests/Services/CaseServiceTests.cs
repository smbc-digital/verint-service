﻿using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using verint_service.Helpers.VerintConnection;
using verint_service.Models;
using verint_service.Services.Case;
using VerintWebService;
using Xunit;
using verint_service.Services;

namespace verint_service_tests.Services
{
    public class CaseServiceTests
    {
        private readonly Mock<IVerintClient> _mockClient = new Mock<IVerintClient>();
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly Mock<ILogger<CaseService>> _mockLogger = new Mock<ILogger<CaseService>>();
        private readonly CaseService _caseService;

        public CaseServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);

            _caseService = new CaseService(_mockConnection.Object, _mockLogger.Object, new IndividualService(_mockConnection.Object), new InteractionService(_mockConnection.Object));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void GetCase_ShouldThrowException_WhenCaseIdIsNullOrWhiteSpace(string caseId)
        {
            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => _caseService.GetCase(caseId).Wait());
            Assert.Equal("Null or empty references are not allowed", ex.InnerException.Message);
        }

        [Fact]
        public async Task GetCase_ShouldCall_Verint_retrieveCaseDetailsAsync()
        {
            // Arrange
            var caseDetails = new retrieveCaseDetailsResponse
            {
                FWTCaseFullDetails = CreateBaseCase()
            };
            caseDetails.FWTCaseFullDetails.CoreDetails.AssociatedObject = null;

            _mockClient
                .Setup(client => client.retrieveCaseDetailsAsync(It.IsAny<FWTCaseFullDetailsRequest>()))
                .ReturnsAsync(caseDetails);

            // Act
            await _caseService.GetCase("baseCase1");

            // Assert
            _mockClient.Verify(client => client.retrieveCaseDetailsAsync(It.IsAny<FWTCaseFullDetailsRequest>()), Times.Once);
        }

        [Fact]
        public async Task GetCase_ShouldCall_Verint_retrieveOrganisationAsync()
        {
            // Arrange
            var caseDetails = new retrieveCaseDetailsResponse
            {
                FWTCaseFullDetails = CreateBaseCase()
            };

            caseDetails.FWTCaseFullDetails.Interactions = new[]
            {
                new FWTInteractionDetails
                {
                    PartyID = new FWTObjectID
                    {
                        ObjectReference = new []{ "test" },
                        ObjectType = "D1"
                    }
                }
            };

            _mockClient
                .Setup(client => client.retrieveCaseDetailsAsync(It.IsAny<FWTCaseFullDetailsRequest>()))
                .ReturnsAsync(caseDetails);

            // Act
            await _caseService.GetCase("baseCase1");

            // Assert
            _mockClient.Verify(client => client.retrieveOrganisationAsync(It.IsAny<FWTObjectID>()), Times.Once);
        }

        [Fact]
        public async Task GetCase_ShouldReturnMappedOrganisation()
        {
            // Arrange
            var caseDetails = new retrieveCaseDetailsResponse
            {
                FWTCaseFullDetails = CreateBaseCase()
            };

            caseDetails.FWTCaseFullDetails.Interactions = new[]
            {
                new FWTInteractionDetails
                {
                    PartyID = new FWTObjectID
                    {
                        ObjectReference = new []{ "test" },
                        ObjectType = "D1"
                    }
                }
            };

            _mockClient
                .Setup(client => client.retrieveCaseDetailsAsync(It.IsAny<FWTCaseFullDetailsRequest>()))
                .ReturnsAsync(caseDetails);

            _mockClient
                .Setup(client => client.retrieveOrganisationAsync(It.IsAny<FWTObjectID>()))
                .ReturnsAsync(new retrieveOrganisationResponse
                {
                    FWTOrganisation = new FWTOrganisation()
                });


            // Act
            var result = await _caseService.GetCase("baseCase1");

            // Assert
            Assert.NotNull(result.Organisation);
        }

        [Fact]
        public async Task GetCase_ShouldCall_Verint_retrieveIndividualAsync()
        {
            // Arrange
            var caseDetails = new retrieveCaseDetailsResponse
            {
                FWTCaseFullDetails = CreateBaseCase()
            };

            caseDetails.FWTCaseFullDetails.Interactions = new[]
            {
                new FWTInteractionDetails
                {
                    PartyID = new FWTObjectID
                    {
                        ObjectReference = new []
                        {
                            "test"
                        },
                        ObjectType = "C1"
                    }
                }
            };

            _mockClient
                .Setup(client => client.retrieveCaseDetailsAsync(It.IsAny<FWTCaseFullDetailsRequest>()))
                .ReturnsAsync(caseDetails);

            // Act
            await _caseService.GetCase("baseCase1");

            // Assert
            _mockClient.Verify(client => client.retrieveIndividualAsync(It.IsAny<FWTObjectID>()), Times.Once);
        }

        [Fact]
        public async Task GetCase_ShouldReturnMappedIndividual()
        {
            // Arrange
            var caseDetails = new retrieveCaseDetailsResponse
            {
                FWTCaseFullDetails = CreateBaseCase()
            };

            caseDetails.FWTCaseFullDetails.Interactions = new[]
            {
                new FWTInteractionDetails
                {
                    PartyID = new FWTObjectID
                    {
                        ObjectReference = new []
                        {
                            "test"
                        },
                        ObjectType = "C1"
                    }
                }
            };

            var individualDetails = new retrieveIndividualResponse
            {
                FWTIndividual = new FWTIndividual
                {
                    Name = new[]
                    {
                        new FWTIndividualName
                        {
                            Forename = new [] { "test" },
                            Surname = "test"
                        }
                    }
                }
            };

            _mockClient
                .Setup(client => client.retrieveCaseDetailsAsync(It.IsAny<FWTCaseFullDetailsRequest>()))
                .ReturnsAsync(caseDetails);

            _mockClient
                .Setup(client => client.retrieveIndividualAsync(It.IsAny<FWTObjectID>()))
                .ReturnsAsync(individualDetails);

            // Act
            var result = await _caseService.GetCase("baseCase1");

            // Assert
            Assert.NotNull(result.Customer);
        }

        [Fact]
        public async Task CreateCase_ShouldCall_Verint_createCaseDetailsAsync()
        {
            // Arrange
            var caseDetails = new Case
            {
                EventCode = 1234567,
                EventTitle = "test title",
                Description = "test description"
            };

            _mockClient
                .Setup(client => client.createCaseAsync(It.IsAny<FWTCaseCreate>()))
                .ReturnsAsync(It.IsAny<createCaseResponse>());

            // Act
            await _caseService.CreateCase(caseDetails);

            // Assert
            _mockClient.Verify(client => client.createCaseAsync(It.IsAny<FWTCaseCreate>()), Times.Once);
        }

        [Fact]
        public async Task CreateCase_ShouldThrowError()
        {
            // Arrange
            var caseDetails = new Case
            {
                EventCode = 1234567,
                EventTitle = "test title",
                Description = "test description"
            };

            _mockClient
                .Setup(client => client.createCaseAsync(It.IsAny<FWTCaseCreate>()))
                .Throws(new Exception());

            // Act
            await Assert.ThrowsAsync<Exception>(() => _caseService.CreateCase(caseDetails));

            // Assert
            _mockClient.Verify(_ => _.createCaseAsync(It.IsAny<FWTCaseCreate>()), Times.Once);
        }

        private FWTCaseFullDetails CreateBaseCase()
        {
            return new FWTCaseFullDetails
            {
                CoreDetails = new FWTCaseCoreDetails
                {
                    CaseReference = "1234",
                    Classification = new[] { "Test", "Test two", "Test three" },
                    Title = "Case title",
                    Description = "Description",
                    Status = "Status",
                    AssociatedObject = new FWTObjectBriefDetails
                    {
                        ObjectID = new FWTObjectID
                        {
                            ObjectType = Common.OrganisationObjectType
                        }
                    }
                },
                Events = new[]
                {
                    new FWTCaseEvent
                    {
                        EventTitle = "Event title",
                        Created = new DateTime()
                    }
                }
            };
        }
    }
}
