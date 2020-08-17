using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
using verint_service.Models;
using verint_service.Utils.Consts;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using verint_service.Services.Organisation;

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

        public async Task<long> CreateInteraction(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            _logger.LogDebug($"InteractionService.Create:Attempting to create interaction, Event {crmCase.EventTitle}, event code {crmCase.EventCode}");
            var interactionDetails = new FWTInteractionCreate
            {
                Channel = VerintConstants.Channel,
                Verified = false,
            };

            await ResolveIndividual(crmCase);
            await ResolveOrganisation(crmCase);

            if (crmCase.Organisation != null && crmCase.Customer != null)
            {
                interactionDetails.PartyID = GetRaisedByObjects(crmCase);
            }

            var createInteractionResult = await _verintConnection.createInteractionAsync(interactionDetails);
            _logger.LogDebug($"InteractionService.Create: Created interaction, Id {createInteractionResult.InteractionID} Event {crmCase.EventTitle}, event code {crmCase.EventCode}");
            
            return createInteractionResult.InteractionID;
        }

        private async Task ResolveIndividual(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            if (crmCase.Customer != null && string.IsNullOrEmpty(crmCase.Customer.CustomerReference))
            {
                var individual = await _individualService.ResolveIndividual(crmCase.Customer);
                crmCase.Customer.CustomerReference = individual.ObjectReference[0];
            }
        }

        private async Task ResolveOrganisation(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            if (crmCase.Organisation != null && string.IsNullOrEmpty(crmCase.Organisation.Reference))
            {
                var organisation = await _organisationService.ResolveAsync(crmCase.Organisation);
                crmCase.Organisation.Reference = organisation.ObjectReference[0];
            }
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
            else if(crmCase.Organisation != null)
            {
                raisedBy = new FWTObjectID
                {
                    ObjectType = VerintConstants.OrganisationObjectType,
                    ObjectReference = new string[] { crmCase.Organisation.Reference }
                };
            }

            _logger.LogDebug($"InteractionService: GetRaisedByObject - Raised By: {raisedBy.ObjectReference}, {raisedBy.ObjectType}");
            
            return raisedBy;
        }
    }
}