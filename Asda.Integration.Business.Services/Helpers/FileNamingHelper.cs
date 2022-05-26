using System;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Business.XML.InventorySnapshot;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;

namespace Asda.Integration.Business.Services.Helpers
{
    public static class FileNamingHelper
    {
        private const string OrderUpdate = "cXML_OrderUpdate";

        private const string OrderCancellation = "cXML_OrderCancellation";

        private const string OrderConfirmation = "cXML_OrderShipmentConfirmation";

        private const string ItemUpdate = "cXML_ItemUpdate";

        public static string GetFileName(object obj)
        {
            var operationName = GetOperationName(obj);
            var timeStamp = GetTimeStamp(obj);
            var id = string.Empty;
            if (obj is not InventorySnapshot inventorySnapshot)
            {
                GetOrderIdFromObj(obj, ref id);
            }
            else
            {
                id = inventorySnapshot.Request.InventorySnapshotRequest.Records.Record.ProductId;
            }

            return $"{operationName}_{id}_{timeStamp}.xml";
        }

        private static string GetTimeStamp(object obj)
        {
            var header = obj as HeaderBase;
            return header?.Timestamp.ToString("dd.MM.yyyy");
        }

        private static string GetOperationName(object obj)
        {
            var path = obj switch
            {
                Acknowledgment => OrderUpdate,
                Cancellation => OrderCancellation,
                ShipmentConfirmation => OrderConfirmation,
                InventorySnapshot => ItemUpdate,
                _ => throw new ArgumentOutOfRangeException(nameof(obj), obj, null)
            };

            return path;
        }

        private static void GetOrderIdFromObj(object obj, ref string id)
        {
            if (obj == null)
            {
                return;
            }

            var objType = obj.GetType();
            var propertyInfos = objType.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var propValue = propertyInfo.GetValue(obj, null);
                if (propertyInfo.PropertyType.Assembly == objType.Assembly)
                {
                    GetOrderIdFromObj(propValue, ref id);
                }
                else
                {
                    if (propertyInfo.Name == "OrderID")
                    {
                        id = propValue?.ToString();
                        break;
                    }
                }
            }
        }
    }
}