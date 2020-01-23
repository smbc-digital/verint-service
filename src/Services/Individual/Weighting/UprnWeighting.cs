using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

public class UprnWeighting : IIndividualWeighting
{
    public int Calculate(FWTIndividual individual, Customer customer)
    {
        if (customer.Address != null && 
            individual.ContactPostals != null && 
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.UPRN) && x.UPRN.Trim() == customer.Address.UPRN.Trim()))
        {
                return 2;
        }

        return 0;
    }
}