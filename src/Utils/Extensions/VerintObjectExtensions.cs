﻿using System;
using System.Collections.Generic;
using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Utils.Consts;
using VerintWebService;

namespace verint_service.Utils.Extensions
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

            if ((individual.ContactPostals == null || !individual.ContactPostals.Any()))
            {    
                return true;
            }

            //Check whether the address with the UPRN already exists  contact postals
            if(!string.IsNullOrEmpty(customer.Address.UPRN) &&
                individual.ContactPostals.Any(contactPostal => !string.IsNullOrEmpty(contactPostal.UPRN) 
                                                                && contactPostal.UPRN == customer.Address.UPRN))
                    return false;

            if(!string.IsNullOrWhiteSpace(customer.Address.Postcode))
            {
                if (!individual.ContactPostals.Any(_ =>
                        !string.IsNullOrEmpty(_.Postcode) &&
                        _.Postcode.Trim().ToUpper() == customer.Address.Postcode.Trim().ToUpper()))
                {
                    return true;
                }

                var foundAddresses = individual.ContactPostals.Where(_ => !string.IsNullOrEmpty(_.Postcode) 
                                                                        && _.Postcode.Trim().ToUpper() == customer.Address.Postcode.Trim().ToUpper());
                
                if (foundAddresses != null && 
                    !foundAddresses.Any(_ => (!string.IsNullOrEmpty(customer.Address.AddressLine1) && _.AddressLine[0] == customer.Address.AddressLine1)
                                            && (!string.IsNullOrEmpty(customer.Address.AddressLine2)  && _.AddressLine[1] == customer.Address.AddressLine2)
                                            && (!string.IsNullOrEmpty(customer.Address.City) && _.City == customer.Address.City)))
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
                var newContactPostal = new FWTContactPostal();
                newContactPostal.Preferred = true;
                if (!string.IsNullOrEmpty(customer.Address.UPRN))
                {
                    newContactPostal.Option = new [] { VerintConstants.UseUprnForAddress,  VerintConstants.IgnoreInvalidUprn };
                    newContactPostal.UPRN = customer.Address.UPRN.Trim();
                }
                else
                {
                    newContactPostal.AddressNumber = customer.Address.Number;
                    newContactPostal.AddressLine = new[] { customer.Address.AddressLine1, customer.Address.AddressLine2, customer.Address.AddressLine3, customer.Address.City };
                    newContactPostal.City = customer.Address.City;
                    newContactPostal.Postcode = customer.Address.Postcode;
                }

                update.ContactPostals = new[] { new FWTContactPostalUpdate { PostalDetails = newContactPostal, ListItemUpdateType = "Insert" }};
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(customer.Address.UPRN) && individual.ContactPostals != null)
            {
                var preferredContact = individual.ContactPostals.FirstOrDefault(x => x.Preferred);

                if (preferredContact != null)
                {
                    if(string.IsNullOrEmpty(preferredContact.UPRN))
                    {
                        return false;
                    }

                    if ( customer.Address.UPRN.Trim() != preferredContact.UPRN.Trim())
                    {
                        var contactPostal = individual.ContactPostals.Where(_ => _.UPRN != null).FirstOrDefault(_ => _.UPRN.Trim() == customer.Address.UPRN.Trim());
                        if(contactPostal != null)
                        {
                            contactPostal.Preferred = true;
                            update.ContactPostals = new[] { new FWTContactPostalUpdate { PostalDetails = contactPostal, ListItemUpdateType = "Update" }};
                            return true;
                        }
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