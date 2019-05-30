using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using Microsoft.Extensions.Options;
using verint_service.Models.Config;
using VerintAuthWebService;

namespace verint_service.Helpers.VerintConnection
{
    internal class RequestInspector : IClientMessageInspector
    {
        private TokenSecurityHeader _securityHeader;
        private readonly VerintConnectionConfiguration _verintConfiguration;

        public RequestInspector(IOptions<VerintConnectionConfiguration> verintConfiguration)
        {
            _verintConfiguration = verintConfiguration.Value;
            GetAuthToken();
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            request.Headers.Add(_securityHeader);

            return null;
        }

        private void GetAuthToken()
        {
            var defaultSize = 67108864;
            var defaultTime = new TimeSpan(0, 10, 0);

            var httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
            {
                Name = "FLWebBinding",
                MaxReceivedMessageSize = defaultSize,
                CloseTimeout = defaultTime,
                OpenTimeout = defaultTime,
                SendTimeout = defaultTime,
                MaxBufferPoolSize = defaultSize,
                MaxBufferSize = defaultSize,
                TextEncoding = Encoding.UTF8,
                TransferMode = TransferMode.Buffered,
            };

            httpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            httpBinding.Security.Mode = BasicHttpSecurityMode.None;

            var authEndpointAddress = new EndpointAddress(_verintConfiguration.AuthConnectionString);
            var auth = new FLAuthWebInterfaceClient(httpBinding, authEndpointAddress);

            using (new OperationContextScope(auth.InnerChannel))
            {
                var currentContext = OperationContext.Current;
                currentContext.OutgoingMessageHeaders.Add(new SecurityHeader(_verintConfiguration.Username, _verintConfiguration.Password));

                auth.verifyAsync().Wait();

                var data = currentContext.IncomingMessageHeaders.GetReaderAtHeader(0);
                data.ReadToFollowing("wsse:BinarySecurityToken");

                _securityHeader = new TokenSecurityHeader(data.ReadElementContentAsString());
            }
        }
    }
}