using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

public class TelephoneWeighting : IIndividualWeighting
{
    public int Calculate(FWTIndividual individual, Customer customer)
    {
        if (!string.IsNullOrEmpty(customer.Telephone) && 
            individual.ContactPhones != null)
        {
            if (individual.ContactPhones.Any(x => x.Number.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()
                == customer.Telephone.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()))
            {
                return 1;
            }
        }

        return 0;
    }
}