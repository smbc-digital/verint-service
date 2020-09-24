using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
using verint_service.Utils.Consts;
using Microsoft.Extensions.Logging;
using verint_service.Services.Organisation;
using System.Linq;

namespace verint_service.Services
{
    public class InteractionService : IInteractionService
    {
        private ILogger<InteractionService> _logger;
        private readonly IVerintClient _verintConnection;
        private readonly IIndividualService _individualService;
        private readonly IOrganisationService _organisationService;

        public InteractionService(IVerintConnection verint, IIndividualService individualService, IOrganisationService organisationService, ILogger<InteractionService> logger)
        {
            _verintConnection = verint.Client();
            _individualService = individualService;
            _organisationService = organisationService;
            _logger = logger;
        }

        public async Task<long> CreateAsync(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            _logger.LogDebug($"InteractionService.Create:{crmCase.ID}:Attempting to create interaction, Event {crmCase.EventTitle}, event code {crmCase.EventCode}");
            var interactionDetails = new FWTInteractionCreate
            {
                Channel = VerintConstants.Channel,
                Verified = false
            };

            if (crmCase.Customer != null && string.IsNullOrEmpty(crmCase.Customer.CustomerReference))
            {
                var individual = await _individualService.ResolveAsync(crmCase.Customer);
                crmCase.Customer.CustomerReference = individual.ObjectReference[0];
            }
            
            if (crmCase.Organisation != null && string.IsNullOrEmpty(crmCase.Organisation.Reference))
            {
                var organisation = await _organisationService.ResolveAsync(crmCase.Organisation);
                crmCase.Organisation.Reference = organisation.ObjectReference[0];
            }

            if (crmCase.Organisation != null || crmCase.Customer != null)
            {
                interactionDetails.PartyID = GetRaisedByObjects(crmCase);
            }

            var createInteractionResult = await _verintConnection.createInteractionAsync(interactionDetails);
            _logger.LogDebug($"InteractionService.Create:{crmCase.ID}: Created interaction, Id {createInteractionResult.InteractionID} Event {crmCase.EventTitle}, event code {crmCase.EventCode}");
            
            return createInteractionResult.InteractionID;
        }

        private FWTObjectID GetRaisedByObjects(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            FWTObjectID raisedBy = null;
            
            if(crmCase.Customer != null && crmCase.RaisedByBehaviour == RaisedByBehaviourEnum.Individual)
            {
                raisedBy = new FWTObjectID
                {
                    ObjectType = VerintConstants.IndividualObjectType,
                    ObjectReference = new string[] { crmCase.Customer.CustomerReference }
                };
            }
            else if(crmCase.Organisation != null && crmCase.RaisedByBehaviour == RaisedByBehaviourEnum.Organisation)
            {
                raisedBy = new FWTObjectID
                {
                    ObjectType = VerintConstants.OrganisationObjectType,
                    ObjectReference = new string[] { crmCase.Organisation.Reference }
                };
            }

            _logger.LogDebug($"InteractionService: GetRaisedByObject - Raised By: {raisedBy.ObjectReference.First()}, {raisedBy.ObjectType}");
            
            return raisedBy;
        }
    }
}