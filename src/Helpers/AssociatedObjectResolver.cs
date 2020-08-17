using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Utils.Consts;
using VerintWebService;

namespace verint_service.Helpers
{
    public class AssociatedObjectResolver : IAssociatedObjectResolver
    {
        public FWTObjectBriefDetails Resolve(Case crmCase)
        {
            var associatedObject = new FWTObjectID();
            var associatedObjectBriefDetails = new FWTObjectBriefDetails();

            switch(crmCase.AssociatedWithBehaviour)
            {
                case AssociatedWithBehaviourEnum.Street : 
                    if(crmCase.Street.Reference != null)
                    {
                        associatedObject.ObjectType = VerintConstants.StreetObjectType;
                        associatedObject.ObjectReference = new[] { crmCase.Street.Reference };
                        associatedObjectBriefDetails.ObjectID = associatedObject;
                        return associatedObjectBriefDetails;
                    
                    }
                    
                    break;

                case AssociatedWithBehaviourEnum.Property:
                    
                    if(crmCase.Property.Reference != null)
                    {
                        associatedObject.ObjectType = VerintConstants.PropertyObjectType;
                        associatedObject.ObjectReference = new[] { crmCase.Property.Reference };
                        associatedObjectBriefDetails.ObjectID = associatedObject;
                        return associatedObjectBriefDetails;
                    }
                    
                    break;

                case AssociatedWithBehaviourEnum.Organisation:
                    if(crmCase.Organisation.Reference != null)
                    {
                        associatedObjectBriefDetails.ObjectID =  new FWTObjectID
                        {
                            ObjectType = VerintConstants.OrganisationObjectType,
                            ObjectReference = new[] { crmCase.Organisation.Reference } 
                        };

                        return associatedObjectBriefDetails;
                    }
                    
                    break;

                case AssociatedWithBehaviourEnum.Individual:
                    if(crmCase.Customer.CustomerReference != null)
                    {
                        associatedObject.ObjectType = VerintConstants.IndividualObjectType;
                        associatedObject.ObjectReference = new[] { crmCase.Customer.CustomerReference };
                        associatedObjectBriefDetails.Details = crmCase.Customer.FullName;
                        associatedObjectBriefDetails.ObjectID = associatedObject;
                        return associatedObjectBriefDetails;
                    }
                    
                    break;
                    
                default: 
                    return null;
            }

            return null;    
        }
    }
}