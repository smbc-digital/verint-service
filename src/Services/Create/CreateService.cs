using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages.Internal;
using verint_service.Helpers.VerintConnection;
using VerintWebService;

namespace verint_service.Services.Create
{
    public class CreateService : ICreateService
    {
        private readonly IVerintClient _verintConnection;

        public CreateService(IVerintConnection verint)
        {
            _verintConnection = verint.Client();
        }

        public async Task<createCaseResponse> CreateCase(Models.Case crmCase)
        {
            var caseDetails = new FWTCaseCreate
            {
                ClassificationEventCode = crmCase.EventCode,
                Title = crmCase.EventTitle,
                Description = crmCase.Description
            };

            //if (crmCase.CaseForm == null && !string.IsNullOrEmpty(crmCase.FormName))
            //{
            //    crmCase.CaseForm = CreateCaseForm(crmCase);
            //}

            //caseDetails.Form = crmCase.CaseForm;

            try
            {
                return await _verintConnection.createCaseAsync(caseDetails);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
