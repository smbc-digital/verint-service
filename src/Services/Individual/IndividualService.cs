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
using System.Diagnostics;

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
                _logger.LogDebug($"IndividualService.ResolveIndividual: No match - Creating new inidividual - Customer {customer.Surname}");
                return await CreateIndividual(customer);
            }
 
            return individual;
        }

        private async Task<FWTObjectID> CreateIndividual(Customer customer)
        {
            // HACK: Check whether UPRN provided is actually an ID and if so lookup the reals UPRN
            if (customer.Address != null)
            {
                customer.Address.UPRN = await CheckUPRNForId(customer);
            }

            var fwtIndividual = customer.Map();
            var createIndividualResult = await _verintConnection.createIndividualAsync(fwtIndividual);

            return createIndividualResult.FLNewIndividualID;
        }
        
        private async Task<FWTObjectID> FindIndividual(Customer customer)
        {
            _logger.LogDebug($"IndividualService.FindIndividual: *** Starting customer search - {customer.Forename} {customer.Surname}");
            var stopwatch = Stopwatch.StartNew();

            var stopwatchSearchSpecifc = Stopwatch.StartNew();
            FWTObjectID individual = await SearchByEmail(customer);
            stopwatchSearchSpecifc.Stop();
            _logger.LogDebug($"   InteractionService: FindIndividual - Email Time Elapsed (seconds) {stopwatchSearchSpecifc.Elapsed.TotalSeconds}");

            if (individual == null)
            {
                stopwatchSearchSpecifc = Stopwatch.StartNew();
                individual = await SearchByTelephone(customer);
                stopwatchSearchSpecifc.Stop();
                _logger.LogDebug($"   InteractionService: FindIndividual - Telephone Time Elapsed (seconds) {stopwatchSearchSpecifc.Elapsed.TotalSeconds}");
            }            
            
            if (individual == null)
            {
                stopwatchSearchSpecifc = Stopwatch.StartNew();
                individual = await SearchByAddress(customer);
                stopwatchSearchSpecifc.Stop();
                _logger.LogDebug($"   InteractionService: FindIndividual - Address Time Elapsed (seconds) {stopwatchSearchSpecifc.Elapsed.TotalSeconds}");
            }

            if (individual == null)
            {
                stopwatchSearchSpecifc = Stopwatch.StartNew();
                individual = await SearchByName(customer);
                stopwatchSearchSpecifc.Stop();
                _logger.LogDebug($"   InteractionService: FindIndividual - Name Time Elapsed (seconds) {stopwatchSearchSpecifc.Elapsed.TotalSeconds}");
            }

            if (individual == null)
            {
                _logger.LogDebug($"   IndividualService.FindIndividual: No Result found for Customer {customer.Surname}");
            }
            else
            {
                _logger.LogDebug($"   IndividualService.FindIndividual: Result found for Customer {customer.Surname}");
            }

            stopwatch.Stop();
            _logger.LogDebug($"   InteractionService: FindIndividual - Total Time Elapsed (seconds) {stopwatch.Elapsed.TotalSeconds}");
            _logger.LogDebug($"IndividualService.FindIndividual: *** Finished customer search - {customer.Forename} {customer.Surname}");
            return individual;
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
            searchCriteria.SearchType = "individual";
            var matchingIndividuals = await _verintConnection.searchForPartyAsync(searchCriteria);
            _logger.LogDebug($"   IndividualService.SearchIndividuals: Name matchings found for Customer {customer.Surname} = {matchingIndividuals.FWTObjectBriefDetailsList?.Count()}");

            FWTObjectID individual = null;
            if (matchingIndividuals.FWTObjectBriefDetailsList.Any() && matchingIndividuals != null)
            {
                individual =  await GetBestMatchingIndividual(matchingIndividuals.FWTObjectBriefDetailsList, customer);
            }

            return individual;
        }

        private async Task<FWTObjectID> SearchByEmail(Customer customer)
        {
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.EmailAddress = customer.Email.Trim();
                return await SearchIndividuals(searchCriteria, customer);
            }
            
            return null;
        }

        private async Task<FWTObjectID> SearchByTelephone(Customer customer)
        {
            if (!string.IsNullOrWhiteSpace(customer.Telephone))
            {
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.PhoneNumber = customer.Telephone.Trim();
                return await SearchIndividuals(searchCriteria, customer);
            }

            return null;
        }

        private async Task<FWTObjectID> SearchByAddress(Customer customer)
        {
            if (customer.Address != null && !string.IsNullOrWhiteSpace(customer.Address.Postcode)  && !string.IsNullOrWhiteSpace(customer.Address.Number))
            {
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.AddressNumber = customer.Address.Number.Trim();
                searchCriteria.Postcode = customer.Address.Postcode.Trim();

                return await SearchIndividuals(searchCriteria, customer);
            }

            return null;
        }

        private async Task<FWTObjectID> SearchByName(Customer customer)
        {
            return await SearchIndividuals(GetBaseSearchCriteria(customer), customer);
        }

        private async Task<FWTObjectID> GetBestMatchingIndividual(FWTObjectBriefDetails[] individualResults, Customer customer)
        {
            FWTIndividual bestMatch = null;
            FWTObjectID bestMatchingObjectID  = null;
            var bestMatchScore = 0;

            var tasks = new List<Task<retrieveIndividualResponse>>();

            foreach (var individualResult in individualResults)
            {
                tasks.Add(Task.Run(async () =>
                {
                    return await _verintConnection.retrieveIndividualAsync(individualResult.ObjectID);
                }));
            }

            var results = await Task.WhenAll(tasks);

            results.ToList().ForEach((result) =>
            {
                var individual = result.FWTIndividual;
                var score = 0;

                _individualWeightings.ToList().ForEach(_ => score += _.Calculate(individual, customer));

                if (score > bestMatchScore)
                {
                    bestMatchScore = score;
                    bestMatch = individual;
                }
            });

            if (bestMatch != null && bestMatchScore >= 5)
            {
                _logger.LogDebug($"IndividualService.GetBestMatchingIndividual Match Found - Customer: {customer.Surname} Score: {bestMatchScore}");
                await UpdateIndividual(bestMatch, customer);
                bestMatchingObjectID = bestMatch.BriefDetails.ObjectID;
            }

            return bestMatchingObjectID;
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

        public async Task<string> CheckUPRNForId(Customer customer)
        {
            // HACK: Check whether UPRN provided is actually an ID and if so lookup the real UPRN
            // If it's a real ID it shouldn't return a property!
            if(!string.IsNullOrEmpty(customer.Address.UPRN))
            {
                try{
                    var propertyResult = await _propertyService.GetPropertyAsync(customer.Address.UPRN);
                    if(propertyResult != null)
                    {
                        return propertyResult.UPRN;
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"IndividualService.CheckUPRNForId - Exception occured searching for property, assume UPRN {customer.Address.UPRN}", ex);            
                }
            }

            return customer.Address.UPRN;
        }
    }
}