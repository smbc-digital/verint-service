using System.Linq;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Builders
{
    public class CaseFormBuilder : ICaseFormBuilder
    {
        public FWTCaseForm Build(Case crmCase)
        {
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

        private static FWTCaseFormField CreateCaseFormField(CustomField customField)
        {
            var caseFormField = new FWTCaseFormField();
            caseFormField.Value = customField.Value ?? string.Empty;
            caseFormField.Key = customField.Name;

            return caseFormField;
        }
    }
}