using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Domain.Models.Business.XML.Acknowledgment;
using Asda.Integration.Domain.Models.Business.XML.Cancellation;
using Asda.Integration.Domain.Models.Business.XML.ShipmentConfirmation;
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

        public List<XmlError> CreateXmlFilesOnFtp<T>(List<T> list)
        {
            var path = list switch
            {
                List<Acknowledgment> => _remoteConfig.AcknowledgmentPath,
                List<Cancellation> => _remoteConfig.CancellationPath,
                List<ShipmentConfirmation> => _remoteConfig.DispatchPath,
                _ => _remoteConfig.SnapInventoryPath
            };

            return _ftp.CreateFiles(list, path);
        }
    }
}