using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
using verint_service.Models;

namespace verint_service.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly IVerintClient _verintConnection;

        private readonly IIndividualService _individualService;

        public InteractionService(IVerintConnection verint, IIndividualService individualService)
        {
            _verintConnection = verint.Client();
            _individualService = individualService;
        }

        public async Task<long> CreateInteraction(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            var interactionDetails = new FWTInteractionCreate {
                Channel = Common.Channel,
                Verified = false,
            };

            FWTObjectID raisedBy = await GetRaisedByObject(crmCase);
            if(raisedBy != null)
            {
                interactionDetails.PartyID = raisedBy;
            }

            var createInteractionResult = await _verintConnection.createInteractionAsync(interactionDetails);
            return createInteractionResult.InteractionID;
        }

        private async Task<FWTObjectID> GetRaisedByObject(StockportGovUK.NetStandard.Models.Verint.Case crmCase)
        {
            FWTObjectID raisedBy = null;

            if(crmCase.Customer != null && crmCase.RaisedByBehaviour == RaisedByBehaviourEnum.Individual)
            {
                var individual = await _individualService.ResolveIndividual(crmCase.Customer);
                crmCase.Customer.CustomerReference = individual.ObjectReference[0];

                if(crmCase.RaisedByBehaviour == RaisedByBehaviourEnum.Individual)
                {
                    raisedBy = individual;
                }

                return raisedBy;
            }

            if(crmCase.Organisation != null)
            {
                raisedBy = new FWTObjectID()
                {
                    ObjectType = Common.OrganisationObjectType,
                    ObjectReference = new[] { crmCase.Organisation.Reference }
                };    
            }
            
            return raisedBy;
        }
    }
}