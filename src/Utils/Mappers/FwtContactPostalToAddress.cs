using System.Linq;
using StockportGovUK.NetStandard.Models.Verint;
using VerintWebService;

namespace verint_service.Utils.Mappers
{
    public static class FwtContactPostalToAddress
    {
        public static Address Map(this FWTContactPostal contactPostal)
        {
            return new Address
            {
                UPRN = contactPostal.UPRN,
                AddressLine1 = contactPostal.AddressLine[0],
                AddressLine2 = contactPostal.AddressLine[1],
                AddressLine3 = contactPostal.AddressLine[2],
                City = contactPostal.City,
                Number = contactPostal.AddressNumber,
                Postcode = contactPostal.Postcode,
                PropertyId = contactPostal.PropertyID,
            };
        }
    }
}
