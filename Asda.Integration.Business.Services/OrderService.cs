using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Business.XML.InventorySnapshot;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;
using Asda.Integration.Service.Intefaces;
using Asda.Integration.Service.Interfaces;

namespace Asda.Integration.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IFtpServerService _ftpServer;

        private readonly IXmlService _xmlService;

        private readonly ILocalConfigManagerService _localConfig;

        private readonly IRemoteConfigManagerService _remoteConfig;

        public OrderService(IFtpServerService ftpServer, IXmlService xmlService,
            ILocalConfigManagerService localConfig, IRemoteConfigManagerService remoteConfig)
        {
            _ftpServer = ftpServer;
            _xmlService = xmlService;
            _localConfig = localConfig;
            _remoteConfig = remoteConfig;
        }

        public PurchaseOrder GetPurchaseOrder()
        {
            _ftpServer.DownloadXmlFileFromServer(_localConfig.OrderPath);
            return _xmlService.GetPurchaseOrderFromXml(_localConfig.OrderPath);
        }

        public void SendDispatchFiles(List<ShipmentConfirmation> shipmentConfirmations)
        {
            _xmlService.CreateLocalDispatchXmlFiles(shipmentConfirmations, _localConfig.DispatchPath);
            _ftpServer.SentFilesToServer(_localConfig.DispatchPath, _remoteConfig.DispatchPath);
        }

        public void SendAcknowledgmentFile(Acknowledgment acknowledgment)
        {
            _xmlService.CreateLocalAcknowledgmentXmlFile(acknowledgment, _localConfig.AcknowledgmentPath);
            _ftpServer.SentFilesToServer(_localConfig.AcknowledgmentPath, _remoteConfig.AcknowledgmentPath);
        }

        public void SendCancellationsFile(List<Cancellation> cancellations)
        {
            _xmlService.CreateLocalCancellationXmlFiles(cancellations, _localConfig.CancellationPath);
            _ftpServer.SentFilesToServer(_localConfig.CancellationPath, _remoteConfig.CancellationPath);
        }

        public void SendSnapInventoriesFile(List<InventorySnapshot> inventorySnapshots)
        {
            _xmlService.CreateLocalSnapInventoriesXmlFiles(inventorySnapshots, _localConfig.SnapInventoryPath);
            _ftpServer.SentFilesToServer(_localConfig.SnapInventoryPath, _remoteConfig.SnapInventoryPath);
        }
    }
}