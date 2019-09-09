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

        /**
         * Verint Docs available : http://scnverinttest.stockport.gov.uk:8080/lagan/schema/FLService.wsdl#op.idp167663632
         *
         * TODO: Add functionality to work with existing forms
         */
        public async Task<createCaseResponse> CreateCase(Models.Case crmCase)
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

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
