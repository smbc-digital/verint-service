using System;
using System.Collections.Generic;
using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Extensions
{
    public static class VerintObjectExtensions
    {
        public static bool AddAnyRequiredUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            var nameUpdate = update.AddNameUpdates(individual, customer);
            var addressUpdate = update.AddAddressUpdates(individual, customer);
            var emailUpdate = update.AddEmailUpdates(individual, customer);
            var phoneUpdate = update.AddPhoneUpdates(individual, customer);
            var socialUpdate = update.AddSocialContactUpdates(individual, customer);
            var dobUpdate = update.AddDateOfBirth(individual, customer);

            return nameUpdate || addressUpdate || emailUpdate || phoneUpdate || socialUpdate || dobUpdate;

        }
        private static bool AddDateOfBirth(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            if(customer.DateOfBirth != null && customer.DateOfBirth != DateTime.MinValue &&  !individual.DateOfBirthSpecified)
            {
                update.DateOfBirthUpdate = new FWTDateOfBirthUpdate
                {
                    DateOfBirth = customer.DateOfBirth,
                    DateOfBirthSpecified = true,
                    UpdateType = "Insert"
                };

                return true;
            }

            return false;
        }

        private static bool AddNameUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
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
                return true;
            }

            return false;
        }

        private static bool RequiresAddressUpdate(this FWTIndividual individual, Customer customer)
        {
            if(customer.Address == null)
            {
                return false;
            }

            if (individual.ContactPostals == null)
            {
                return true;
            }

            if (individual.ContactPostals != null 
                && !string.IsNullOrEmpty(customer.Address.Postcode)
                && !individual.ContactPostals.Any(x =>
                    !string.IsNullOrEmpty(x.Postcode) &&
                    x.Postcode.Trim().ToUpper() == customer.Address.Postcode.Trim().ToUpper()))
            {
                return true;
            }

            var foundAddress = individual.ContactPostals.Where(x => x.Postcode.Trim().ToUpper() == customer.Address.Postcode.Trim().ToUpper());
            
            if (foundAddress != null && 
                !foundAddress.Any(_ => _.AddressLine[0] == customer.Address.AddressLine1 && _.AddressLine[1] == (string.IsNullOrEmpty(customer.Address.AddressLine2) ? "" : customer.Address.AddressLine2) && _.City == customer.Address.City))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(customer.Address.UPRN) && individual.ContactPostals != null &&
                        !individual.ContactPostals.Where(x => !string.IsNullOrWhiteSpace(x.UPRN) && x.UPRN.Trim() == customer.Address.UPRN.Trim()).Any())
            {
                return true;
            }

            return false;
        }

        private static bool AddAddressUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            if (customer.Address == null)
            {
                return false;
            }

            if (individual.RequiresAddressUpdate(customer)) 
            {
                var newContactPostal = new FWTContactPostal
                {
                    AddressID = -1,
                    AddressNumber = customer.Address.Number,
                    AddressLine = new[] { customer.Address.AddressLine1, customer.Address.AddressLine2, customer.Address.AddressLine3, customer.Address.City },
                    City = customer.Address.City,
                    Postcode = customer.Address.Postcode,
                    Preferred = true
                };

                if (!string.IsNullOrEmpty(customer.Address.UPRN))
                {
                    newContactPostal.Option = new [] { Common.UseUprnForAddress,  Common.IgnoreInvalidUprn };
                    newContactPostal.UPRN = customer.Address.UPRN.Trim();
                }

                update.ContactPostals = new[] { new FWTContactPostalUpdate { PostalDetails = newContactPostal, ListItemUpdateType = "Insert" }};
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(customer.Address.UPRN) && individual.ContactPostals != null)
            {
                var preferredContact = individual.ContactPostals.FirstOrDefault(x => x.Preferred);
                if (preferredContact != null && customer.Address.UPRN.Trim() != preferredContact.UPRN.Trim())
                {
                    var contactPostal = individual.ContactPostals.FirstOrDefault(_ => _.UPRN.Trim() == customer.Address.UPRN.Trim());
                    if(contactPostal != null)
                    {
                        contactPostal.Preferred = true;
                        update.ContactPostals = new[] { new FWTContactPostalUpdate { PostalDetails = contactPostal, ListItemUpdateType = "Update" }};
                        return true;
                    }
                }
            }

            return false;
        }


        private static bool HasMatchingAddresses(this FWTContactEmail[] contactEmails, Customer customer)
        {            
            return contactEmails.Any(x => x.EmailAddress.Trim().ToUpper() == customer.Email.Trim().ToUpper());
        }

        private static bool HasMatchingPreferredAddresses(this FWTContactEmail[] contactEmails, Customer customer)
        {            
            return contactEmails.Any(x => x.EmailAddress.Trim().ToUpper() == customer.Email.Trim().ToUpper() && x.Preferred == true);
        }

        private static bool RequiresNewEmailUpdate(this FWTIndividual individual, Customer customer)
        {            
            if(individual.ContactEmails == null)
            {
                return true;
            }

            return !individual.ContactEmails.HasMatchingAddresses(customer);
        }

        private static bool RequiresPreferredEmailUpdate(this FWTIndividual individual, Customer customer)
        {
            if(individual.ContactEmails == null)
            {
                return false;
            }

            return !individual.ContactEmails.HasMatchingPreferredAddresses(customer);
        }

        private static bool AddEmailUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            if(string.IsNullOrWhiteSpace(customer.Email))
            {
                return false;
            }

            if (individual.RequiresNewEmailUpdate(customer))
            {
                update.ContactEmails = new[] { new FWTContactEmailUpdate {  EmailDetails = new FWTContactEmail { EmailAddress = customer.Email, Preferred = true }, ListItemUpdateType = "Insert" } };
                return true;
            }
            else if (individual.RequiresPreferredEmailUpdate(customer))
            {
                var email = individual.ContactEmails.First(x => x.EmailAddress.Trim().ToUpper() == customer.Email.Trim().ToUpper());
                email.Preferred = true;
                update.ContactEmails = new[] {  new FWTContactEmailUpdate { EmailDetails = email, ListItemUpdateType = "Update" } };
                return true;
            }

            return false;
        }

        private static bool RequiresPhoneUpdate(this FWTIndividual individual, Customer customer)
        {
            
            return (!string.IsNullOrWhiteSpace(customer.Telephone) && individual.ContactPhones == null) || 
                    (!string.IsNullOrWhiteSpace(customer.Telephone) && individual.ContactPhones != null &&
                    !individual.ContactPhones.Where(x => x.Number.Replace(" ", "").Replace("-", "").Equals(customer.Telephone.Replace(" ", "").Replace("-", ""))).Any());
        }

        private static bool AddPhoneUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
        {
            if (individual.RequiresPhoneUpdate(customer))
            {
                var newPhone = new FWTContactPhone { Number = customer.Telephone, Preferred = true, DeviceType = "unknown" };
                update.ContactPhones = new[] { new FWTContactPhoneUpdate { PhoneDetails = newPhone, ListItemUpdateType = "Insert" } };
                return true;
            }

            if ((!string.IsNullOrWhiteSpace(customer.Telephone) && individual.ContactPhones != null) &&
                (individual.ContactPhones.Where(x => x.Number.Replace(" ", "").Replace("-", "").Equals(customer.Telephone.Replace(" ", "").Replace("-", "")) && x.Preferred == false).Count() > 0))
            {
                var updatePhone = individual.ContactPhones.FirstOrDefault(x => x.Number.Replace(" ", "").Replace("-", "").Equals(customer.Telephone.Replace(" ", "").Replace("-", "")));
                if (updatePhone != null)
                {
                    updatePhone.Preferred = true;
                    update.ContactPhones = new[] { new FWTContactPhoneUpdate { PhoneDetails = updatePhone, ListItemUpdateType = "Update" } };
                    return true;
                }
            }

            return false;
        }

        private static bool AddSocialContactUpdates(this FWTIndividualUpdate update, FWTIndividual individual, Customer customer)
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
                    return true;
                }
                
                update.SocialContacts = socialContactUpdate;
            }

            return false;
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