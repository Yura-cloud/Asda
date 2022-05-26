namespace Asda.Integration.Domain.Models.Business
{
    public class FtpSettingsModel
    {
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }

        public FtpSettingsModel()
        {
            
        }
        public FtpSettingsModel(int port, string userName, string password, string host)
        {
            Port = port;
            UserName = userName;
            Password = password;
            Host = host;
        }
    }
}