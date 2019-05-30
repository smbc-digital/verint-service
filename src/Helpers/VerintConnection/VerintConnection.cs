using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using Microsoft.Extensions.Options;
using verint_service.Models.Config;
using VerintWebService;

namespace verint_service.Helpers.VerintConnection
{
    public class VerintConnection : IVerintConnection
    {
        private FLWebInterfaceClient _client;

        private const int DefaultSize = 67108864;
        private static readonly TimeSpan DefaultTime = new TimeSpan(0, 10, 0);

        private readonly BasicHttpBinding _httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
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

        private readonly VerintConnectionConfiguration _verintConfiguration;
        private readonly IEndpointBehavior _requestBehavior;

        public VerintConnection(IOptions<VerintConnectionConfiguration> verintConfiguration, IEndpointBehavior requestBehavior)
        {
            _verintConfiguration = verintConfiguration.Value;
            _requestBehavior = requestBehavior;
            _httpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            _httpBinding.Security.Mode = BasicHttpSecurityMode.None;
        }

        public FLWebInterfaceClient Client()
        {
            if (_client == null)
            {
                var endpointAddress = new EndpointAddress(_verintConfiguration.BaseConnectionString);

                _client = new FLWebInterfaceClient(_httpBinding, endpointAddress);

                _client.Endpoint.EndpointBehaviors.Add(_requestBehavior);
            }

            return _client;
        }
    }
}