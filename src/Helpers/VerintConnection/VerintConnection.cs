using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using Microsoft.Extensions.Options;
using verint_service.Models.Config;
using VerintWebService;
using VOFWebService;

namespace verint_service.Helpers.VerintConnection
{
    public class VerintConnection : IVerintConnection
    {
        private FLWebInterfaceClient _client;
        private serviceClient _VOFClient;

        private const int DefaultSize = 67108864;
        private static readonly TimeSpan DefaultTime = new TimeSpan(0, 10, 0);

        private readonly BasicHttpBinding _verintHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
        {
            Name = "FLWebBinding",
            MaxReceivedMessageSize = DefaultSize,
            CloseTimeout = DefaultTime,
            OpenTimeout = DefaultTime,
            SendTimeout = DefaultTime,
            MaxBufferPoolSize = DefaultSize,
            MaxBufferSize = DefaultSize,
            TextEncoding = Encoding.UTF8,
            TransferMode = TransferMode.Buffered,
        };

        private readonly BasicHttpBinding _VOFHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
        {
            Name = "VOFWebBinding",
            MaxReceivedMessageSize = DefaultSize,
            CloseTimeout = DefaultTime,
            OpenTimeout = DefaultTime,
            SendTimeout = DefaultTime,
            MaxBufferPoolSize = DefaultSize,
            MaxBufferSize = DefaultSize,
            TextEncoding = Encoding.UTF8,
            TransferMode = TransferMode.Buffered,
        };

        private readonly VerintConnectionConfiguration _verintConfiguration;
        private readonly IEndpointBehavior _requestBehavior;

        public VerintConnection(IOptions<VerintConnectionConfiguration> verintConfiguration, IEndpointBehavior requestBehavior)
        {
            _verintConfiguration = verintConfiguration.Value;
            _requestBehavior = requestBehavior;
            _verintHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            _verintHttpBinding.Security.Mode = BasicHttpSecurityMode.None;
        }

        public IVerintClient Client()
        {
            if (_client == null)
            {
                var endpointAddress = new EndpointAddress(_verintConfiguration.VerintBaseConnectionString);

                _client = new FLWebInterfaceClient(_verintHttpBinding, endpointAddress);

                _client.Endpoint.EndpointBehaviors.Add(_requestBehavior);
            }

            return _client;
        }

        public IVOFClient VOFClient()
        {
            if (_VOFClient == null)
            {
                var endpointAddress = new EndpointAddress(_verintConfiguration.VerintOnlineFormBaseConnectionString);

                _VOFClient = new serviceClient(_VOFHttpBinding, endpointAddress);

                _VOFClient.Endpoint.EndpointBehaviors.Add(_requestBehavior);
            }

            return _VOFClient;
        }
    }
}