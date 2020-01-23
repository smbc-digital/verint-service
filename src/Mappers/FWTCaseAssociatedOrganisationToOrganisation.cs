using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Mappers
{
    public static class FwtCaseAssociatedOrganisationToOrganisation
    {
        public static Organisation MapToOrganisation(this FWTOrganisation organisation)
        {
            var mappedOrganisation = new Organisation();

            if (organisation.SocialContacts != null && organisation.SocialContacts.Any())
            {
                mappedOrganisation.SocialContacts = new SocialContact[organisation.SocialContacts.Length];
                for (int i = 0; i < organisation.SocialContacts.Length; i++)
                {
                    mappedOrganisation.SocialContacts[i] = new SocialContact
                    {
                        Value = organisation.SocialContacts[i].SocialID,
                        Type = organisation.SocialContacts[i].SocialChannel
                    };
                }
            }

            if (organisation.Name?[0].FullName != null)
            {
                mappedOrganisation.Name = organisation.Name[0].FullName;
            }

            if (organisation.ContactEmails != null && organisation.ContactEmails.Any())
            {
                mappedOrganisation.Email = organisation.ContactEmails[0].EmailAddress;
            }

            return mappedOrganisation;
        }
    }
}
