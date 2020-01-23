using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

public class EmailWeighting : IIndividualWeighting
{
    public int Calculate(FWTIndividual individual, Customer customer)
    {
        if (individual.ContactEmails == null || 
            individual.ContactEmails.Length == 0 || 
            string.IsNullOrEmpty(customer.Email))
        {
            return 0;
        } 
        
        if (individual.ContactEmails.Any(x => x.EmailAddress == customer.Email))
        {
            return 2;
        } 

        return 0;
    }
}