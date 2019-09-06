using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Services.Create
{
    public interface ICreateService
    {
        Task<CreateCaseResponse> CreateCase(Models.Case crmCase);
    }
}
