using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;
using Asda.Integration.Service.Intefaces;

namespace Asda.Integration.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IFtpServerService _ftpServer;
        private readonly IXmlService _xmlService;
        private LocalFileStorageModel LocalFileStorage { get; }
        private RemoteFileStorageModel RemoteFileStorage { get; }

        public OrderService(IFtpServerService ftpServer, IXmlService xmlService,
            ILocalConfigManagerService localConfig, IRemoteConfigManagerService remoteConfig)
        {
            _ftpServer = ftpServer;
            _xmlService = xmlService;
            LocalFileStorage = new LocalFileStorageModel(localConfig.OrderPath, localConfig.DispatchPath,
                localConfig.AcknowledgmentPath);
            RemoteFileStorage = new RemoteFileStorageModel(remoteConfig.DispatchPath,remoteConfig.AcknowledgmentPath);
        }

        public PurchaseOrder GetPurchaseOrder()
        {
            _ftpServer.DownloadXmlFileFromServer(LocalFileStorage.OrderPath);
            return _xmlService.GetPurchaseOrderFromXml(LocalFileStorage.OrderPath);
        }

        public void SendDispatchFile(List<ShipmentConfirmation> shipmentConfirmations)
        {
            _xmlService.CreateLocalDispatchXmlFile(shipmentConfirmations, LocalFileStorage.DispatchPath);
            _ftpServer.SentFileToServer(LocalFileStorage.DispatchPath, RemoteFileStorage.DispatchPath);
        }

        public void SendAcknowledgmentFile(Acknowledgment acknowledgment)
        {
            _xmlService.CreateLocalAcknowledgmentXmlFile(acknowledgment,LocalFileStorage.AcknowledgmentPath);
            _ftpServer.SentFileToServer(LocalFileStorage.AcknowledgmentPath, RemoteFileStorage.Acknowledgment);
        }
    }
}