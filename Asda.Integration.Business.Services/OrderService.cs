using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
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

        private readonly IRemoteConfigManagerService _remoteConfig;

        private readonly LocalFileStorageModel _localFileStorage;

        public OrderService(IFtpServerService ftpServer, IXmlService xmlService,
            ILocalConfigManagerService localConfig, IRemoteConfigManagerService remoteConfig)
        {
            _localFileStorage = localConfig.LocalFileStorage;
            _ftpServer = ftpServer;
            _xmlService = xmlService;
            _remoteConfig = remoteConfig;
        }

        public PurchaseOrder GetPurchaseOrder()
        {
            _ftpServer.DownloadXmlFileFromServer(_localFileStorage.OrderPath);
            return _xmlService.GetPurchaseOrderFromXml(_localFileStorage.OrderPath);
        }

        public void SendDispatchFiles(List<ShipmentConfirmation> shipmentConfirmations)
        {
            _xmlService.CreateLocalDispatchXmlFiles(shipmentConfirmations, _localFileStorage.DispatchPath);
            _ftpServer.SendFilesToServer(_localFileStorage.DispatchPath, _remoteConfig.DispatchPath);
        }

        public void SendAcknowledgmentFile(Acknowledgment acknowledgment)
        {
            _xmlService.CreateLocalAcknowledgmentXmlFile(acknowledgment,
                _localFileStorage.AcknowledgmentPath);
            _ftpServer.SendFilesToServer(_localFileStorage.AcknowledgmentPath,
                _remoteConfig.AcknowledgmentPath);
        }

        public void SendCancellationsFiles(List<Cancellation> cancellations)
        {
            _xmlService.CreateLocalCancellationXmlFiles(cancellations, _localFileStorage.CancellationPath);
            _ftpServer.SendFilesToServer(_localFileStorage.CancellationPath,
                _remoteConfig.CancellationPath);
        }

        public void SendSnapInventoriesFiles(List<InventorySnapshot> inventorySnapshots)
        {
            _xmlService.CreateLocalSnapInventoriesXmlFiles(inventorySnapshots,
                _localFileStorage.SnapInventoryPath);
            _ftpServer.SendFilesToServer(_localFileStorage.SnapInventoryPath,
                _remoteConfig.SnapInventoryPath);
        }
    }
}