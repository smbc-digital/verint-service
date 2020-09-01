using StockportGovUK.NetStandard.Models.Verint;
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
                    new FWTOrganisationName { 
                        FullName = organisation.Name.Trim(),
                        Preferred = true
                    }
                }
            };          

            if (organisation.Address != null)
            {
                fwtOrganisation.ContactPostals = new[] { organisation.Address.Map() };
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
