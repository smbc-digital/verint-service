using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
using verint_service.Models;
using verint_service.Utils.Consts;
using Microsoft.Extensions.Logging;

namespace verint_service.Services
{

    
    public class InteractionService : IInteractionService
    {
        private ILogger<InteractionService> _logger;

        private readonly IVerintClient _verintConnection;

        private readonly IIndividualService _individualService;

        public InteractionService(IVerintConnection verint, IIndividualService individualService, ILogger<InteractionService> logger)
        {
            _verintConnection = verint.Client();
            _individualService = individualService;
            _logger = logger;
        }

        public async Task<long> CreateInteraction(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            _logger.LogInformation($"InteractionService.Create:Attempting to create interaction, Event {crmCase.EventTitle}, event code {crmCase.EventCode}");

            var interactionDetails = new FWTInteractionCreate {
                Channel = VerintConstants.Channel,
                Verified = false,
            };

            FWTObjectID raisedBy = await GetRaisedByObject(crmCase);
            if(raisedBy != null)
            {
                interactionDetails.PartyID = raisedBy;
            }

            var createInteractionResult = await _verintConnection.createInteractionAsync(interactionDetails);
            _logger.LogInformation($"InteractionService.Create: Create interaction, Id {createInteractionResult.InteractionID} Event {crmCase.EventTitle}, event code {crmCase.EventCode}");
            
            return createInteractionResult.InteractionID;
        }

        private async Task<FWTObjectID> GetRaisedByObject(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            FWTObjectID raisedBy = null;

            if(crmCase.Customer != null && crmCase.RaisedByBehaviour == RaisedByBehaviourEnum.Individual)
            {
                _logger.LogInformation($"InteractionService.GetRaisedByObject - Individual");

                var individual = await _individualService.ResolveIndividual(crmCase.Customer);
                crmCase.Customer.CustomerReference = individual.ObjectReference[0];
                raisedBy = individual;
                
                _logger.LogInformation($"InteractionService.GetRaisedByObject - Result Individual Id: {individual.ObjectReference[0]}");
                

                return raisedBy;
            }

            if(crmCase.Organisation != null)
            {
                _logger.LogInformation($"InteractionService.GetRaisedByObject - Organisation");

                raisedBy = new FWTObjectID()
                {
                    ObjectType = VerintConstants.OrganisationObjectType,
                    ObjectReference = new[] { crmCase.Organisation.Reference }
                };    
            }
            
            return raisedBy;
        }
    }
}