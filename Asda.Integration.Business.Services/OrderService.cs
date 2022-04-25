using System;
using System.Collections.Generic;
using System.IO;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.ShipmentConfirmation;
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
            LocalFileStorage = new LocalFileStorageModel(localConfig.OrderPath, localConfig.DispatchPath);
            RemoteFileStorage = new RemoteFileStorageModel(remoteConfig.DispatchPath);
        }

        public PurchaseOrder GetPurchaseOrder()
        {
            _ftpServer.DownloadXmlFileFromServer(LocalFileStorage.OrderPath);
            return _xmlService.GetPurchaseOrderFromXml(LocalFileStorage.OrderPath);
        }

        public void SentDispatchFile(List<ShipmentConfirmation> shipmentConfirmations)
        {
            _xmlService.CreateLocalDispatchXmlFile(shipmentConfirmations,LocalFileStorage.DispatchPath);
            _ftpServer.SentFileToServer(LocalFileStorage.DispatchPath,RemoteFileStorage.DispatchPath);
        }
    }
}