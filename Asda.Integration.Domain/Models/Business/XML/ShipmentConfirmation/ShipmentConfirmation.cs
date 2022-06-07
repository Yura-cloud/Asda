using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Asda.Integration.Service.Intefaces;

namespace Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation
{
    [XmlRoot(ElementName = "cXML")]
    public class ShipmentConfirmation : HeaderBase, IGetFileName
    {
        private const string OrderConfirmation = "cXML_OrderShipmentConfirmation";

        [XmlElement(ElementName = "Request")]
        public Request Request { get; set; }

        public string GetFileName()
        {
            var timeStamp = Timestamp.ToString("yyyy.MM.dd");
            var id = Request.ShipNoticeRequest.ShipNoticePortion.OrderReference.OrderID;
            return $"{OrderConfirmation}_{id}_{timeStamp}.xml";
        }
    }

    [XmlRoot(ElementName = "Request")]
    public class Request
    {
        [XmlElement(ElementName = "ShipNoticeRequest")]
        public ShipNoticeRequest ShipNoticeRequest { get; set; }
    }

    [XmlRoot(ElementName = "ShipNoticeHeader")]
    public class ShipNoticeHeader
    {
        [XmlAttribute(AttributeName = "shipmentID")]
        public string ShipmentID { get; set; }

        [XmlAttribute(AttributeName = "noticeDate")]
        public DateTime NoticeDate { get; set; }

        [XmlAttribute(AttributeName = "shipmentDate")]
        public DateTime ShipmentDate { get; set; }

        [XmlAttribute(AttributeName = "deliveryDate")]
        public DateTime DeliveryDate { get; set; }

        [XmlAttribute(AttributeName = "carrierId")]
        public string CarrierId { get; set; }
    }

    [XmlRoot(ElementName = "CarrierIdentifier")]
    public class CarrierIdentifier
    {
        [XmlAttribute(AttributeName = "domain")]
        public string Domain { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ShipControl")]
    public class ShipControl
    {
        [XmlElement(ElementName = "CarrierIdentifier")]
        public CarrierIdentifier CarrierIdentifier { get; set; }

        [XmlElement(ElementName = "ShipmentIdentifier")]
        public string ShipmentIdentifier { get; set; }
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
        public string OrderID { get; set; }
    }

    [XmlRoot(ElementName = "ShipNoticeItem")]
    public class ShipNoticeItem
    {
        [XmlElement(ElementName = "UnitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        [XmlAttribute(AttributeName = "lineNumber")]
        public int LineNumber { get; set; }

        [XmlAttribute(AttributeName = "quantity")]
        public int Quantity { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ShipNoticePortion")]
    public class ShipNoticePortion
    {
        [XmlElement(ElementName = "OrderReference")]
        public OrderReference OrderReference { get; set; }

        [XmlElement(ElementName = "ShipNoticeItem")]
        public List<ShipNoticeItem> ShipNoticeItem { get; set; }
    }

    [XmlRoot(ElementName = "ShipNoticeRequest")]
    public class ShipNoticeRequest
    {
        [XmlElement(ElementName = "ShipNoticeHeader")]
        public ShipNoticeHeader ShipNoticeHeader { get; set; }

        [XmlElement(ElementName = "ShipControl")]
        public ShipControl ShipControl { get; set; }

        [XmlElement(ElementName = "ShipNoticePortion")]
        public ShipNoticePortion ShipNoticePortion { get; set; }
    }
}