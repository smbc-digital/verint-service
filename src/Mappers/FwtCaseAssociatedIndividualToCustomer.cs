using System.Linq;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Mappers
{
    public static class FwtCaseAssociatedIndividualToCustomer
    {
        public static Customer MapToCustomer(this FWTIndividual individual)
        {
            var mappedCustomer = new Customer
            {
                Forename = individual.Name[0].Forename[0],
                Surname = individual.Name[0].Surname
            };

            if (individual.ContactPostals != null && individual.ContactPostals.Any())
            {
                var address = individual.ContactPostals.Any(_ => _.Preferred)
                    ? individual.ContactPostals.FirstOrDefault(_ => _.Preferred)
                    : individual.ContactPostals[0];

                if (address != null)
                {
                    mappedCustomer.Address = new Address
                    {
                        UPRN = address.UPRN,
                        AddressLine1 = address.AddressLine[0],
                        AddressLine2 = address.AddressLine[1],
                        AddressLine3 = address.AddressLine[2],
                        City = address.City,
                        Number = address.AddressNumber,
                        Postcode = address.Postcode,
                        PropertyId = address.PropertyID,
                    };
                }
            }

            if (individual.SocialContacts != null && individual.SocialContacts.Any())
            {
                mappedCustomer.SocialContacts = new SocialContact[individual.SocialContacts.Length];
                for (var i = 0; i < individual.SocialContacts.Length; i++)
                {
                    mappedCustomer.SocialContacts[i] = new SocialContact
                    {
                        Value = individual.SocialContacts[i].SocialID,
                        Type = individual.SocialContacts[i].SocialChannel
                    };
                }
            }


            if (individual.ContactEmails != null && individual.ContactEmails.Any())
            {
                mappedCustomer.Email = individual.ContactEmails.Any(_ => _.Preferred)
                    ? individual.ContactEmails.FirstOrDefault(_ => _.Preferred)?.EmailAddress
                    : individual.ContactEmails[0].EmailAddress;
            }

            return mappedCustomer;
        }
    }
}
