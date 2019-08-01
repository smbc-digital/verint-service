using System.Xml.Serialization;

namespace verint_service.Models
{
    public class CaseEventModel
    {
        public EventCaseType EventType { get; set; } = EventCaseType.None;

        public EventCase EventCase { get; set; }
    }

    public enum EventCaseType
    {
        None,
        PopulatedCloseCaseEvent
    }

    [XmlRoot(ElementName = "classification", Namespace = "http://www.kana.com/lagan/schemas/casemanagement")]
    public class EventClassification
    {
        [XmlElement(ElementName = "subject", Namespace = "http://www.kana.com/lagan/schemas/casemanagement")]
        public string Subject { get; set; }
        [XmlElement(ElementName = "reason", Namespace = "http://www.kana.com/lagan/schemas/casemanagement")]
        public string Reason { get; set; }
        [XmlElement(ElementName = "type", Namespace = "http://www.kana.com/lagan/schemas/casemanagement")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "parent", Namespace = "http://www.kana.com/lagan/schemas/casemanagement")]
    public class Parent
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ref", Namespace = "http://www.kana.com/lagan/schemas/core")]
    public class EventRef
    {
        [XmlAttribute(AttributeName = "core", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Core { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "name", Namespace = "http://www.kana.com/lagan/schemas/core")]
    public class EventName
    {
        [XmlAttribute(AttributeName = "core", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Core { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "associated-with", Namespace = "http://www.kana.com/lagan/schemas/casemanagement")]
    public class Associatedwith
    {
        [XmlElement(ElementName = "ref", Namespace = "http://www.kana.com/lagan/schemas/core")]
        public EventRef Ref { get; set; }
        [XmlElement(ElementName = "name", Namespace = "http://www.kana.com/lagan/schemas/core")]
        public EventName Name { get; set; }
    }

    [XmlRoot(ElementName = "case", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
    public class EventCase
    {
        [XmlElement(ElementName = "classification", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public EventClassification Classification { get; set; }

        [XmlElement(ElementName = "enquiryId", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string EnquiryId { get; set; }

        [XmlElement(ElementName = "title", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string Title { get; set; }

        [XmlElement(ElementName = "description", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string Description { get; set; }

        [XmlElement(ElementName = "status", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string Status { get; set; }

        [XmlElement(ElementName = "priority", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string Priority { get; set; }

        [XmlElement(ElementName = "severity", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string Severity { get; set; }

        [XmlElement(ElementName = "isInternal", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string IsInternal { get; set; }

        [XmlElement(ElementName = "creationDate", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string CreationDate { get; set; }

        [XmlElement(ElementName = "openedDateTime", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string OpenedDateTime { get; set; }

        [XmlElement(ElementName = "closedDateTime", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string ClosedDateTime { get; set; }

        [XmlElement(ElementName = "lastEventDate", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string LastEventDate { get; set; }


        [XmlElement(ElementName = "userAllocationStatus", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string UserAllocationStatus { get; set; }

        [XmlElement(ElementName = "queueId", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string QueueId { get; set; }

        [XmlElement(ElementName = "targetFixDays", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string TargetFixDays { get; set; }

        [XmlElement(ElementName = "targetFixHours", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string TargetFixHours { get; set; }

        [XmlElement(ElementName = "targetFixDateTime", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string TargetFixDateTime { get; set; }

        [XmlElement(ElementName = "sourceId", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string SourceId { get; set; }

        [XmlElement(ElementName = "sourceUserId", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string SourceUserId { get; set; }

        [XmlElement(ElementName = "sourceUserName", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string SourceUserName { get; set; }

        [XmlElement(ElementName = "schedulingStatus", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string SchedulingStatus { get; set; }

        [XmlElement(ElementName = "schedulingPriority", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string SchedulingPriority { get; set; }

        [XmlElement(ElementName = "workFlowId", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string WorkFlowId { get; set; }

        [XmlElement(ElementName = "workFlowStep", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string WorkFlowStep { get; set; }

        [XmlElement(ElementName = "callBackDateTime", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public string CallBackDateTime { get; set; }

        [XmlElement(ElementName = "parent", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public Parent Parent { get; set; }

        [XmlElement(ElementName = "associated-with", Namespace = "http://www.kana.com/lagan/schemas/casemanagement", IsNullable = true)]
        public Associatedwith Associatedwith { get; set; }

        [XmlAttribute(AttributeName = "category")]
        public string Category { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }
}
