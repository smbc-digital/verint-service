using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using verint_service.Models.EForms;
using VerintWebService;

namespace verint_service.Models
{
    public class Case
    {
        public Case()
        {
            this.ID = Guid.NewGuid();
            this.IntegrationFormFields = new List<CustomField>();
            this.CaseFormFields = new List<CustomField>();
            this.CustomAttributes = new List<CustomField>();
            this.CaseReference = string.Empty;
        }

        public Guid ID { get; set; }

        public bool AnonymousSubmission { get; set; }

        public Customer Customer { get; set; }

        public Organisation Organisation { get; set; }

        public string DefinitionName { get; set; }

        public string IntegrationFormName { get; set; }

        public List<CustomField> IntegrationFormFields { get; set; }

        public List<CustomField> CaseFormFields { get; set; }

        public List<Note> Notes { get; set; }

        public string FormName { get; set; }

        public string EventTitle { get; set; }

        public int EventCode { get; set; }

        public string EventId { get; set; }

        public string Status { get; set; }

        public DateTime EventDate { get; set; }

        public bool HasEventCode
        {
            get
            {
                return this.EventCode != 0;
            }
        }

        public string EventFurtherInformation { get; set; }

        public Street Street { get; set; }

        public Address Property { get; set; }

        public string FurtherLocationInformation { get; set; }

        public string Description { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string CaseReference { get; set; }

        public long InteractionReference { get; set; }

        public string Queue { get; set; }

        public string Classification { get; set; }

        public string EnquirySubject { get; set; }

        public string EnquiryReason { get; set; }

        public string EnquiryType { get; set; }

        public List<CustomField> CustomAttributes { get; set; }

        public bool IsSMBCEmployee { get; set; }

        public string SMBCChannel { get; set; }

        public string CaseTitle { get; set; }

        public BaseEform Eform { get; set; }

        public FWTCaseForm CaseForm { get; set; }

        public DestinationSystem DestinationSystem { get; set; }

        public Guid BookingId { get; set; }

        public DateTime BookingDate { get; set; }

        public string BookingTime { get; set; }

        public decimal Price { get; set; }

        public string PaymentRef { get; set; }

        public List<string> LinkCases { get; set; }

        public string SerializeToXML()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Case));
            StringWriter sw = new StringWriter();
            XmlWriter xmlw = XmlWriter.Create(sw);
            xs.Serialize(xmlw, this);
            return sw.ToString();
        }

        public Case DeserializeFromXML(string xml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Case));
            StringReader sr = new StringReader(xml);
            XmlReader xmlr = System.Xml.XmlReader.Create(sr);
            Case crmCase = (Case)xs.Deserialize(xmlr);
            return crmCase;
        }

        public CustomField SearchForIntegrationFormField(string fieldName)
        {
            CustomField result = IntegrationFormFields.Find(
                                delegate (CustomField cf)
                                {
                                    return cf.Name == fieldName;
                                });

            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public CustomField SearchForCaseFormFields(string fieldName)
        {
            CustomField result = CaseFormFields.Find(
                                delegate (CustomField cf)
                                {
                                    return cf.Name == fieldName;
                                });

            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public void SetCustomFieldValue(string key, string value)
        {
            var customField = SearchForCaseFormFields(key);
            if (customField != null)
            {
                customField.Value = value;
            }
        }

        public string CustomAttributeValue(string customAttributeName)
        {
            return this.CustomAttributes.Where(x => x.Name.Equals(customAttributeName)).Select(x => x.Value).SingleOrDefault();
        }

        public void SetCustomAttribute(string customAttributeName, string customAttributeValue)
        {
            var existingAttribute = this.CustomAttributes.Where(x => x.Name.Equals(customAttributeName)).SingleOrDefault();

            if (existingAttribute != null)
                this.CustomAttributes.Remove(existingAttribute);

            this.CustomAttributes.Add(new CustomField(customAttributeName, customAttributeValue));
        }
    }
}
