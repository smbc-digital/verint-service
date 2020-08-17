using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Utils.Mappers
{
    public static class FwtCaseAssociatedOrganisationToOrganisation
    {
        public static Organisation Map(this FWTOrganisation organisation)
        {
            // _logger.LogDebug($"FwtCaseAssociatedOrganisationToOrganisation.Map Start");

            var mappedOrganisation = new Organisation();

            if(organisation.BriefDetails !=null && organisation.BriefDetails.ObjectID.ObjectReference.Any())
            {
                mappedOrganisation.Reference = organisation.BriefDetails.ObjectID.ObjectReference[0];
            }
            

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

            if(organisation.ContactPostals != null && organisation.ContactPostals.Any())
            {
                mappedOrganisation.Address = organisation.ContactPostals.OrderByDescending(_ => _.Preferred).FirstOrDefault().Map();
            }

            if (organisation.Name?[0].FullName != null)
            {
                mappedOrganisation.Name = organisation.Name[0].FullName;
            }

            if (organisation.ContactEmails != null && organisation.ContactEmails.Any())
            {
                mappedOrganisation.Email = organisation.ContactEmails.OrderByDescending(_ => _.Preferred).FirstOrDefault().EmailAddress;
            }

            if (organisation.ContactPhones != null && organisation.ContactPhones.Any())
            {
                mappedOrganisation.Telephone = organisation.ContactPhones.OrderByDescending(_ => _.Preferred).FirstOrDefault().Number;
            }

            return mappedOrganisation;
        }
    }
}
