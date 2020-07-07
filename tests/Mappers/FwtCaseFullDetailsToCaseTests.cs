using System;
using System.Linq;
using Moq;
using verint_service.Helpers.VerintConnection;
using verint_service.Services;
using verint_service.Utils.Mappers;
using VerintWebService;
using Xunit;

namespace verint_service_tests.Mappers
{
    public class FwtCaseFullDetailsToCaseTests
    {
        [Fact]
        public void MapToCase_ShouldMapClassification()
        {
            // Arrange
            var baseCase = CreateBaseCase();

            // Act
            var mappedCase = baseCase.MapToCase();

            // Assert
            Assert.Equal("Test > Test two > Test three", mappedCase.Classification);
        }

        [Fact]
        public void MapToCase_ShouldMapAssociatedObject_WhenObjectTypeIsD3()
        {
            // Arrange
            var baseCase = CreateBaseCase();
            baseCase.CoreDetails.AssociatedObject = new FWTObjectBriefDetails
            {
                ObjectID = new FWTObjectID
                {
                    ObjectType = "D3"
                },
                ObjectDescription = "test desc"
            };

            // Act
            var mappedCase = baseCase.MapToCase();

            // Assert
            Assert.Equal("test desc", mappedCase.Property.Description);
        }

        [Fact]
        public void MapToCase_ShouldMapAssociatedObject_WhenObjectTypeIsD4()
        {
            // Arrange
            var baseCase = CreateBaseCase();
            baseCase.CoreDetails.AssociatedObject = new FWTObjectBriefDetails
            {
                ObjectID = new FWTObjectID
                {
                    ObjectType = "D4"
                },
                ObjectDescription = "test desc"
            };

            // Act
            var mappedCase = baseCase.MapToCase();

            // Assert
            Assert.Equal("test desc", mappedCase.Street.Description);
        }

        [Fact]
        public void MapToCase_ShouldMapForm()
        {
            // Arrange
            var baseCase = CreateBaseCase();
            baseCase.Form = new FWTCaseForm
            {
                FormName = "Form name",
                FormField = new []
                {
                    new FWTCaseFormField
                    {
                        Key = "key",
                        Label = "label",
                        Value = "value"
                    },
                    new FWTCaseFormField
                    {
                        Key = "key 2",
                        Label = "label 2",
                        Value = "value 2"
                    } 
                }
            };

            // Act
            var mappedCase = baseCase.MapToCase();

            // Assert
            Assert.Equal("Form name", mappedCase.FormName);
            Assert.True(mappedCase.CaseFormFields.Exists(_ => _.Name == "key"));
            Assert.True(mappedCase.CaseFormFields.Exists(_ => _.Label == "label"));
            Assert.True(mappedCase.CaseFormFields.Exists(_ => _.Value == "value"));
            Assert.True(mappedCase.CaseFormFields.Exists(_ => _.Name == "key 2"));
            Assert.True(mappedCase.CaseFormFields.Exists(_ => _.Label == "label 2"));
            Assert.True(mappedCase.CaseFormFields.Exists(_ => _.Value == "value 2"));
        }

        [Fact]
        public void MapToCase_ShouldMapEFormDataFields()
        {
            // Arrange
            var baseCase = CreateBaseCase();
            baseCase.EformData = new []
            {
                new FWTCaseEformData
                {
                    EformData = new []
                    {
                        new FWTEformField
                        {
                            FieldName = "field name 1",
                            FieldValue = "field value 1"
                        },
                        new FWTEformField
                        {
                            FieldName = "field name 2",
                            FieldValue = "field value 2"
                        },
                    }
                }, 
            };

            // Act
            var mappedCase = baseCase.MapToCase();

            // Assert
            Assert.True(mappedCase.IntegrationFormFields.Exists(field => field.Name == "field name 1" && field.Value == "field value 1"));
            Assert.True(mappedCase.IntegrationFormFields.Exists(field => field.Name == "field name 2" && field.Value == "field value 2"));
        }

        [Fact]
        public void MapToCase_ShouldMapDefinitionName()
        {
            // Arrange
            var baseCase = CreateBaseCase();
            baseCase.Eforms = new FWTCaseEform[]
            {
                new FWTCaseEform
                {
                    Created = DateTime.Now.AddDays(-2),
                    Name = "Eform Name 1"
                }, 
                new FWTCaseEform
                {
                    Created = DateTime.Now.AddDays(-1),
                    Name = "Eform Name 2"
                } 
            };

            // Act
            var mappedCase = baseCase.MapToCase();

            // Assert
            Assert.Equal("Eform Name 2", mappedCase.DefinitionName);
        }

        [Fact]
        public void MapToCase_ShouldMapNotes()
        {
            // Arrange
            var baseCase = CreateBaseCase();
            baseCase.Notes = new[]
            {
                new FWTNote
                {
                    NoteID = 1,
                    Created = DateTime.Now,
                    CreatedBy = new FWTUser
                    {
                        UserName = "test name 1"
                    },
                    Text = "test text 1"
                },
                new FWTNote
                {
                    NoteID = 2,
                    Created = DateTime.Now,
                    CreatedBy = new FWTUser
                    {
                        UserName = "test name 2"
                    },
                    Text = "test text 2"
                }
            };

            // Act
            var mappedCase = baseCase.MapToCase();

            // Assert
            Assert.True(mappedCase.Notes.Exists(note => note.ID == 1 && note.Text == "test text 1" && note.CreatedBy == "test name 1"));
            Assert.True(mappedCase.Notes.Exists(note => note.ID == 2 && note.Text == "test text 2" && note.CreatedBy == "test name 2"));
        }

        [Fact]
        public void MapToCase_ShouldMapLinkedCases()
        {
            // Arrange
            var baseCase = CreateBaseCase();
            baseCase.LinkCases = new FWTLinkedCase[]
            {
                new FWTLinkedCase
                {
                    LinkedCase = "case 1"
                }, 
                new FWTLinkedCase
                {
                    LinkedCase = "case 2"
                } 
            };

            // Act
            var mappedCase = baseCase.MapToCase();

            // Assert
            Assert.True(mappedCase.LinkCases.Exists(_ => _ == "case 1"));
            Assert.True(mappedCase.LinkCases.Exists(_ => _ == "case 2"));
        }

        private FWTCaseFullDetails CreateBaseCase()
        {
            return new FWTCaseFullDetails
            {
                CoreDetails = new FWTCaseCoreDetails
                {
                    CaseReference = "1234",
                    Classification = new [] { "Test", "Test two", "Test three" },
                    Title = "Case title",
                    Description = "Description",
                    Status = "Status"
                },
                Events = new []
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
