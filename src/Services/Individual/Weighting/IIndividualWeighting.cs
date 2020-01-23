using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

public interface IIndividualWeighting
{
    int Calculate(FWTIndividual individual, Customer customer);
}