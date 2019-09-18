using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages.Internal;
using verint_service.Helpers.VerintConnection;
using verint_service.Models;
using VerintWebService;

namespace verint_service.Services
{
    public interface IIndividualService
    {
        Task<FWTObjectID> ResolveIndividual(Customer customer);
    }

    public class IndividualService : IIndividualService
    {
        private readonly IVerintClient _verintConnection;

        public IndividualService(IVerintConnection verint)
        {
            _verintConnection = verint.Client();
        }

        public async Task<FWTObjectID> ResolveIndividual(Customer customer)
        {
            FWTObjectID individual = await FindIndividual(customer);

            if(individual == null)
            {
                individual = await CreateIndividual(customer);
            }

            return individual;
        }

        private async Task<FWTObjectID> CreateIndividual(Customer customer)
        {
            var contactName = new FWTIndividualName
            {
                Title = customer.Title,
                Forename = new[] { customer.Forename },
                Initials = customer.Initials,
                Surname = customer.Surname,
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
                var contactAddress = new FWTContactPostal
                {
                    AddressNumber = customer.Address.Number,
                    AddressLine = new[] { customer.Address.AddressLine1, customer.Address.AddressLine2, customer.Address.AddressLine3, customer.Address.City },
                    City = customer.Address.City,
                    Postcode = customer.Address.Postcode,
                    Preferred = true,
                };

                if (!string.IsNullOrEmpty(customer.Address.UPRN))
                {
                    contactAddress.UPRN = customer.Address.UPRN.Trim();
                }

                personDetails.ContactPostals = new[] { contactAddress };
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

            var createIndividualResult = await _verintConnection.createIndividualAsync(personDetails);
            return createIndividualResult.FLNewIndividualID;
        }
        
        private async Task<FWTObjectID> FindIndividual(Customer customer)
        {
            FWTObjectID individual = await SearchByEmail(customer);
            if (individual != null)
            {
                return individual;
            }

            individual = await SearchByTelephone(customer);
            if (individual != null)
            {
                return individual;
            }
            
            individual = await SearchByAddress(customer);
            if (individual != null)
            {
                return individual;
            }
            
            return null;
        }

        private FWTPartySearch GetBaseSearchCriteria(Customer customer)
        {
            return new FWTPartySearch(){
                Forename = customer.Forename,
                Name = customer.Surname
            };
        }

        private async Task<FWTObjectID> SearchIndividuals(FWTPartySearch searchCriteria, Customer customer)
        {
            searchCriteria.SearchType = "individual";
            var matchingIndividuals = await _verintConnection.searchForPartyAsync(searchCriteria); 

            if (matchingIndividuals.FWTObjectBriefDetailsList.Any() && matchingIndividuals != null)
            {
                return await GetBestMatchingIndividual(matchingIndividuals.FWTObjectBriefDetailsList, customer);
            }

            return null;
        }

        private async Task<FWTObjectID> SearchByEmail(Customer customer)
        {
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.EmailAddress = customer.Email;
                return await SearchIndividuals(searchCriteria, customer);
            }

            return null;
        }

        private async Task<FWTObjectID> SearchByTelephone(Customer customer)
        {
            // If customer has email assume use this for intial search
            if (!string.IsNullOrWhiteSpace(customer.Telephone))
            {
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.PhoneNumber = customer.Telephone;
                return await SearchIndividuals(searchCriteria, customer);
            }

            return null;
        }

        private async Task<FWTObjectID> SearchByAddress(Customer customer)
        {
            // If customer has email assume use this for intial search
            if (customer.Address != null && !string.IsNullOrWhiteSpace(customer.Address.Postcode))
            {
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.EmailAddress = null;
                searchCriteria.Postcode = customer.Address.Postcode;
                return await SearchIndividuals(searchCriteria, customer);
            }

            return null;
        }

        private async Task<FWTObjectID> GetBestMatchingIndividual(FWTObjectBriefDetails[] individualResults, Customer customer)
        {
            FWTIndividual bestMatch = null;
            var bestMatchScore = 0;

            foreach (var individualResult in individualResults)
            {
                var score = 0;
                var retrieveResult = await _verintConnection.retrieveIndividualAsync(individualResult.ObjectID);
                var individual = retrieveResult.FWTIndividual;
                
                if (individual.ContactEmails != null && individual.ContactEmails.Any(x => x.EmailAddress == customer.Email))
                {
                    score += 3;
                }

                if (individual.Name != null)
                {

                    if (!string.IsNullOrEmpty(customer.Forename) &&
                        individual.Name.Any(x => x.Forename.Any()) &&
                        individual.Name.Any(x => string.Join(" ", x.Forename).ToUpper().Trim().Contains(customer.Forename.ToUpper().Trim())))
                    {
                        score += 1;
                    }

                    if (!string.IsNullOrEmpty(customer.Surname) &&
                        individual.Name.Any(x => !string.IsNullOrEmpty(x.Surname)) &&
                        individual.Name.Any(x => string.Equals(x.Surname.Trim(),
                            customer.Surname.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                    {
                        score += 1;
                    }

                    if (!string.IsNullOrEmpty(customer.Title) &&
                        individual.Name.Any(x => !string.IsNullOrEmpty(x.Title)) &&
                        individual.Name.Any(x =>
                            string.Equals(x.Title.Trim(), customer.Title.Trim(),
                                StringComparison.CurrentCultureIgnoreCase)))
                    {
                        score += 1;
                    }

                    if (!string.IsNullOrEmpty(customer.Initials) &&
                        individual.Name.Any(x => (x.Initials != null
                            ? x.Initials.Trim().ToUpper() == customer.Initials.Trim().ToUpper()
                            : string.Empty == customer.Initials.Trim().ToUpper())))
                    {
                        score += 1;
                    }

                }

                if (!string.IsNullOrEmpty(customer.Telephone) && individual.ContactPhones != null)
                {
                    if (individual.ContactPhones.Any(x => x.Number.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()
                        == customer.Telephone.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()))
                    {
                        score += 1;
                    }
                }

                if (!string.IsNullOrEmpty(customer.AlternativeTelephone) && individual.ContactPhones != null)
                {
                    if (individual.ContactPhones
                        .Any(x => x.Number.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()
                            == customer.AlternativeTelephone.Replace(" ", string.Empty).Replace("-", string.Empty).Trim()))
                    {
                        score += 1;
                    }
                }

                if (customer.Address != null && individual.ContactPostals != null)
                {
                    if (!string.IsNullOrEmpty(customer.Address.UPRN) &&
                        individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.UPRN) && x.UPRN.Trim() == customer.Address.UPRN.Trim()))
                    {
                        score += 3;
                    }

                    if (!string.IsNullOrEmpty(customer.Address.Postcode) &&
                        
                        individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.Postcode) && string.Equals(x.Postcode.Trim().Replace(" ", string.Empty), customer.Address.Postcode.Trim().Replace(" ", string.Empty), StringComparison.CurrentCultureIgnoreCase)))
                    {
                        score += 1;
                    }

                    if (!string.IsNullOrEmpty(customer.Address.Number) &&
                        individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressNumber) && string.Equals(x.AddressNumber.Trim(), customer.Address.Number.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                    {
                        score += 1;
                    }

                    //No Uprn, customer used manual address
                    if (string.IsNullOrEmpty(customer.Address.UPRN))
                    {
                        if (!string.IsNullOrEmpty(customer.Address.AddressLine1) &&
                            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[0]) && string.Equals(x.AddressLine[0].Trim(), customer.Address.AddressLine1.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                        {
                            score += 1;
                        }

                        if (!string.IsNullOrEmpty(customer.Address.AddressLine2) &&
                            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[1]) && string.Equals(x.AddressLine[1].Trim(), customer.Address.AddressLine2.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                        {
                            score += 1;
                        }

                        if (!string.IsNullOrEmpty(customer.Address.AddressLine3) &&
                            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.AddressLine[2]) && string.Equals(x.AddressLine[2].Trim(), customer.Address.AddressLine3.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                        {
                            score += 1;
                        }

                        if (!string.IsNullOrEmpty(customer.Address.City) &&
                            individual.ContactPostals.Any(x => !string.IsNullOrEmpty(x.City) && string.Equals(x.City.Trim(), customer.Address.City.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                        {
                            score += 1;
                        }
                    }
                }

                if (score > bestMatchScore)
                {
                    bestMatchScore = score;
                    bestMatch = individual;
                }
            }

            if (bestMatch != null && bestMatchScore >= 5)
            {
                // UpdateIndividual(bestMatch, customer); - Check whether there is still an update required
                return bestMatch.BriefDetails.ObjectID;
            }
            else
            {
                return null;
            }
        }
    }
}