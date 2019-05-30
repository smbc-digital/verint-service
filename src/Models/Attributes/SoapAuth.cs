using System;
using System.ServiceModel;
using verint_service.Helpers.VerintConnection;
using VerintWebService;

namespace verint_service.Models.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property)]
    internal class SoapAuth: Attribute
    {
        public SoapAuth(FLWebInterfaceClient lagan, string authToken)
        {
            //inject auth token
            var laganOperation = new OperationContextScope(lagan.InnerChannel);
            OperationContext.Current.OutgoingMessageHeaders.Add(new TokenSecurityHeader(authToken));
            //
        }
    }
}
