using System.Threading.Tasks;

namespace VerintWebService
{
    /*
     * Add methods that are contained within Reference.cs:FLWebInterfaceClient to use within service.
     * This is used to mock the client within unit testing project.
     */
    public interface IVerintClient
    {
        Task<retrieveCaseDetailsResponse> retrieveCaseDetailsAsync(FWTCaseFullDetailsRequest FWTCaseFullDetailsRequest);

        Task<retrieveOrganisationResponse> retrieveOrganisationAsync(FWTObjectID FLOrganisationID);

        Task<retrieveIndividualResponse> retrieveIndividualAsync(FWTObjectID FLIndividualID);

        Task<writeCaseEformDataResponse> writeCaseEformDataAsync(FWTCaseEformData eformData);

        Task<updateCaseResponse> updateCaseAsync(FWTCaseUpdate FWTCaseUpdate);

        Task<createCaseResponse> createCaseAsync(FWTCaseCreate crmCase);

        Task<createInteractionResponse> createInteractionAsync(FWTInteractionCreate interaction);

        Task<createIndividualResponse> createIndividualAsync(FWTIndividual individual);

        Task<searchForPartyResponse> searchForPartyAsync(FWTPartySearch searchCriteria);

        Task<updateIndividualResponse> updateIndividualAsync(FWTIndividualUpdate individualUpdate);

        Task<searchForPropertyResponse> searchForPropertyAsync(FWTPropertySearch propertySearch);
    }


    public partial class FLWebInterfaceClient : IVerintClient
    {

    }
}