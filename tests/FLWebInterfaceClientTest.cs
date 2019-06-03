using verint_service.Models;
using VerintWebService;

namespace verint_service_tests
{
    public class FLWebInterfaceClientTest : FLWebInterfaceClient
    {
        public retrieveCaseDetailsResponse retrieveCaseDetailsAsync(FWTCaseFullDetailsRequest FWTCaseFullDetailsRequest)
        {
            return new retrieveCaseDetailsResponse
            {
                FWTCaseFullDetails = new FWTCaseFullDetails
                {
                    CoreDetails = new FWTCaseCoreDetails
                    {
                        AssociatedObject = new FWTObjectBriefDetails
                        {
                            ObjectID = new FWTObjectID
                            {
                                ObjectType = Common.OrganisationObjectType
                            }
                        }
                    }
                }
            };
        }

        public retrieveOrganisationResponse retrieveOrganisationAsync(FWTObjectID FWTObjectID)
        {
            return new retrieveOrganisationResponse
            {
                FWTOrganisation = new FWTOrganisation
                {
                    Name = new FWTOrganisationName[]
                    {
                        new FWTOrganisationName
                        {
                            FullName = "MockOrganisation"
                        }
                    }
                }
            };
        }
    }
}
