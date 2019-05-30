using System;
using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using verint_service.Mappers;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Services
{
    public class CaseService : ICaseService
    {
        private readonly FLWebInterfaceClient _verintConnection;

        public CaseService(IVerintConnection verint)
        {
            _verintConnection = verint.Client();
        }

        public async Task<Case> GetCase(string caseId)
        {

            if (string.IsNullOrWhiteSpace(caseId))
            {
                throw new Exception("Null or empty references are not allowed");
            }

            var caseRequest = new FWTCaseFullDetailsRequest
            {
                CaseReference = caseId.Trim(),
                Option = new[] { "all" }
            };
            
            var response = await _verintConnection.retrieveCaseDetailsAsync(caseRequest);

            var caseDetails = FWTCaseFullDetailsToCaseMapper.MapFrom(response.FWTCaseFullDetails);

            return caseDetails;
        }
    }
}