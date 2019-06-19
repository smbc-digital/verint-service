using System;
using System.Linq;
using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Services.Update
{
    public class UpdateService : IUpdateService
    {
        private readonly IVerintClient _verintConnection;

        public UpdateService(IVerintConnection verint)
        {
            _verintConnection = verint.Client();
        }

        public async Task<writeCaseEformDataResponse> UpdateIntegrationFormFields(IntegrationFormFieldsUpdateEntity updateEntity)
        {
            var eformData = new FWTCaseEformData();
            var formFields = new FWTEformField[0];

            var caseEformInstance = new FWTCaseEformInstance
            {
                CaseReference = updateEntity.CaseReference,
                EformName = updateEntity.IntegrationFormName
            };

            // Enquiry Details Panel
            if (updateEntity.IntegrationFormFields != null && updateEntity.IntegrationFormFields.Any())
            {
                formFields = new FWTEformField[updateEntity.IntegrationFormFields.Count];
                var count = 0;

                foreach (var field in updateEntity.IntegrationFormFields)
                {
                    var caseFormField = new FWTEformField
                    {
                        FieldValue = field.FormFieldValue ?? string.Empty,
                        FieldName = field.FormFieldName
                    };
                    // If value is null, set to empty string
                    formFields[count] = caseFormField;
                    count = count + 1;
                }
            }

            eformData.CaseEformInstance = caseEformInstance;
            eformData.EformData = formFields;

            try
            {
                return await _verintConnection.writeCaseEformDataAsync(eformData);
            }
            catch (Exception e)
            {
                throw new Exception($"UpdateService: UpdateIntegrationFormField threw and exception while attempting to update EForm Data, Exception: {e}");
            }
        }
    }
}
