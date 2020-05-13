using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Property;
using verint_service.Utils.Extensions;
using verint_service.Utils.Mappers;
using VerintWebService;
using System;

namespace verint_service.Services
{
    public class IndividualService : IIndividualService
    {
        private readonly IVerintClient _verintConnection;

        private readonly IEnumerable<IIndividualWeighting> _individualWeightings;

        private readonly IPropertyService _propertyService;

        private readonly ILogger<IndividualService> _logger;


        public IndividualService(IVerintConnection verint, IEnumerable<IIndividualWeighting> individualWeightings, IPropertyService propertyService, ILogger<IndividualService> logger)
        {
            _verintConnection = verint.Client();
            _individualWeightings = individualWeightings;
            _propertyService = propertyService;
            _logger = logger;
        }

        public async Task<FWTObjectID> ResolveIndividual(Customer customer)
        {
            // HACK: Check whether UPRN provided is actually an ID and if so lookup the reals UPRN
            if (customer.Address != null)
            {
                customer.Address.UPRN = await CheckUPRNForId(customer);
            }

            FWTObjectID individual = await FindIndividual(customer);
            if(individual == null)
            {
                _logger.LogInformation($"IndividualService.ResolveIndividual: No match - Creating new inidividual - Customer {customer.Surname}");
                return await CreateIndividual(customer);
            }
 
            return individual;
        }

        private async Task<FWTObjectID> CreateIndividual(Customer customer)
        {
            _logger.LogInformation($"IndividualService.CreateIndividual -  Customer {customer.Surname}");
            var fwtIndividual = customer.Map();

            var createIndividualResult = await _verintConnection.createIndividualAsync(fwtIndividual);
            return createIndividualResult.FLNewIndividualID;
        }
        
        private async Task<FWTObjectID> FindIndividual(Customer customer)
        {
            _logger.LogInformation($"IndividualService.FindIndividual: Searching by Email - Customer {customer.Surname}");
            FWTObjectID individual = await SearchByEmail(customer);
            if (individual != null)
            {
                _logger.LogInformation($"IndividualService.FindIndividual: Searching by Email - Result found for Customer {customer.Surname}");
                return individual;
            }

            _logger.LogInformation($"IndividualService.FindIndividual: Searching by Telephone - Customer {customer.Surname}");
            individual = await SearchByTelephone(customer);
            if (individual != null)
            {
                _logger.LogInformation($"IndividualService.FindIndividual: Searching by Telephone - Result found for Customer {customer.Surname}");
                return individual;
            }
            
            _logger.LogInformation($"IndividualService.FindIndividual: Searching by Address - Customer {customer.Surname}");
            individual = await SearchByAddress(customer);
            if (individual != null)
            {
                _logger.LogInformation($"IndividualService.FindIndividual: Searching by Address - Result found for Customer {customer.Surname}");
                return individual;
            }
            
            _logger.LogInformation($"IndividualService.FindIndividual: No Result found for Customer {customer.Surname}");
            return null;
        }

        private FWTPartySearch GetBaseSearchCriteria(Customer customer)
        {
            return new FWTPartySearch(){
                Forename = customer.Forename.Trim(),
                Name = customer.Surname.Trim()
            };
        }

        private async Task<FWTObjectID> SearchIndividuals(FWTPartySearch searchCriteria, Customer customer)
        {
            _logger.LogInformation($"IndividualService.SearchIndividuals: Customer - {customer.Surname}");
            searchCriteria.SearchType = "individual";
            var matchingIndividuals = await _verintConnection.searchForPartyAsync(searchCriteria); 

            if (matchingIndividuals.FWTObjectBriefDetailsList.Any() && matchingIndividuals != null)
            {
                return await GetBestMatchingIndividual(matchingIndividuals.FWTObjectBriefDetailsList, customer);
            }

            _logger.LogInformation($"IndividualService.SearchIndividuals: No Results");
            return null;
        }

        private async Task<FWTObjectID> SearchByEmail(Customer customer)
        {
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                _logger.LogInformation($"IndividualService.SearchByEmail: Searching by Email - {customer.Email}");
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.EmailAddress = customer.Email;
                return await SearchIndividuals(searchCriteria, customer);
            }
            
            _logger.LogInformation($"IndividualService.SearchByEmail: Searching by Email, Email null");
            return null;
        }

        private async Task<FWTObjectID> SearchByTelephone(Customer customer)
        {
            if (!string.IsNullOrWhiteSpace(customer.Telephone))
            {
                _logger.LogInformation($"IndividualService.SearchByEmail: Searching by Telephone - {customer.Telephone}");
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.PhoneNumber = customer.Telephone;
                return await SearchIndividuals(searchCriteria, customer);
            }

            _logger.LogInformation($"IndividualService.SearchByEmail: Searching by Telephone, Telephone null");
            return null;
        }

        private async Task<FWTObjectID> SearchByAddress(Customer customer)
        {
            if (customer.Address != null && !string.IsNullOrWhiteSpace(customer.Address.Postcode))
            {
                _logger.LogInformation($"IndividualService.SearchByEmail: Searching by Address - {customer.Address.Postcode}");
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.EmailAddress = null;
                searchCriteria.Postcode = customer.Address.Postcode;
                return await SearchIndividuals(searchCriteria, customer);
            }

            _logger.LogInformation($"IndividualService.SearchByEmail: Searching by Address, Address null");
            return null;
        }

        private async Task<FWTObjectID> GetBestMatchingIndividual(FWTObjectBriefDetails[] individualResults, Customer customer)
        {
            _logger.LogInformation($"IndividualService.GetBestMatchingIndividual Attempting to match customer: {customer.Surname}");

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
                _logger.LogInformation($"IndividualService.GetBestMatchingIndividual Match Found - Customer: {customer.Surname} Score: {bestMatchScore}");
                await UpdateIndividual(bestMatch, customer);
                return bestMatch.BriefDetails.ObjectID;
            }
            
            _logger.LogInformation($"IndividualService.GetBestMatchingIndividual Match Not Found - Customer: {customer.Surname} Score: {bestMatchScore}");
            return null;
        }

        public async Task UpdateIndividual(FWTIndividual individual, Customer customer)
        {
            _logger.LogInformation($"IndividualService.UpdateIndividual - Updating Customer: {customer.Surname}");

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

        public async Task<string> CheckUPRNForId(Customer customer)
        {
            // HACK: Check whether UPRN provided is actually an ID and if so lookup the real UPRN
            // If it's a real ID it shouldn't return a property!
            if(!string.IsNullOrEmpty(customer.Address.UPRN))
            {
                _logger.LogWarning($"IndividualService.CheckUPRNForId - Customer has uprn {customer.Address.UPRN}");

                try{
                    var propertyResult = await _propertyService.GetPropertyAsync(customer.Address.UPRN);
                    if(propertyResult != null)
                    {
                        _logger.LogWarning($"IndividualService.CheckUPRNForId - Returning propertyResult.UPRN: {propertyResult.UPRN}, {propertyResult.Description}");
                        return propertyResult.UPRN;
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"IndividualService.CheckUPRNForId - Exception occured searching for property, assume UPRN {customer.Address.UPRN}", ex);            
                }
            }

            _logger.LogWarning($"IndividualService.CheckUPRNForId - Return original uprn {customer.Address.UPRN}");            
            return customer.Address.UPRN;
        }
    }
}