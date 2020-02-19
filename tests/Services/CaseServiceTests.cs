using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Case;
using VerintWebService;
using Xunit;
using verint_service.Services;
using verint_service.Helpers;
using verint_service.Builders;
using verint_service.Mappers;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service;

namespace verint_service_tests.Services
{
    public class CaseServiceTests
    {
        private readonly Mock<IVerintClient> _mockClient = new Mock<IVerintClient>();
        private readonly Mock<IVerintConnection> _mockConnection = new Mock<IVerintConnection>();
        private readonly Mock<ILogger<CaseService>> _mockLogger = new Mock<ILogger<CaseService>>();
        private readonly Mock<IInteractionService> _mockInteractionService = new Mock<IInteractionService>();
        private readonly Mock<IAssociatedObjectResolver> _mockAssociatedObjectHelper = new Mock<IAssociatedObjectResolver>();
        private readonly CaseService _caseService;


        public CaseServiceTests()
        {
            _mockConnection
                .Setup(_ => _.Client())
                .Returns(_mockClient.Object);

            _mockInteractionService
                .Setup(_ => _.CreateInteraction(It.IsAny<Case>()))
                .ReturnsAsync(987654321);

            _mockAssociatedObjectHelper
                .Setup(helper => helper.Resolve(It.IsAny<Case>()))
                .Returns(It.IsAny<FWTObjectBriefDetails>());
            
            _caseService = new CaseService(_mockConnection.Object, _mockLogger.Object, _mockInteractionService.Object, new CaseToFWTCaseCreateMapper(new CaseFormBuilder(), _mockAssociatedObjectHelper.Object));
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
        public async Task GetCase_WithCustomer_ShouldCall_Verint_retrieveIndividualAsync()
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

        public async Task CreateCase_WithAssociatedStreet_ShouldCall_AssociatedObjectHelper()
        {
            // Arrange
            var testCase = new Case()
            {
                Street = new Street(){
                    USRN = "38102548",
                    Reference = "38102548",
                    Description = "Hibbert Lane"
                }
            };

            _mockClient
                .Setup(client => client.createCaseAsync(It.IsAny<FWTCaseCreate>()))
                .ReturnsAsync(new createCaseResponse{
                    CaseReference = "1223456"
                });

            // Act
            await _caseService.CreateCase(testCase);

            // Assert
            _mockAssociatedObjectHelper.Verify(helper => helper.Resolve(It.IsAny<Case>()), Times.Once);
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
                .ReturnsAsync(new createCaseResponse{
                    CaseReference = "1223456"
                });

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

        [Fact]
        public async Task UpdateCaseDescription_HappyPath()
        {
            // Arrange
            var caseDetails = new Case
            {
                EventCode = 1234567,
                EventTitle = "test title",
                Description = "test description",
                CaseReference = "1234"
            };

            _mockClient
                .Setup(client => client.updateCaseAsync(It.IsAny<FWTCaseUpdate>()))
                .ReturnsAsync(() => new updateCaseResponse{ FWTCaseUpdateResponse = 1});

            // Act
            await _caseService.UpdateCaseDescription(caseDetails);

            // Assert
            _mockClient.Verify(client => client.updateCaseAsync(It.IsAny<FWTCaseUpdate>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCaseDescription_ShouldThrowError()
        {
            // Arrange
            var caseDetails = new Case
            {
                EventCode = 1234567,
                EventTitle = "test title",
                Description = "test description"
            };

            _mockClient
                .Setup(client => client.updateCaseAsync(It.IsAny<FWTCaseUpdate>())).Throws(new Exception());

            // Act
            await Assert.ThrowsAsync<Exception>(() => _caseService.UpdateCaseDescription(caseDetails));

            // Assert
            _mockClient.Verify(client => client.updateCaseAsync(It.IsAny<FWTCaseUpdate>()), Times.Once);
        }

        [Fact]
        public async Task CreateNotesWithAttachment_ShouldCallRepository_ToAddUploadedFile()
        {
            var file1Name = "file.txt";
            var file1Content = "cba";
            var file2Name = "second.txt";
            var file2Content = "abc";

            // Arrange
            var fileRequest = new NoteWithAttachments
            {
                Attachments = new System.Collections.Generic.List<StockportGovUK.NetStandard.Models.Models.FileManagement.File>
                {
                    new StockportGovUK.NetStandard.Models.Models.FileManagement.File 
                    {
                        Content = file1Content,
                        FileName = file1Name
                    },
                    new StockportGovUK.NetStandard.Models.Models.FileManagement.File 
                    {
                        Content = file2Content,
                        FileName = file2Name
                    },
                },
                AttachmentsDescription = "description",
                CaseRef = 123456789123
            };

            _mockClient
                .Setup(client => client.addDocumentToRepositoryAsync(It.IsAny<FWTDocument>())).ReturnsAsync(new addDocumentToRepositoryResponse{ FWTDocumentRef = "123REF" });

            // Act
            await _caseService.CreateNotesWithAttachment(fileRequest);

            // Assert
            _mockClient.Verify(client => client.addDocumentToRepositoryAsync(It.IsAny<FWTDocument>()), Times.Exactly(2));
            _mockClient.Verify(client => client.addDocumentToRepositoryAsync(It.Is<FWTDocument>(x => x.DocumentName == file1Name && x.Document == file1Content)), Times.Once);
            _mockClient.Verify(client => client.addDocumentToRepositoryAsync(It.Is<FWTDocument>(x => x.DocumentName == file2Name && x.Document == file2Content)), Times.Once);
        }

                [Fact]
        public async Task CreateNotesWithAttachment_ShouldAttachDocumentToCase_AsNote()
        {
            var casRef = 123456789123;
            // Arrange
            var fileRequest = new NoteWithAttachments
            {
                Attachments = new System.Collections.Generic.List<StockportGovUK.NetStandard.Models.Models.FileManagement.File>
                {
                    new StockportGovUK.NetStandard.Models.Models.FileManagement.File 
                    {
                        Content = "abc",
                        FileName = "file.txt"
                    },
                    new StockportGovUK.NetStandard.Models.Models.FileManagement.File 
                    {
                        Content = "cba",
                        FileName = "second.txt"
                    },
                },
                AttachmentsDescription = "description",
                CaseRef = casRef
            };

            _mockClient
                .Setup(client => client.addDocumentToRepositoryAsync(It.IsAny<FWTDocument>())).ReturnsAsync(new addDocumentToRepositoryResponse{ FWTDocumentRef = "123REF" });

            // Act
            await _caseService.CreateNotesWithAttachment(fileRequest);

            // Assert
            _mockClient.Verify(client => client.addDocumentToRepositoryAsync(It.IsAny<FWTDocument>()), Times.Exactly(2));
            _mockClient.Verify(client => client.createNotesAsync(It.Is<FWTCreateNoteToParent>(x => x.ParentId == casRef && x.NoteDetails.NoteAttachments.Length == 2)), Times.Once);
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