using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Intefaces;
using Renci.SshNet.Sftp;

namespace Asda.Integration.Service.Interfaces
{
    public interface IFtpService
    {
        List<XmlError> CreateFiles<T>(List<T> models, FtpSettingsModel ftpSettings, string remotePath, string userToken) where T : IGetFileName;

        List<SftpFile> GetAllFilesPath(FtpSettingsModel ftpSettings,string path);

        List<T> GetFiles<T>(FtpSettingsModel ftpSettings,
            List<string> filesPath, string userToken);
    }
}