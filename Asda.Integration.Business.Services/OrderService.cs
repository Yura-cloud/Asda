using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
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

        public List<XmlError> CreateXmlFilesOnFtp<T>(List<T> list, XmlModelType modelType)
        {
            var path = modelType switch
            {
                XmlModelType.Acknowledgment => _remoteConfig.AcknowledgmentPath,
                XmlModelType.Cancellations => _remoteConfig.CancellationPath,
                XmlModelType.Dispatch => _remoteConfig.DispatchPath,
                XmlModelType.SnapInventory => _remoteConfig.SnapInventoryPath
            };

            return _ftpServer.CreateFiles(list, path);
        }
    }
}