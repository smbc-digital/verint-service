using verint_service.Models;
using VerintWebService;

namespace verint_service.Helpers
{

    public class AssociatedObjectHelper : IAssociatedObjectHelper
    {
        public AssociatedObjectHelper()
        {
        }

        public FWTObjectBriefDetails GetAssociatedObject(Models.Case crmCase)
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