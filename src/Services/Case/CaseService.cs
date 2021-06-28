using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using verint_service.Helpers.VerintConnection;
using VerintWebService;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Utils.Consts;
using verint_service.Utils.Mappers;
using verint_service.Utils.Builders;
using StockportGovUK.NetStandard.Abstractions.Caching;
using Newtonsoft.Json;
using System.Threading;

namespace verint_service.Services.Case
{
    public class CaseService : ICaseService
    {
        private readonly ILogger<CaseService> _logger;

        private readonly IVerintClient _verintConnection;

        private IInteractionService _interactionService;

        private IIndividualService _individualService;

        private ICacheProvider _cacheProvider;

        private CaseToFWTCaseCreateMapper _caseToFWTCaseCreateMapper;

        public CaseService(IVerintConnection verint,
                            ILogger<CaseService> logger,
                            IInteractionService interactionService,
                            CaseToFWTCaseCreateMapper caseToFWTCaseCreateMapper,
                            IIndividualService individualService,
                            ICacheProvider cacheProvider)
        {
            _logger = logger;
            _verintConnection = verint.Client();
            _interactionService = interactionService;
            _caseToFWTCaseCreateMapper = caseToFWTCaseCreateMapper;
            _individualService = individualService;
            _cacheProvider = cacheProvider;
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
            crmCase.InteractionReference = await _interactionService.CreateAsync(crmCase);
            var caseDetails = _caseToFWTCaseCreateMapper.Map(crmCase);

            try
            {
                var result = _verintConnection.createCaseAsync(caseDetails).Result.CaseReference;
                if (crmCase.UploadNotesWithAttachmentsAfterCaseCreation)
                {
                    _logger.LogError($"CaseService.create: (crmCase.Note.Count) Notes {crmCase.NotesWithAttachments.Count}. ");
                    crmCase.NotesWithAttachments.ForEach(note => note.CaseRef = Convert.ToInt64(result));
                    await CacheNotesWithAttachments(result, crmCase.NotesWithAttachments);
                }
                else
                {
                    crmCase.NotesWithAttachments.ForEach(async note => {
                        note.CaseRef = Convert.ToInt64(result);
                        await CreateNotesWithAttachment(note);
                    });
                }                

                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"CaseService.Create:{crmCase.ID} Verint create case failed");
                throw ex;
            }
        }

        public async Task<string> Close(string caseReference, string reasonTitle, string description)
        {
            try
            {
                var closeCase = new FWTCaseClose
                {
                    CaseReference = new[] {caseReference},
                    Reason = new FWTCaseActionReason {Title = reasonTitle, Description = description}
                };

                var reference = await _verintConnection.closeCasesAsync(closeCase);
                return reference.ToString();
            }
            catch(Exception ex)
            {
                throw new Exception($"CaseService.Create:{caseReference} Verint create case failed", ex);
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

        private async Task CacheNotesWithAttachments(string id, List<NoteWithAttachments> notesWithAttachment)
        {
            await _cacheProvider.SetStringAsync(id, JsonConvert.SerializeObject(notesWithAttachment));
        }

        public async Task WriteCachedNotes(string id)
        {
            if (id.Contains('-'))
                id = id.Split('-')[1];

            string json = await _cacheProvider.GetStringAsync(id);
            if (!string.IsNullOrEmpty(json))
            {
                var notes = JsonConvert.DeserializeObject<List<NoteWithAttachments>>(json);
              //notes.ForEach(async note => await CreateNotesWithAttachment(note));
				foreach (var note in notes)
				{
                    await CreateNotesWithAttachment(note);
                    _logger.LogError($"CaseController.WriteCachedNotes: {note.Attachments[0].TrustedOriginalFileName}. {DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}");
                    //Thread.Sleep(1000);
                }
            }
        }


		public async Task CreateNotesWithAttachment(NoteWithAttachments note)
        {
            try
            {
                var repositoryResult = await AddDocumentToRepository(note.Attachments);
                _logger.LogError($"CaseController.CreateNotesWithAttachment: Docuiment attachmet keyname {note.Attachments[0].TrustedOriginalFileName}. {DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}");
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

                _logger.LogError($"CaseController.AddNoteWithAttachments: Number of attachments {note.Attachments.Count}. {DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}");
                await _verintConnection.createNotesAsync(noteWithAttachments);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                _logger.LogError(exception, "Error when adding attachment");
                throw;
            }
        }

        public async Task CreateNote(NoteRequest noteRequest)
        {
                var note = new FWTCreateNoteToParent
                {
                    NoteDetails = new FWTCreateNoteDetail
                    {
                        Text = noteRequest.NoteText
                    },
                    ParentId = noteRequest.CaseRef,
                    ParentType = noteRequest.Interaction
                };

                await _verintConnection.createNotesAsync(note);
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