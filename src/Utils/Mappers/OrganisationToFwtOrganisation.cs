using System;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Utils.Consts;
using VerintWebService;

namespace verint_service.Utils.Mappers
{
    public static class OrganisationToFwtOrganisation
    {
        public static FWTOrganisation Map(this Organisation organisation)
        {
            var fwtOrganisation = new FWTOrganisation()
            {
                Name = new []
                {
                    new FWTOrganisationName { FullName = organisation.Name.Trim() }
                }
            };          

            if (organisation.Address != null)
            {
                var contactPostal = new FWTContactPostal
                {
                    AddressNumber = organisation.Address.Number,
                    AddressLine = new[] { organisation.Address.AddressLine1, organisation.Address.AddressLine2, organisation.Address.AddressLine3, organisation.Address.City },
                    City = organisation.Address.City,
                    Postcode = organisation.Address.Postcode?.Trim(),
                    Preferred = true,
                };

                if (!string.IsNullOrEmpty(organisation.Address.UPRN))
                {
                    contactPostal.Option = new [] { VerintConstants.UseUprnForAddress,  VerintConstants.IgnoreInvalidUprn };
                    contactPostal.UPRN = organisation.Address.UPRN.Trim();
                }

                fwtOrganisation.ContactPostals = new[] { contactPostal };
            }

            if(!string.IsNullOrEmpty(organisation.Telephone))
            {
                
                fwtOrganisation.ContactPhones = new[] 
                { 
                    new FWTContactPhone
                    {
                        Number = organisation.Telephone?.Trim(),
                        Preferred = true
                    } 
                };
            }
            
            if(!string.IsNullOrEmpty(organisation.Email))
            {
                fwtOrganisation.ContactEmails = new[] 
                { 
                    new FWTContactEmail
                    {
                        EmailAddress = organisation.Email?.Trim(),
                        Preferred = true
                    }
                };
            }

            return fwtOrganisation;
        }
    }
}
