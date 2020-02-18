using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Extensions;
using verint_service.Helpers.VerintConnection;
using verint_service.Mappers;
using VerintWebService;

namespace verint_service.Services
{
    public class IndividualService : IIndividualService
    {
        private readonly IVerintClient _verintConnection;

        private readonly IEnumerable<IIndividualWeighting> _individualWeightings;

        public IndividualService(IVerintConnection verint, IEnumerable<IIndividualWeighting> individualWeightings)
        {
            _verintConnection = verint.Client();
            _individualWeightings = individualWeightings;
        }

        public async Task<FWTObjectID> ResolveIndividual(Customer customer)
        {
            FWTObjectID individual = await FindIndividual(customer);
            if(individual == null)
            {
                return await CreateIndividual(customer);
            }

            return individual;
        }

        private async Task<FWTObjectID> CreateIndividual(Customer customer)
        {
            var fwtIndividual = customer.Map();
            var createIndividualResult = await _verintConnection.createIndividualAsync(fwtIndividual);
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
                var retrieveResult = await _verintConnection.retrieveIndividualAsync(individualResult.ObjectID);
                var individual = retrieveResult.FWTIndividual;
                var score = 0;

                _individualWeightings.ToList().ForEach(_ => score += _.Calculate(individual, customer));

                if (score > bestMatchScore)
                {
                    bestMatchScore = score;
                    bestMatch = individual;
                }
            }

            if (bestMatch != null && bestMatchScore >= 5)
            {
                await UpdateIndividual(bestMatch, customer);
                return bestMatch.BriefDetails.ObjectID;
            }
            
            return null;
        }

        public async Task UpdateIndividual(FWTIndividual individual, Customer customer)
        {
            var individualResponse = await _verintConnection.retrieveIndividualAsync(individual.BriefDetails.ObjectID);
            individual = individualResponse.FWTIndividual;
            
            var update = new FWTIndividualUpdate(){
                BriefDetails = individual.BriefDetails
            };
            
            var requiresUpdate = update.AddAnyRequiredUpdates(individual, customer);

            if(requiresUpdate)
            {
                await _verintConnection.updateIndividualAsync(update); 
            } 
        }
    }
}