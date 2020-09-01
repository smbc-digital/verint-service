using System;
using StockportGovUK.NetStandard.Models.Verint;
using verint_service.Utils.Consts;
using VerintWebService;

namespace verint_service.Utils.Mappers
{
    public static class AddressToFwtContactPostal
    {
        public static FWTContactPostal Map(this Address address)
        {
            FWTContactPostal contactPostal = null;
            if (address != null)
            {
                contactPostal = new FWTContactPostal{
                    Preferred = true
                };

                if (!string.IsNullOrEmpty(address.UPRN))
                {
                    contactPostal.Option = new [] { VerintConstants.UseUprnForAddress,  VerintConstants.IgnoreInvalidUprn };
                    contactPostal.UPRN = address.UPRN.Trim();
                }
                else{
                    contactPostal.AddressNumber = address.Number;
                    contactPostal.AddressLine = new[] { address.AddressLine1, address.AddressLine2, address.AddressLine3, address.City };
                    contactPostal.City = address.City;
                    contactPostal.Postcode = address.Postcode?.Trim();
                }
            }

            return contactPostal;
        }
    }
}
