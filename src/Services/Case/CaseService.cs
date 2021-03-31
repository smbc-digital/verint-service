using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using verint_service.Helpers.VerintConnection;
using VerintWebService;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Utils.Consts;
using verint_service.Utils.Mappers;
using System.Diagnostics;
using verint_service.Utils.Builders;

namespace verint_service.Services.Case
{
    public class CaseService : ICaseService
    {
        private readonly ILogger<CaseService> _logger;

        private readonly IVerintClient _verintConnection;

        private IInteractionService _interactionService;

        private IIndividualService _individualService;

        private CaseToFWTCaseCreateMapper _caseToFWTCaseCreateMapper;

        public CaseService(IVerintConnection verint,
                            ILogger<CaseService> logger,
                            IInteractionService interactionService,
                            CaseToFWTCaseCreateMapper caseToFWTCaseCreateMapper,
                            IIndividualService individualService
                            )
        {
            _logger = logger;
            _verintConnection = verint.Client();
            _interactionService = interactionService;
            _caseToFWTCaseCreateMapper = caseToFWTCaseCreateMapper;
            _individualService = individualService;
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
                    VerintConstants.OrganisationObjectType)
                {
                    var organisation = await _verintConnection.retrieveOrganisationAsync(response.FWTCaseFullDetails.CoreDetails.AssociatedObject.ObjectID);
                    if (organisation != null)
                    {
                        caseDetails.Organisation = organisation.FWTOrganisation.Map();
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

        public async Task<string> Create(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            _logger.LogDebug($"CaseService.Create:{crmCase.ID}: Create Interaction");
            crmCase.InteractionReference = await _interactionService.CreateAsync(crmCase);
            var caseDetails = _caseToFWTCaseCreateMapper.Map(crmCase);

            try
            {
                _logger.LogDebug($"CaseService.Create:{crmCase.ID}: Create Case");
                return _verintConnection.createCaseAsync(caseDetails).Result.CaseReference;
            }
            catch(Exception ex)
            {
                throw new Exception($"CaseService.Create:{crmCase.ID} Verint create case failed", ex);
            }
        }

        public async Task<int> UpdateDescription(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
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

        public async Task<bool> AddCaseFormField(string caseReference, string key, string value)
        {
            var caseDetails = new FWTCaseUpdate
            {
                CaseReference = caseReference,
            };

            caseDetails.Form = new FWTCaseForm{ 
                                FormField = new FWTCaseFormField[] 
                                    { 
                                        CaseFormBuilder.CreateCaseFormField(key, value) 
                                    }
                                };
                                
            var result = await _verintConnection.updateCaseAsync(caseDetails);

            return result != null;
        }

        public async Task CreateNotesWithAttachment(NoteWithAttachments note)
        {
            try
            {
                var repositoryResult = await AddDocumentToRepository(note.Attachments);
                var attachedFileReferences = new List<FWTNoteDetailAttachment>();

                repositoryResult.ForEach(r =>
                {
                    attachedFileReferences.Add(new FWTNoteDetailAttachment
                    {
                        AttachmentIdentifier = r.documentReference,
                        AttachmentName = r.documentName,
                        AttachmentTypeSpecified = false
                    });
                });

                var noteWithAttachments = new FWTCreateNoteToParent
                {
                    NoteDetails = new FWTCreateNoteDetail
                    {
                        Text = note.AttachmentsDescription,
                        NoteAttachments = attachedFileReferences.ToArray()
                    },
                    ParentId = note.CaseRef,
                    ParentType = note.Interaction
                };

                await _verintConnection.createNotesAsync(noteWithAttachments);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                _logger.LogError(exception, "Error when adding attachment");
                throw;
            }
        }

        private async Task<List<FWTAttachedDocument>> AddDocumentToRepository(List<StockportGovUK.NetStandard.Models.FileManagement.File> attachments)
        {
            var attachedDocuments = new List<FWTAttachedDocument>();

            foreach(var attachment in attachments)
            {
                var document = new FWTDocument
                {
                    DocumentName = attachment.TrustedOriginalFileName,
                    DocumentType = 1,
                    Document = attachment.Content
                };

                var docRef = await _verintConnection.addDocumentToRepositoryAsync(document);
                var attachedDoc = new FWTAttachedDocument
                {
                    documentName = document.DocumentName,
                    documentReference = docRef.FWTDocumentRef,
                    storageTypeSpecified = false
                };
                attachedDocuments.Add(attachedDoc);
            }

            return attachedDocuments;
        }
    }
}