using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using verint_service.Builders;
using verint_service.Helpers;
using verint_service.Helpers.VerintConnection;
using verint_service.Mappers;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Services.Case
{
    public class CaseService : ICaseService
    {
        private readonly ILogger<CaseService> _logger;

        private readonly IVerintClient _verintConnection;

        private IIndividualService _individualService;

        private IInteractionService _interactionService;

        private IAssociatedObjectHelper _associatedObjectHelper;

        private ICaseFormBuilder _caseFormBuilder;

        public CaseService(IVerintConnection verint,
                            ILogger<CaseService> logger,
                            IIndividualService individualService,
                            IInteractionService interactionService,
                            IAssociatedObjectHelper associatedObjectHelper,
                            ICaseFormBuilder caseFormBuilder)
        {
            _logger = logger;
            _verintConnection = verint.Client();
            _individualService = individualService;
            _interactionService = interactionService;
            _associatedObjectHelper = associatedObjectHelper;
            _caseFormBuilder = caseFormBuilder;
        }

        public async Task<Models.Case> GetCase(string caseId)
        {
            _logger.LogWarning($"**DEBUG: CaseService: GetCase() caseId: {caseId}");

            if (string.IsNullOrWhiteSpace(caseId))
            {
                _logger.LogWarning($"**DEBUG: CaseService: GetCase(). Null or empty references are not allowed {caseId}");
                throw new Exception("Null or empty references are not allowed");
            }

            var caseRequest = new FWTCaseFullDetailsRequest
            {
                CaseReference = caseId.Trim(),
                Option = new[] { "all" }
            };

            _logger.LogWarning("**DEBUG: CaseService: GetCase(). Making call to Verint");

            var response = await _verintConnection.retrieveCaseDetailsAsync(caseRequest);
            _logger.LogWarning("**DEBUG: CaseService: GetCase(). Retrieved case response from Verint");


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
        
        public async Task<string> CreateCase(Models.Case crmCase)
        {
            var caseDetails = new FWTCaseCreate
            {
                ClassificationEventCode = crmCase.EventCode,
                Title = crmCase.EventTitle,
                Description = crmCase.Description,
            };

            if(crmCase.Customer != null)
            {
                var individual = await _individualService.ResolveIndividual(crmCase.Customer);
                var interactionReference = await _interactionService.CreateInteractionForIndividual(individual);
                crmCase.InteractionReference = interactionReference;
            }

            var associatedObjectBriefDetails = _associatedObjectHelper.GetAssociatedObject(crmCase);
            if (associatedObjectBriefDetails != null)
            {
                caseDetails.AssociatedObject = associatedObjectBriefDetails;
            }

            if (crmCase.CaseForm == null && !string.IsNullOrWhiteSpace(crmCase.FormName))
            {
                caseDetails.Form = _caseFormBuilder.Build(crmCase);
            }

            try
            {
                var result = await _verintConnection.createCaseAsync(caseDetails);
                return result.CaseReference;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}