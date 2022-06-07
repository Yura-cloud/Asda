using System;
using System.Xml.Serialization;
using Asda.Integration.Service.Intefaces;

namespace Asda.Integration.Domain.Models.Business.XML.Acknowledgment
{
    [XmlRoot(ElementName = "cXML")]
    public class Acknowledgment : HeaderBase, IGetFileName
    {
        private const string OrderUpdate = "cXML_OrderUpdate";

        [XmlElement(ElementName = "Request")]
        public Request Request { get; set; }

        public string GetFileName()
        {
            var timeStamp = Timestamp.ToString("yyyy.MM.dd");
            var id = Request.ConfirmationRequest.OrderReference.OrderID;
            return $"{OrderUpdate}_{id}_{timeStamp}.xml";
        }
    }

    [XmlRoot(ElementName = "Request")]
    public class Request
    {
        [XmlElement(ElementName = "ConfirmationRequest")]
        public ConfirmationRequest ConfirmationRequest { get; set; }
    }

    [XmlRoot(ElementName = "ConfirmationRequest")]
    public class ConfirmationRequest
    {
        [XmlElement(ElementName = "ConfirmationHeader")]
        public ConfirmationHeader ConfirmationHeader { get; set; }

        [XmlElement(ElementName = "OrderReference")]
        public OrderReference OrderReference { get; set; }
    }

    [XmlRoot(ElementName = "ConfirmationHeader")]
    public class ConfirmationHeader
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "noticeDate")]
        public DateTime NoticeDate { get; set; }
    }

    [XmlRoot(ElementName = "OrderReference")]
    public class OrderReference
    {
        [XmlElement(ElementName = "DocumentReference")]
        public DocumentReference DocumentReference { get; set; }

        [XmlAttribute(AttributeName = "orderID")]
        public string OrderID { get; set; }
    }

    [XmlRoot(ElementName = "DocumentReference")]
    public class DocumentReference
    {
        [XmlAttribute(AttributeName = "payloadID")]
        public string PayloadID { get; set; }
    }
}