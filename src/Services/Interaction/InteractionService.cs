using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Services
{
    public interface IInteractionService
    {
        Task<long> CreateInteractionForIndividual(FWTObjectID individual);
    }

    public class InteractionService : IInteractionService
    {
        private readonly IVerintClient _verintConnection;

        public InteractionService(IVerintConnection verint)
        {
            _verintConnection = verint.Client();
        }

        public async Task<long> CreateInteractionForIndividual(FWTObjectID individual)
        {
            var interactionDetails = new FWTInteractionCreate {
                Channel = Common.Channel,
                Verified = false,
                PartyID = individual
            };

            var createInteractionResult = await _verintConnection.createInteractionAsync(interactionDetails);

            return createInteractionResult.InteractionID;
        }
    }
}