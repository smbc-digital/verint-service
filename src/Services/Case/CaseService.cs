using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        public CaseService(IVerintConnection verint, ILogger<CaseService> logger, IIndividualService individualService, IInteractionService interactionService)
        {
            _logger = logger;
            _verintConnection = verint.Client();
            _individualService = individualService;
            _interactionService = interactionService;
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

            var associatedObjectBriefDetails = GetAssociatedObject(crmCase);
            if (associatedObjectBriefDetails != null)
            {
                caseDetails.AssociatedObject = associatedObjectBriefDetails;
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

        private FWTObjectBriefDetails GetAssociatedObject(Models.Case crmCase)
        {
            var associatedObject = new FWTObjectID();
            var associatedObjectBriefDetails = new FWTObjectBriefDetails();

            if (crmCase.Property != null && crmCase.Property.Reference != null)
            {
                associatedObject.ObjectType = Common.PropertyObjectType;
                associatedObject.ObjectReference = new[] { crmCase.Property.Reference };
            }
            else if (crmCase.Street != null && crmCase.Street.Reference != null)
            {
                associatedObject.ObjectType = Common.StreetObjectType;
                associatedObject.ObjectReference = new[] { crmCase.Street.Reference };
            }
            else if (crmCase.Organisation != null && crmCase.Organisation.Reference != null)
            {
                associatedObject.ObjectType = Common.OrganisationObjectType;
                associatedObject.ObjectReference = new[] { crmCase.Organisation.Reference };
                associatedObjectBriefDetails.Details = crmCase.Organisation.Name;
            }
            else if (crmCase.Customer != null && crmCase.Customer.CustomerReference != null)
            {
                associatedObject.ObjectType = Common.IndividualObjectType;
                associatedObject.ObjectReference = new[] { crmCase.Customer.CustomerReference };
                associatedObjectBriefDetails.Details = crmCase.Customer.FullName;
            }
            else
            {
                return null;
            }

            associatedObjectBriefDetails.ObjectID = associatedObject;
            return associatedObjectBriefDetails;
        }
    }
}