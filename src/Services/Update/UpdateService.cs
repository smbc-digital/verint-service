using System;
using System.Linq;
using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Models.Verint.Update;
using verint_service.Helpers.VerintConnection;
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

        public async Task<writeCaseEformDataResponse> UpdateIntegrationFormFields(IntegrationFormFieldsUpdateModel updateEntity)
        {
            var eformData = new FWTCaseEformData();
            var formFields = new FWTEformField[0];

            var caseEformInstance = new FWTCaseEformInstance
            {
                CaseReference = updateEntity.CaseReference,
                EformName = updateEntity.IntegrationFormName
            };

            if (updateEntity.IntegrationFormFields != null && updateEntity.IntegrationFormFields.Any())
            {
                formFields = new FWTEformField[updateEntity.IntegrationFormFields.Count];
                for (int i = 0; i < updateEntity.IntegrationFormFields.Count; i++)
                {
                    var caseFormField = new FWTEformField
                    {
                        FieldName = updateEntity.IntegrationFormFields[i].FormFieldName,
                        FieldValue = updateEntity.IntegrationFormFields[i].FormFieldValue ?? string.Empty
                    };
                    formFields[i] = caseFormField;
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
                throw new Exception($"UpdateService: UpdateIntegrationFormField threw an exception while attempting to update EForm Data for case {updateEntity.CaseReference}, Exception: {e}");
            }
        }
    }
}
