using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Helpers.VerintConnection;
using verint_service.Services.Individual.Weighting;
using verint_service.Services.Property;
using verint_service.Utils.Extensions;
using verint_service.Utils.Mappers;
using VerintWebService;

namespace verint_service.Services.Individual
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

        public async Task<FWTObjectID> ResolveAsync(Customer customer)
        {
            // HACK: Check whether UPRN provided is actually an ID and if so lookup the reals UPRN
            if (customer.Address != null)
            {
                customer.Address.UPRN = await _propertyService.CheckUPRNForId(customer.Address);
            }

            if(string.IsNullOrEmpty(customer.Forename) || string.IsNullOrEmpty(customer.Surname))
            {
                return await CreateAsync(customer);
            }

            var individual = await FindAsync(customer);
            if (individual == null)
            {
                _logger.LogDebug($"IndividualService.ResolveIndividual: No match - Creating new individual - Customer {customer.Surname}");

                return await CreateAsync(customer);
            }

            return individual;
        }

        public async Task UpdateIndividual(FWTIndividual individual, Customer customer)
        {
            var update = new FWTIndividualUpdate
            {
                BriefDetails = individual.BriefDetails
            };

            var requiresUpdate = update.AddAnyRequiredUpdates(individual, customer);

            if (requiresUpdate)
                await _verintConnection.updateIndividualAsync(update);
        }

        private async Task<FWTObjectID> CreateAsync(Customer customer)
        {
            // HACK: Check whether UPRN provided is actually an ID and if so lookup the reals UPRN
            if (customer.Address != null)
                customer.Address.UPRN = await _propertyService.CheckUPRNForId(customer.Address);

            var fwtIndividual = customer.Map();
            var createIndividualResult = await _verintConnection.createIndividualAsync(fwtIndividual);

            return createIndividualResult.FLNewIndividualID;
        }

        private async Task<FWTObjectID> FindAsync(Customer customer)
        {
            var individual = await SearchByAllProvidedData(customer);

            if (individual == null)
                individual = await SearchByEmailAsync(customer);

            if (individual == null)
                individual = await SearchByTelephoneAsync(customer);

            if (individual == null)
                individual = await SearchByAddressAsync(customer);

            if (individual == null)
                individual = await SearchByNameAsync(customer);

            _logger.LogDebug(individual == null
                ? $"IndividualService.FindIndividual:{customer.Surname}: No Result found for Customer"
                : $"IndividualService.FindIndividual:{customer.Surname}: Result found for Customer");

            return individual;
        }

        private FWTPartySearch GetBaseSearchCriteria(Customer customer) =>
            new FWTPartySearch
            {
                Forename = customer.Forename.Trim(),
                Name = customer.Surname.Trim(),
                SearchMatch = "equals_ignore_case"
            };

        private async Task<FWTObjectID> SearchAsync(FWTPartySearch searchCriteria, Customer customer)
        {
            searchCriteria.SearchType = "individual";
            var matchingIndividuals = await _verintConnection.searchForPartyAsync(searchCriteria);
            _logger.LogDebug($"IndividualService.SearchIndividuals:{customer.Surname}: {matchingIndividuals.FWTObjectBriefDetailsList?.Count()} name matchings found for Customer");

            FWTObjectID individual = null;
            if (matchingIndividuals.FWTObjectBriefDetailsList.Any() && matchingIndividuals != null)
            {
                individual = await GetBestMatchingAsync(matchingIndividuals.FWTObjectBriefDetailsList.Take(30).ToArray(), customer);
            }

            return individual;
        }

        private async Task<FWTObjectID> SearchByEmailAsync(Customer customer)
        {
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.EmailAddress = customer.Email.Trim();

                return await SearchAsync(searchCriteria, customer);
            }

            return null;
        }

        private async Task<FWTObjectID> SearchByTelephoneAsync(Customer customer)
        {
            if (!string.IsNullOrWhiteSpace(customer.Telephone))
            {
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.PhoneNumber = customer.Telephone.Trim();

                return await SearchAsync(searchCriteria, customer);
            }

            return null;
        }

        private async Task<FWTObjectID> SearchByAddressAsync(Customer customer)
        {
            if (customer.Address != null && !string.IsNullOrWhiteSpace(customer.Address.Postcode) && !string.IsNullOrWhiteSpace(customer.Address.Number))
            {
                var searchCriteria = GetBaseSearchCriteria(customer);
                searchCriteria.AddressNumber = customer.Address.Number.Trim();
                searchCriteria.Postcode = customer.Address.Postcode.Trim();

                return await SearchAsync(searchCriteria, customer);
            }

            return null;
        }

        private async Task<FWTObjectID> SearchByNameAsync(Customer customer) => await SearchAsync(GetBaseSearchCriteria(customer), customer);

        private async Task<FWTObjectID> SearchByAllProvidedData(Customer customer)
        {
            var baseSearchCriteria = GetBaseSearchCriteria(customer);

            if (!string.IsNullOrWhiteSpace(customer.Email))
                baseSearchCriteria.EmailAddress = customer.Email.Trim();

            if (!string.IsNullOrWhiteSpace(customer.Telephone))
                baseSearchCriteria.PhoneNumber = customer.Telephone.Trim();

            if (customer.Address != null && !string.IsNullOrWhiteSpace(customer.Address.Postcode) && !string.IsNullOrWhiteSpace(customer.Address.Number))
            {
                baseSearchCriteria.AddressNumber = customer.Address.Number.Trim();
                baseSearchCriteria.Postcode = customer.Address.Postcode.Trim();
            }

            return await SearchAsync(baseSearchCriteria, customer);
        }
        
        private async Task<FWTObjectID> GetBestMatchingAsync(FWTObjectBriefDetails[] individualResults, Customer customer)
        {
            FWTIndividual bestMatch = null;
            FWTObjectID bestMatchingObjectID = null;
            var bestMatchScore = 0;

            var tasks = new List<Task<retrieveIndividualResponse>>();
            _logger.LogDebug($"IndividualService.GetBestMatchingAsync:{customer.Surname}: Retrieving results for {individualResults.Count()} results");

            foreach (var individualResult in individualResults)
            {
                tasks.Add(Task.Run(async () =>
                {
                    _logger.LogDebug($"IndividualService.GetBestMatchingAsync:{customer.Surname}: Retrievingindividual");
                    return await _verintConnection.retrieveIndividualAsync(individualResult.ObjectID);
                }));
            }

            var results = await Task.WhenAll(tasks);
            _logger.LogDebug($"IndividualService.GetBestMatchingAsync:{customer.Surname}: Retrieved all search result objects");

            results.ToList().ForEach(result =>
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

            if (bestMatch != null && bestMatchScore >= 1)
            {
                _logger.LogDebug($"IndividualService.GetBestMatchingAsync:{customer.Surname}: Match found, score: {bestMatchScore}");
                await UpdateIndividual(bestMatch, customer);
                bestMatchingObjectID = bestMatch.BriefDetails.ObjectID;
            }

            return bestMatchingObjectID;
        }
    }
}