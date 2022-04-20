using System.IO;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Intefaces;

namespace Asda.Integration.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IFtpServerService _ftpServer;
        private readonly IXmlConvertor _xmlConvertor;
        private LocalSettingsModel LocalSettings { get; }

        public OrderService(IFtpServerService ftpServer, IXmlConvertor xmlConvertor,
            ILocalConfigManagerService localConfig)
        {
            _ftpServer = ftpServer;
            _xmlConvertor = xmlConvertor;
            LocalSettings = new LocalSettingsModel(localConfig.LocalFilePath);
        }

        public PurchaseOrder GetPurchaseOrder()
        {
            _ftpServer.DownloadXmlFileFromServer();
            return _xmlConvertor.GetPurchaseOrderFromXml(LocalSettings.LocalFilePath);
        }
    }
}