using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Models.Verint.Lookup;
using VerintWebService;
using verint_service.Helpers.VerintConnection;
using verint_service.Utils.Mappers;
using verint_service.Services.Organisation.Weighting;
using verint_service.Utils.Consts;

namespace verint_service.Services.Organisation
{
    public class OrganisationService : IOrganisationService
    {
        private readonly ILogger<OrganisationService> _logger;
        private readonly IVerintClient _verintConnection;
        private readonly IEnumerable<IOrganisationWeighting> _organisationWeightings;

        public OrganisationService(IVerintConnection verint, IEnumerable<IOrganisationWeighting> organisationWeightings, ILogger<OrganisationService> logger)
        {
            _logger = logger;
            _verintConnection = verint.Client();
            _organisationWeightings = organisationWeightings;
        }

        public async Task<StockportGovUK.NetStandard.Models.Verint.Organisation> GetAsync(string id)
        {
            var objectID = new FWTObjectID
            {
                ObjectReference = new string [] { id },
                ObjectType = VerintConstants.OrganisationObjectType
            };
            
            _logger.LogDebug($"OrganisationService.GetAsync: Retrieve - {id}");
            var result = await _verintConnection.retrieveOrganisationAsync(objectID);
            
            _logger.LogDebug($"OrganisationService.GetAsync: Map - {id}");
            return result.FWTOrganisation.Map();
        }

        public async Task<FWTObjectID> CreateAsync(StockportGovUK.NetStandard.Models.Verint.Organisation organisation)
        {
            var fwtOrganisation = organisation.Map();
            var response = await _verintConnection.createOrganisationAsync(fwtOrganisation);
            return response.FLNewOrganisationID;
        }

        public async Task<FWTObjectID> ResolveAsync(StockportGovUK.NetStandard.Models.Verint.Organisation organisation)
        {
            var organisationObject = await MatchAsync(organisation);
            if(organisationObject != null)
            {
                return organisationObject;    
            }

            _logger.LogDebug($"OrganisationService.ResolveOrganisation: No match - Creating new organisation - {organisation.Name}");
            return await CreateAsync(organisation);
        }

        public async Task<IEnumerable<OrganisationSearchResult>> SearchByNameAsync(string organisationName)
        {
            var orgSearch = new FWTPartySearch
            {
                SearchType = "organisation",
                Name = organisationName
            };

            return await DoOrganisationSearchAsync(orgSearch);
        }

        public async Task<FWTObjectID> MatchAsync(StockportGovUK.NetStandard.Models.Verint.Organisation organisation)
        {
            _logger.LogDebug($"OrganisationService.MatchAsync - Organisation: {organisation.Name}");

            var search = new FWTPartySearch
            {
                SearchType = "organisation",
                Name = organisation.Name
            };

            if(!string.IsNullOrEmpty(organisation.Telephone))
            {
                search.PhoneNumber = organisation.Telephone;
            }

            if(!string.IsNullOrEmpty(organisation.Email))
            {
                search.EmailAddress = organisation.Email;
            }
            
            var searchResults = await _verintConnection.searchForPartyAsync(search);
            _logger.LogDebug($"OrganisationService.MatchAsync - SearchResults: { searchResults.FWTObjectBriefDetailsList.Count() }");

            FWTObjectID matchingOrganisation = null;
            if (searchResults.FWTObjectBriefDetailsList.Any() && searchResults != null)
            {
                matchingOrganisation =  await GetBestMatchingOrganisationAsync(searchResults.FWTObjectBriefDetailsList.Take(50).ToArray(), organisation);
                _logger.LogDebug($"OrganisationService.MatchAsync - Organisation Found: { matchingOrganisation.ObjectReference.First() }");
                return matchingOrganisation;
            }

            _logger.LogDebug($"OrganisationService.MatchAsync - Organisation Not Found: { organisation.Name }");
            return null;
        }

        private async Task<FWTObjectID> GetBestMatchingOrganisationAsync(FWTObjectBriefDetails[] searchResults, StockportGovUK.NetStandard.Models.Verint.Organisation organisation)
        {
            FWTOrganisation bestMatch = null;
            FWTObjectID bestMatchingObjectID  = null;
            var bestMatchScore = 0;

            var tasks = new List<Task<retrieveOrganisationResponse>>();

            foreach (var result in searchResults)
            {
                tasks.Add(Task.Run(async () =>
                {
                    _logger.LogDebug($"OrganisationService.GetBestMatchingOrganisationAsync - Get core Orgnisation Record : { result.ObjectID }");
                    return await _verintConnection.retrieveOrganisationAsync(result.ObjectID);
                }));
            }

            var results = await Task.WhenAll(tasks);

            results.ToList().ForEach((result) =>
            {
                var orgnisationResult = result.FWTOrganisation;
                var score = 0;

                _organisationWeightings.ToList().ForEach(_ => score += _.Calculate(orgnisationResult, organisation));
                _logger.LogDebug($"OrganisationService.GetBestMatchingOrganisationAsync - Organisation: { orgnisationResult.Name } Score: { score }");

                if (score > bestMatchScore)
                {
                    bestMatchScore = score;
                    bestMatch = orgnisationResult;
                }
            });

            if (bestMatch != null && bestMatchScore >= 3)
            {
                _logger.LogDebug($"OrganisationService.GetBestMatchingOrganisation Match Found - Organisation: {bestMatch.Name} Score: {bestMatchScore}");
                // await UpdateIndividual(bestMatch, customer);
                bestMatchingObjectID = bestMatch.BriefDetails.ObjectID;
            }

            _logger.LogDebug($"OrganisationService.GetBestMatchingOrganisation Match Not Found");
            return bestMatchingObjectID;
        }

        private async Task<IEnumerable<OrganisationSearchResult>> DoOrganisationSearchAsync(FWTPartySearch orgSearch)
        {
            var orgSearchResults = await _verintConnection.searchForPartyAsync(orgSearch);
            var orgResults = orgSearchResults.FWTObjectBriefDetailsList.Select(result => new OrganisationSearchResult
            {
                Reference = result.ObjectID.ObjectReference[0],
                Name = result.ObjectDescription,
                Address = result.Details
            });

            return orgResults;
        }
    }
}
