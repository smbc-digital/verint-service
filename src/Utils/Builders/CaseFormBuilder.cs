using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Utils.Builders
{
    public class CaseFormBuilder : ICaseFormBuilder
    {
        public FWTCaseForm Build(Case crmCase)
        {
            if (string.IsNullOrWhiteSpace(crmCase.FormName))
            {
                return null;
            }
            
            var caseForm = new FWTCaseForm { FormName = crmCase.FormName };

            if (crmCase.CaseFormFields == null || !crmCase.CaseFormFields.Any())
            {
                return caseForm;
            }

            caseForm.FormField = new FWTCaseFormField[crmCase.CaseFormFields.Count];

            var count = 0;

            foreach (var field in crmCase.CaseFormFields)
            {
                var caseFormField = CreateCaseFormField(field);
                caseForm.FormField[count] = caseFormField;
                count = count + 1;
            }

            return caseForm;
        }

        public static FWTCaseFormField CreateCaseFormField(CustomField customField) => CreateCaseFormField(customField.Name, customField.Value);     

        public static FWTCaseFormField CreateCaseFormField(string key, string value) => new FWTCaseFormField { Value = value, Key = key };
    }
}