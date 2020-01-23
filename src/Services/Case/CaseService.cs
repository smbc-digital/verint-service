using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using verint_service.Helpers.VerintConnection;
using verint_service.Mappers;
using VerintWebService;
using StockportGovUK.NetStandard.Models.Verint;

namespace verint_service.Services.Case
{
    public class CaseService : ICaseService
    {
        private readonly ILogger<CaseService> _logger;

        private readonly IVerintClient _verintConnection;

        private IInteractionService _interactionService;

        private CaseToFWTCaseCreateMapper _caseToFWTCaseCreateMapper;

        public CaseService(IVerintConnection verint,
                            ILogger<CaseService> logger,
                            IInteractionService interactionService,
                            CaseToFWTCaseCreateMapper caseToFWTCaseCreateMapper
                            )
        {
            _logger = logger;
            _verintConnection = verint.Client();
            _interactionService = interactionService;
            _caseToFWTCaseCreateMapper = caseToFWTCaseCreateMapper;
        }

        public async Task<StockportGovUK.NetStandard.Models.Verint.Case> GetCase(string caseId)
        {
            if (string.IsNullOrWhiteSpace(caseId))
            {
                throw new Exception("Null or empty references are not allowed");
            }

            var caseRequest = new FWTCaseFullDetailsRequest
            {
                CaseReference = caseId.Trim(),
                Option = new[] { "all" }
            };

            var response = await _verintConnection.retrieveCaseDetailsAsync(caseRequest);
            var caseDetails = response.FWTCaseFullDetails.MapToCase();

            if (response.FWTCaseFullDetails.CoreDetails.AssociatedObject != null)
            {
                if (response.FWTCaseFullDetails.CoreDetails.AssociatedObject.ObjectID.ObjectType ==
                    Common.OrganisationObjectType)
                {
                    var organisation = await _verintConnection.retrieveOrganisationAsync(response.FWTCaseFullDetails.CoreDetails.AssociatedObject.ObjectID);
                    if (organisation != null)
                    {
                        caseDetails.Organisation = organisation.FWTOrganisation.MapToOrganisation();
                    }
                }

                if (!string.IsNullOrWhiteSpace(response.FWTCaseFullDetails.Interactions[0]?.PartyID?.ObjectReference[0])
                    && response.FWTCaseFullDetails.Interactions[0]?.PartyID?.ObjectType == "C1")
                {
                    var individual = await _verintConnection.retrieveIndividualAsync(response.FWTCaseFullDetails.Interactions[0].PartyID);

                    if (individual != null)
                    {
                        caseDetails.Customer = individual.FWTIndividual.MapToCustomer();
                    }
                }
            }

            return caseDetails;
        }

        
        public async Task<string> CreateCase(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            crmCase.InteractionReference = await _interactionService.CreateInteraction(crmCase);                
            var caseDetails = _caseToFWTCaseCreateMapper.Map(crmCase);
            return _verintConnection.createCaseAsync(caseDetails).Result.CaseReference;            
        }

        public async Task<int> UpdateCaseDescription(Models.Case crmCase)
        {
            var caseDetails = new FWTCaseUpdate
            {
                CaseReference = crmCase.CaseReference,
                Description = crmCase.Description
            };

            try
            {
                var result = await _verintConnection.updateCaseAsync(caseDetails);
                return result.FWTCaseUpdateResponse;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error when updating Description field");
                throw;
            }
        }

        public async Task CreateNotesWithAttachment(NoteWithAttachments note)
        {
            var result = AddDocumentToCase(note.Attachments);
            var listA = new List<FWTNoteDetailAttachment>();
            result.ForEach(r =>
            {
                listA.Add(new FWTNoteDetailAttachment
                {
                    AttachmentIdentifier = r.documentReference,
                    AttachmentName = r.documentName,
                    AttachmentTypeSpecified = false
                });
            });

            var noteToParent = new FWTCreateNoteToParent
            {
                NoteDetails = new FWTCreateNoteDetail
                {
                    Text = note.AttachmentsDescription,
                    NoteAttachments = listA.ToArray()
                },
                ParentId = note.CaseRef,
                ParentType = note.Interaction
            };
           
            await _verintConnection.createNotesAsync(noteToParent);
        }

        private List<FWTAttachedDocument> AddDocumentToCase(List<Attachment> attachments)
        {
            var result = new List<FWTAttachedDocument>();
            attachments.ForEach(a =>
            {
                var doc = new FWTDocument
                {
                    DocumentName = a.Filename,
                    DocumentType = 1,
                    Document = a.Base64Content
                };

                var docRef = _verintConnection.addDocumentToRepositoryAsync(doc);
                var attachedDoc = new FWTAttachedDocument
                {
                    documentName = doc.DocumentName,
                    documentReference = docRef.Result.FWTDocumentRef,
                    storageTypeSpecified = false
                };
                result.Add(attachedDoc);
            });
            return result;
        }
    }
}