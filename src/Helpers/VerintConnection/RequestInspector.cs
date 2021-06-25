using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using verint_service.Models.Config;
using VerintAuthWebService;

namespace verint_service.Helpers.VerintConnection
{
    internal class RequestInspector : IClientMessageInspector
    {
        private readonly VerintConnectionConfiguration _verintConfiguration;

        public RequestInspector(IOptions<VerintConnectionConfiguration> verintConfiguration)
        {
            _verintConfiguration = verintConfiguration.Value;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (!reply.IsFault)
            {
                return;
            }

            var xDocument = XDocument.Parse(reply.ToString());
            var elements = xDocument.Descendants("detail")?.FirstOrDefault()?.Descendants()?.FirstOrDefault();

            if (elements == null)
            {
                throw new Exception($"RequestInspector:AfterReceiveReply: Unable to parse XML. {reply}");
            }

            var errorMessage = elements.Element("ErrorMessage")?.Value;
            var errorCode = elements.Element("ErrorCode")?.Value;
            var name = elements.Element("Name")?.Value;
            var additionalInfo = elements.Element("AdditionalInfo")?.Value;

            throw new Exception($"RequestInspector:AfterReceiveReply: Verint Exception. Error message: {errorMessage}, Error code: {name}:{errorCode}, Additional info: {additionalInfo}, XML: {reply.ToString()}");
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var securityHeader = GetAuthToken();

            request.Headers.Add(securityHeader);

            return null;
        }

        private TokenSecurityHeader GetAuthToken()
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

                return new TokenSecurityHeader(data.ReadElementContentAsString());
            }
        }
    }
}