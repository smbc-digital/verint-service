using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VerintWebService;

namespace verint_service.Services.Create
{
    public interface ICreateService
    {
        Task<createCaseResponse> CreateCase(Models.Case crmCase);
    }
}
