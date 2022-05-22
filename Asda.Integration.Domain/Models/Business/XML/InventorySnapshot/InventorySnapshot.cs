using System;
using System.Xml.Serialization;

namespace Asda.Integration.Domain.Models.Business.XML.InventorySnapshot
{
    [XmlRoot(ElementName = "cXML")]
    public class InventorySnapshot : HeaderBase
    {
        [XmlElement(ElementName = "Request")]
        public Request Request { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public DateTime Version { get; set; }
    }

    [XmlRoot(ElementName = "Request")]
    public class Request
    {
        [XmlElement(ElementName = "InventorySnapshotRequest")]
        public InventorySnapshotRequest InventorySnapshotRequest { get; set; }
    }

    [XmlRoot(ElementName = "InventorySnapshotRequestHeader")]
    public class InventorySnapshotRequestHeader
    {
        [XmlAttribute(AttributeName = "SnapshotDate")]
        public string SnapshotDate { get; set; }

        [XmlAttribute(AttributeName = "Description")]
        public string Description { get; set; }

        [XmlAttribute(AttributeName = "ListId")]
        public string ListId { get; set; }
    }

    [XmlRoot(ElementName = "Record")]
    public class Record
    {
        [XmlElement(ElementName = "AllocationQty")]
        public string AllocationQty { get; set; }

        [XmlElement(ElementName = "OrderedQty")]
        public string OrderedQty { get; set; }

        [XmlAttribute(AttributeName = "product-id")]
        public string ProductId { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Records")]
    public class Records
    {
        [XmlElement(ElementName = "Record")]
        public Record Record { get; set; }
    }

    [XmlRoot(ElementName = "InventorySnapshotRequest")]
    public class InventorySnapshotRequest
    {
        [XmlElement(ElementName = "InventorySnapshotRequestHeader")]
        public InventorySnapshotRequestHeader InventorySnapshotRequestHeader { get; set; }

        [XmlElement(ElementName = "Records")]
        public Records Records { get; set; }
    }
}