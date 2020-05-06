using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Helpers;
using verint_service.Utils.Builders;
using VerintWebService;

namespace verint_service.Utils.Mappers
{
    public class CaseToFWTCaseCreateMapper
    {

        private readonly ICaseFormBuilder _caseFormBuilder;

        private readonly IAssociatedObjectResolver _associatedObjectResolver;

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