using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
namespace verint_service.Services.Individual.Weighting
{
    public interface IIndividualWeighting
    {
        int Calculate(FWTIndividual individual, Customer customer);
    }
}