using System;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

public class DateOfBirthWeighting : IIndividualWeighting
{
    public int Calculate(FWTIndividual individual, Customer customer)
    {
        var score  = 0;
        if(customer.DateOfBirth != null && customer.DateOfBirth != DateTime.MinValue)
        {
            if(individual.DateOfBirthSpecified && individual.DateOfBirth == customer.DateOfBirth)
            {
                score +=2;
            }
            else if(individual.DateOfBirthSpecified && individual.DateOfBirth != customer.DateOfBirth)
            {
                score -=10;
            }
            
        }
        return score;
    }
}