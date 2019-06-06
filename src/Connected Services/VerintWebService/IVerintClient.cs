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
    }

    public partial class FLWebInterfaceClient : IVerintClient {}
}