using verint_service.Models;
using VerintWebService;

namespace verint_service.Mappers
{
    public static class FWTCaseAssociatedOrganisationToOrganisation
    {
        public static Organisation MapFrom(FWTOrganisation organisation)
        {
            var mappedOrganisation = new Organisation();

            if (organisation.SocialContacts != null && organisation.SocialContacts.Length > 0)
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

            if (organisation.Name != null && organisation.Name[0].FullName != null)
            {
                mappedOrganisation.Name = organisation.Name[0].FullName;
            }

            if (organisation.ContactEmails != null && organisation.ContactEmails.Length > 0)
                mappedOrganisation.Email = organisation.ContactEmails[0].EmailAddress;

            return mappedOrganisation;
        }
    }
}
