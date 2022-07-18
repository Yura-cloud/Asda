using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;
using Asda.Integration.Service.Intefaces;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Asda.Integration.Service.Interfaces
{
    public interface IFtpService
    {
        List<XmlError> CreateFiles<T>(List<T> models, FtpSettingsModel ftpSettings, string remotePath, string userToken) where T : IGetFileName;

        List<SftpFile> GetAllSortedFilesInfo(SftpClient sftpClient, string path);

        List<T> GetFiles<T>(SftpClient sftpClient, List<string> filesPath, string userToken);
    }
}