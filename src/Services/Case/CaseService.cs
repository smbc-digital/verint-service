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

        public CaseService(IVerintConnection verint, ILogger<CaseService> logger)
        {
            _logger = logger;
            _verintConnection = verint.Client();
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
    }
}