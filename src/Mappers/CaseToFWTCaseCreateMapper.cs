using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Builders;
using verint_service.Helpers;
using VerintWebService;

namespace verint_service.Mappers
{
    public class CaseToFWTCaseCreateMapper
    {

        private ICaseFormBuilder _caseFormBuilder;

        private IAssociatedObjectResolver _associatedObjectResolver;

        public CaseToFWTCaseCreateMapper(ICaseFormBuilder caseFormBuilder, IAssociatedObjectResolver associatedObjectResolver)
        {
            _caseFormBuilder = caseFormBuilder;
            _associatedObjectResolver = associatedObjectResolver;
        }

        public FWTCaseCreate Map(Case crmCase)
        {
            var caseCreateDetails = new FWTCaseCreate
            {
                ClassificationEventCode = crmCase.EventCode,
                Title = crmCase.EventTitle,
                Description = crmCase.Description,
                AssociatedObject = _associatedObjectResolver.Resolve(crmCase),
                Form = _caseFormBuilder.Build(crmCase),
                InteractionID = crmCase.InteractionReference,
                InteractionIDSpecified = true
            };

            return caseCreateDetails;
        }
    }
}