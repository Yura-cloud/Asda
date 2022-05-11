using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.PurchaseOrder;
using Asda.Integration.Service.Interfaces;

namespace Asda.Integration.Business.Services
{
    public class XmlService : IXmlService
    {
        private readonly IRemoteConfigManagerService _remoteConfig;
        private readonly IFtpService _ftp;

        public XmlService(IRemoteConfigManagerService remoteConfig, IFtpService ftp)
        {
            _remoteConfig = remoteConfig;
            _ftp = ftp;
        }

        public PurchaseOrder GetPurchaseOrderFromXml(string path)
        {
            var serializer = new XmlSerializer(typeof(PurchaseOrder));
            try
            {
                using var reader = new StreamReader(path);
                return (PurchaseOrder) serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                var message = $"Failed while working with GetXmlFileFromServer, with message {e.Message}";
                throw new Exception(message);
            }
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

            return _ftp.CreateFiles(list, path);
        }
    }
}