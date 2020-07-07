using System.Threading.Tasks;

namespace VOFWebService
{
    /*
     * Add methods that are contained within Reference.cs:serviceClient to use within service.
     * This is used to mock the client within unit testing project.
     */
    public interface IVOFClient
    {
        Task<CreateResponse1> CreateAsync(CreateRequest CreateRequest);

        Task<UpdateResponse1> UpdateAsync(UpdateRequest request);

        Task<GetResponse1> GetAsync(GetRequest GetRequest);
    }

    public partial class serviceClient : IVOFClient
    {

    }
}