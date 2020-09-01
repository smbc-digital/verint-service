using StockportGovUK.NetStandard.Models.Models.Verint.VerintOnlineForm;
using System;
using System.Linq;
using System.Threading.Tasks;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Case;
using VOFWebService;

namespace verint_service.Services.VerintOnlineForm
{
    public class VerintOnlineFormService : IVerintOnlineFormService
    {
        private readonly IVOFClient _VOFConnection;
        private readonly ICaseService _caseService;

        public VerintOnlineFormService(IVerintConnection connection, ICaseService caseService)
        {
            _VOFConnection = connection.VOFClient();
            _caseService = caseService;
        }

        public async Task<VerintOnlineFormResponse> CreateVOFCase(VerintOnlineFormRequest model)
        {
            var verintCaseId = await _caseService.Create(model.VerintCase);

            var createVOFResponse = await _VOFConnection.CreateAsync(new CreateRequest
            {
                caseid = verintCaseId,
                name = model.FormName,
                data = new Data
                {
                    formdata = new[]{
                        new Field
                        {
                            name = "le_eventcode",
                            Item = model.VerintCase.EventCode.ToString()
                        }
                    }
                }
            });

            if (string.IsNullOrEmpty(createVOFResponse.CreateResponse.@ref))
                throw new Exception("VerintOnlineFormService.CreateVOFCase: VerintOnlineForms-WebService.CreateAsync failed to create basic case.");
                
            var updateVOFResponse = await _VOFConnection.UpdateAsync(new UpdateRequest
            {
                @ref = createVOFResponse.CreateResponse.@ref,
                caseid = verintCaseId,
                name = model.FormName,
                completeSpecified = true,
                complete = stringBoolean.Y,
                dataupdate = dataupdate.overwrite,
                data = new Data
                {
                    formdata = model.FormData.Select(_ => new Field { name = _.Key, Item = _.Value }).ToArray()
                }
            });

            if (string.IsNullOrEmpty(updateVOFResponse?.UpdateResponse?.status) || !updateVOFResponse.UpdateResponse.status.ToLower().Equals("success"))
                throw new Exception("VerintOnlineFormService.CreateVOFCase: VerintOnlineForms-WebService.UpdateAsync failed to update case details.");

            return new VerintOnlineFormResponse
            {
                VerintCaseReference = verintCaseId,
                VerintOnlineFormReference = createVOFResponse.CreateResponse.@ref
            };
        }

        public async Task<GetResponse1> GetVOFCase(string verintOnlineFormReference)
            => await _VOFConnection.GetAsync(new GetRequest
            {
                @ref = verintOnlineFormReference
            });
    }
}
