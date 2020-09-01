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
                personDetails.ContactPostals = new[] { (customer.Address.Map()) };
            }

            // Setup the telephone contact information.
            var contactPhones = new FWTContactPhone
            {
                Number = customer.Telephone?.Trim(),
                Preferred = true
            };

            personDetails.ContactPhones = new[] { contactPhones };

            // Setup email address information.
            var contactEmails = new FWTContactEmail
            {
                EmailAddress = customer.Email?.Trim(),
                Preferred = true
            };

            personDetails.ContactEmails = new[] { contactEmails };

            return personDetails;
        }
    }
}
