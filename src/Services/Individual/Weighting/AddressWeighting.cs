using System;
using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

public class AddressWeighting : IIndividualWeighting
{
    public int Calculate(FWTIndividual individual, Customer customer)
    {
        
        if(customer.Address == null)
        {
            return 0;
        }

        if(individual.ContactPostals == null)
        {
            return 0;
        }

        if(!string.IsNullOrEmpty(customer.Address.UPRN))
        {
            return 0;
        }

        var score = 0; 

            // This is effectively the same as a UPRN match (i.e. House Number and Postcode must match to be given weight)
        if (!string.IsNullOrEmpty(customer.Address.Postcode) &&    
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.Postcode) && string.Equals(x.Postcode.Trim().Replace(" ", string.Empty), customer.Address.Postcode.Trim().Replace(" ", string.Empty), StringComparison.CurrentCultureIgnoreCase)) && 
            !string.IsNullOrEmpty(customer.Address.Number) &&
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressNumber) && string.Equals(x.AddressNumber.Trim(), customer.Address.Number.ToString(), StringComparison.CurrentCultureIgnoreCase))
            )
        {
            score += 2;
        }

        if (!string.IsNullOrEmpty(customer.Address.AddressLine1) &&
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[0]) && string.Equals(x.AddressLine[0].Trim(), customer.Address.AddressLine1.ToString(), StringComparison.CurrentCultureIgnoreCase)))
        {
            score += 1;
        }

        if (!string.IsNullOrEmpty(customer.Address.AddressLine2) &&
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[1]) && string.Equals(x.AddressLine[1].Trim(), customer.Address.AddressLine2.ToString(), StringComparison.CurrentCultureIgnoreCase)))
        {
            score += 1;
        }

        if (!string.IsNullOrEmpty(customer.Address.AddressLine3) &&
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[2]) && string.Equals(x.AddressLine[2].Trim(), customer.Address.AddressLine3.ToString(), StringComparison.CurrentCultureIgnoreCase)))
        {
            score += 1;
        }

        if (!string.IsNullOrEmpty(customer.Address.City) &&
            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.City) && string.Equals(x.City.Trim(), customer.Address.City.ToString(), StringComparison.CurrentCultureIgnoreCase)))
        {
            score += 1;
        }
        
        return score > 2 ? 2 : score;
    }
}