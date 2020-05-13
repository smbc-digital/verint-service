using System;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Utils.Consts;
using VerintWebService;

namespace verint_service.Utils.Mappers
{
    public static class CustomerToFwtIndividual
    {
        public static FWTIndividual Map(this Customer customer)
        {
            var contactName = new FWTIndividualName
            {
                Title = customer.Title?.Trim(),
                Forename = new[] { customer.Forename?.Trim() },
                Initials = customer.Initials?.Trim(),
                Surname = customer.Surname?.Trim(),
                Preferred = true
            };

            var personDetails = new FWTIndividual { Name = new[] { contactName } };
            if (customer.DateOfBirth != DateTime.MinValue)
            {
                personDetails.DateOfBirth = customer.DateOfBirth;
                personDetails.DateOfBirthSpecified = true;
            }

            // Setup the address details
            if (customer.Address != null)
            {
                var contactPostal = new FWTContactPostal
                {
                    AddressNumber = customer.Address.Number,
                    AddressLine = new[] { customer.Address.AddressLine1, customer.Address.AddressLine2, customer.Address.AddressLine3, customer.Address.City },
                    City = customer.Address.City,
                    Postcode = customer.Address.Postcode,
                    Preferred = true,
                };

                if (!string.IsNullOrEmpty(customer.Address.UPRN))
                {
                    contactPostal.Option = new [] { VerintConstants.UseUprnForAddress,  VerintConstants.IgnoreInvalidUprn };
                    contactPostal.UPRN = customer.Address.UPRN.Trim();
                }

                personDetails.ContactPostals = new[] { contactPostal };
            }

            // Setup the telephone contact information.
            var contactPhones = new FWTContactPhone
            {
                Number = customer.Telephone,
                Preferred = true
            };

            personDetails.ContactPhones = new[] { contactPhones };

            // Setup email address information.
            var contactEmails = new FWTContactEmail
            {
                EmailAddress = customer.Email,
                Preferred = true
            };

            personDetails.ContactEmails = new[] { contactEmails };

            return personDetails;
        }
    }
}
