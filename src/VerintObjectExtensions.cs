using System.Collections.Generic;
using System.Linq;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Extensions
{
    public static class VerintObjectExtensions
    {
        public static void AddAnyRequiredUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            update.AddNameUpdates(individual, customer);
            update.AddAddressUpdates(individual, customer);
            update.AddEmailUpdates(individual, customer);
            update.AddPhoneUpdates(individual, customer);
            update.AddSocialContactUpdates(individual, customer);
        }
        private static void AddNameUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            if (!string.IsNullOrWhiteSpace(customer.Surname) && individual.Name == null)
            {
                var newName = new FWTIndividualName
                {
                    Forename = new[] { customer.Forename + string.Empty },
                    Surname = customer.Surname + string.Empty,
                    Initials = customer.Initials + string.Empty,
                    Title = customer.Title + string.Empty,
                };

                update.Name = new[] { new FWTIndividualNameUpdate { IndividualNameDetails = newName, ListItemUpdateType = "Insert" }};
            }
        }

        private static bool RequiresAddressUpdate(this FWTIndividual individual, Customer customer)
        {
            return (customer.Address != null && individual.ContactPostals == null) ||
                    (customer.Address != null && individual.ContactPostals != null && !individual.ContactPostals.Any(x => x.Postcode.Trim().ToUpper() == customer.Address.Postcode.Trim().ToUpper()))  ||
                    (customer.Address != null && !string.IsNullOrWhiteSpace(customer.Address.UPRN) && individual.ContactPostals != null &&
                        !individual.ContactPostals.Where(x => !string.IsNullOrWhiteSpace(x.UPRN) && x.UPRN.Trim() == customer.Address.UPRN.Trim()).Any());
        }

        private static void AddAddressUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            if (individual.RequiresAddressUpdate(customer)) 
            {
                var newAddress = new FWTContactPostal
                {
                    AddressID = -1,
                    AddressNumber = customer.Address.Number,
                    AddressLine = new[] { customer.Address.AddressLine1, customer.Address.AddressLine2 + string.Empty, customer.Address.AddressLine3 },
                    City = customer.Address.City + string.Empty,
                    Postcode = customer.Address.Postcode + string.Empty,
                    UPRN = customer.Address.UPRN + string.Empty,
                    Preferred = true
                };

                update.ContactPostals = new[] { new FWTContactPostalUpdate { PostalDetails = newAddress, ListItemUpdateType = "Insert" }};
                return;
            }

            if (customer.Address != null && !string.IsNullOrWhiteSpace(customer.Address.UPRN) && individual.ContactPostals != null)
            {
                FWTContactPostal preferredContact = individual.ContactPostals.FirstOrDefault(x => x.Preferred);
                if (preferredContact != null && customer.Address.UPRN.Trim() != preferredContact.UPRN.Trim())
                {
                    var contactPostal = individual.ContactPostals.FirstOrDefault(_ => _.UPRN.Trim() == customer.Address.UPRN.Trim());
                    if(contactPostal != null)
                    {
                        contactPostal.Preferred = true;
                        update.ContactPostals = new[] { new FWTContactPostalUpdate { PostalDetails = contactPostal, ListItemUpdateType = "Update" }};
                    }
                }
            }
        }

        private static bool RequiresEmailUpdate(this FWTIndividual individual, Customer customer)
        {
            return (!string.IsNullOrWhiteSpace(customer.Email) && individual.ContactEmails == null) ||
                    (!string.IsNullOrWhiteSpace(customer.Email) && individual.ContactEmails != null &&
                    !individual.ContactEmails.Where(x => x.EmailAddress.Trim().ToUpper() == customer.Email.Trim().ToUpper()).Any());
        }

        private static void AddEmailUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            if (individual.RequiresEmailUpdate(customer))
            {
                var emailUpdate = new FWTContactEmailUpdate 
                { 
                    EmailDetails = new FWTContactEmail { EmailAddress = customer.Email, Preferred = true }, 
                    ListItemUpdateType = "Insert" 
                };

                update.ContactEmails = new[] { emailUpdate };
            }
    
            if (!string.IsNullOrWhiteSpace(customer.Email) && individual.ContactEmails != null && 
                individual.ContactEmails.Where(x => x.EmailAddress.Trim().ToUpper() == customer.Email.Trim().ToUpper() && x.Preferred == false).Count() > 0)
            {
                var email = individual.ContactEmails.FirstOrDefault(x => x.EmailAddress.Trim().ToUpper() == customer.Email.Trim().ToUpper());
                if (email != null)
                {
                    email.Preferred = true;
                    update.ContactEmails = new[] {  new FWTContactEmailUpdate { EmailDetails = email, ListItemUpdateType = "Update" } };
                }
            }
        }

        private static bool RequiresPhoneUpdate(this FWTIndividual individual, Customer customer)
        {
            
            return (!string.IsNullOrWhiteSpace(customer.Telephone) && individual.ContactPhones == null) || 
                    (!string.IsNullOrWhiteSpace(customer.Telephone) && individual.ContactPhones != null &&
                    !individual.ContactPhones.Where(x => x.Number.Replace(" ", "").Replace("-", "").Equals(customer.Telephone.Replace(" ", "").Replace("-", ""))).Any());
        }

        private static void AddPhoneUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            if (individual.RequiresPhoneUpdate(customer))
            {
                var newPhone = new FWTContactPhone { Number = customer.Telephone, Preferred = true, DeviceType = "unknown" };
                update.ContactPhones = new[] { new FWTContactPhoneUpdate { PhoneDetails = newPhone, ListItemUpdateType = "Insert" } };
                return;
            }

            if ((!string.IsNullOrWhiteSpace(customer.Telephone) && individual.ContactPhones != null) &&
                (individual.ContactPhones.Where(x => x.Number.Replace(" ", "").Replace("-", "").Equals(customer.Telephone.Replace(" ", "").Replace("-", "")) && x.Preferred == false).Count() > 0))
            {
                var updatePhone = individual.ContactPhones.FirstOrDefault(x => x.Number.Replace(" ", "").Replace("-", "").Equals(customer.Telephone.Replace(" ", "").Replace("-", "")));
                if (updatePhone != null)
                {
                    updatePhone.Preferred = true;
                    update.ContactPhones = new[] { new FWTContactPhoneUpdate { PhoneDetails = updatePhone, ListItemUpdateType = "Update" } };
                }
            
            }
        }

        private static void AddSocialContactUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            var newSocialContacts = new List<SocialContact>();
            if (customer.SocialContacts != null && individual.SocialContacts == null)
            {
                newSocialContacts = customer.SocialContacts.ToList();
            }

            if (customer.SocialContacts != null && individual.SocialContacts != null)
            {
                newSocialContacts = customer.SocialContacts.Where(_ => !individual.SocialContacts.Any(x => x.SocialID == _.Value)).ToList();
            }

            if (individual.SocialContacts != null && newSocialContacts.Count > 0)
            {
                var socialContacts = CreateSocialContacts(newSocialContacts.ToArray());
                var socialContactUpdate = new FWTSocialContactUpdate[newSocialContacts.Count];
                

                for (int i = 0; i < socialContacts.Length; i++)
                {
                    socialContactUpdate[i] = new FWTSocialContactUpdate { SocialContacts = socialContacts[i], ListItemUpdateType = "Insert" };
                }
                
                update.SocialContacts = socialContactUpdate;
            }
        }

        private static FWTSocialContact[] CreateSocialContacts(SocialContact[] socialContacts)
        {
            var returnSocialContacts = new FWTSocialContact[socialContacts.Length];
            for (int i = 0; i < socialContacts.Length; i++)
            {
                if (socialContacts[i].Type == "WEBSITE")
                {
                    returnSocialContacts[i] = new FWTSocialContact { SocialID = socialContacts[i].Value, SocialChannel = "FORUMORMESSAGEB", SiteName = socialContacts[i].Value };
                }
                else
                {
                    returnSocialContacts[i] = new FWTSocialContact
                    {
                        SocialID = socialContacts[i].Value,
                        SocialChannel = socialContacts[i].Type,
                    };
                }
            }

            return returnSocialContacts;
        }
    }
}