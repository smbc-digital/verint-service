using System.Collections.Generic;
using System.Threading.Tasks;

namespace verint_service.Services.Street
{
    public interface IStreetService
    {
        Task<IEnumerable<Models.Street>> SearchByStreetAsync(string reference);
    }
}
