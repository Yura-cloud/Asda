using System;
using System.Xml.Serialization;

namespace Asda.Integration.Domain.Models.Business.Cancellation
{
    [XmlRoot(ElementName = "cXML")]
    public class Cancellation
    {
        [XmlElement(ElementName = "Header")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "Request")]
        public Request Request { get; set; }

        [XmlAttribute(AttributeName = "lang")]
        public string Lang { get; set; }

        [XmlAttribute(AttributeName = "payloadID")]
        public string PayloadID { get; set; }

        [XmlAttribute(AttributeName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Credential")]
    public class Credential
    {
        [XmlElement(ElementName = "Identity")]
        public string Identity { get; set; }

        [XmlAttribute(AttributeName = "domain")]
        public string Domain { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "From")]
    public class From
    {
        [XmlElement(ElementName = "Credential")]
        public Credential Credential { get; set; }
    }

    [XmlRoot(ElementName = "To")]
    public class To
    {
        [XmlElement(ElementName = "Credential")]
        public Credential Credential { get; set; }
    }

    [XmlRoot(ElementName = "Sender")]
    public class Sender
    {
        [XmlElement(ElementName = "Credential")]
        public Credential Credential { get; set; }

        [XmlElement(ElementName = "UserAgent")]
        public string UserAgent { get; set; }
    }

    [XmlRoot(ElementName = "Header")]
    public class Header
    {
        [XmlElement(ElementName = "From")]
        public From From { get; set; }

        [XmlElement(ElementName = "To")]
        public To To { get; set; }

        [XmlElement(ElementName = "Sender")]
        public Sender Sender { get; set; }
    }

    [XmlRoot(ElementName = "ConfirmationHeader")]
    public class ConfirmationHeader
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "noticeDate")]
        public DateTime NoticeDate { get; set; }
    }

    [XmlRoot(ElementName = "DocumentReference")]
    public class DocumentReference
    {
        [XmlAttribute(AttributeName = "payloadID")]
        public string PayloadID { get; set; }
    }

    [XmlRoot(ElementName = "OrderReference")]
    public class OrderReference
    {
        [XmlElement(ElementName = "DocumentReference")]
        public DocumentReference DocumentReference { get; set; }

        [XmlAttribute(AttributeName = "orderID")]
        public int OrderID { get; set; }
    }

    [XmlRoot(ElementName = "ConfirmationStatus")]
    public class ConfirmationStatus
    {
        [XmlElement(ElementName = "UnitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "quantity")]
        public int Quantity { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ConfirmationItem")]
    public class ConfirmationItem
    {
        [XmlElement(ElementName = "UnitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        [XmlElement(ElementName = "ConfirmationStatus")]
        public ConfirmationStatus ConfirmationStatus { get; set; }

        [XmlAttribute(AttributeName = "lineNumber")]
        public int LineNumber { get; set; }

        [XmlAttribute(AttributeName = "quantity")]
        public int Quantity { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ConfirmationRequest")]
    public class ConfirmationRequest
    {
        [XmlElement(ElementName = "ConfirmationHeader")]
        public ConfirmationHeader ConfirmationHeader { get; set; }

        [XmlElement(ElementName = "OrderReference")]
        public OrderReference OrderReference { get; set; }

        [XmlElement(ElementName = "ConfirmationItem")]
        public ConfirmationItem ConfirmationItem { get; set; }
    }

    [XmlRoot(ElementName = "Request")]
    public class Request
    {
        [XmlElement(ElementName = "ConfirmationRequest")]
        public ConfirmationRequest ConfirmationRequest { get; set; }
    }
}