using System.ServiceModel.Channels;
using System.Xml;

namespace verint_service.Helpers.VerintConnection
{
    internal class TokenSecurityHeader : MessageHeader
    {
        private readonly string _token;

        public TokenSecurityHeader(string token)
        {
            _token = token;
        }

        public override string Name => "Security";

        public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("wsse", "BinarySecurityToken", Namespace);
            writer.WriteValue(_token);
            writer.WriteEndElement();
        }
    }
}