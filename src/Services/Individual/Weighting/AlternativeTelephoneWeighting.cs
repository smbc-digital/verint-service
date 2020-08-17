using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;
namespace verint_service.Services.Individual.Weighting
{
    public class AlternativeTelephoneWeighting : IIndividualWeighting
    {
        public int Calculate(FWTIndividual individual, Customer customer)
        {
            if (!string.IsNullOrEmpty(customer.AlternativeTelephone) && individual.ContactPhones != null)
            {
                if (individual.ContactPhones
                    .Any(x => x.Number.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()
                        == customer.AlternativeTelephone.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()))
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}