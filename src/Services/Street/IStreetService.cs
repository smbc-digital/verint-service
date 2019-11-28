using System.Collections.Generic;
using System.Threading.Tasks;
using verint_service.Controllers;

namespace verint_service.Services.Street
{
    public interface IStreetService
    {
        Task<IEnumerable<StreetX>> SearchByStreetAsync(string reference);
    }
}
