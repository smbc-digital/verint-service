using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages.Internal;
using verint_service.Helpers.VerintConnection;
using verint_service.Models;
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

        public async Task<CreateCaseResponse> CreateCase(Models.Case crmCase)
        {
            var caseDetails = new FWTCaseCreate
            {
                ClassificationEventCode = crmCase.EventCode,
                Title = crmCase.EventTitle,
                Description = crmCase.Description
            };

            try
            {
                var result = await _verintConnection.createCaseAsync(caseDetails);
                var response = new CreateCaseResponse
                {
                    CaseId = result.CaseReference
                };
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
