using System.ServiceModel.Channels;
using System.Xml;

namespace verint_service.Helpers.VerintConnection
{
    internal class SecurityHeader : MessageHeader
    {
        private readonly string _password, _username;

        public SecurityHeader(string username, string password)
        {
            _password = password;
            _username = username;
        }

        public override string Name => "Security";

        public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("wsse", "UsernameToken", Namespace);
            writer.WriteStartElement("wsse", "Username", Namespace);
            writer.WriteValue(_username);
            writer.WriteEndElement();

            writer.WriteStartElement("wsse", "Password", Namespace);
            writer.WriteValue(_password);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}