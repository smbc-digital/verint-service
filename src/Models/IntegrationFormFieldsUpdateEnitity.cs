using System.Collections.Generic;

namespace verint_service.Models
{
    public class IntegrationFormFieldsUpdateEntity
    {
        public string CaseReference { get; set; }

        public string IntegrationFormName { get; set; }

        public List<IntegrationFormField> IntegrationFormFields { get; set; }
    }

    public class IntegrationFormField
    {
        public string FormFieldName { get; set; }

        public string FormFieldValue { get; set; }
    }
}

