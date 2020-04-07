using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

public class UprnWeighting : IIndividualWeighting
{
    public int Calculate(FWTIndividual individual, Customer customer)
    {
        if(customer.Address == null || individual.ContactPostals == null)
        {
            return 0;
        }

        if(string.IsNullOrEmpty(customer.Address.UPRN))
        {
            return 0;
        }

        if (individual.ContactPostals.Any(x => x.UPRN == customer.Address.UPRN.Trim()))
        {
                return 2;
        }

        return 0;
    }
}