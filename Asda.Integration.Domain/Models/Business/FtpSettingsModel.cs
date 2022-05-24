namespace Asda.Integration.Domain.Models.Business
{
    public class FtpSettingsModel
    {
        public int Port { get; }
        public string UserName { get; }
        public string Password { get; }
        public string Host { get; }

        public FtpSettingsModel(int port, string userName, string password, string host)
        {
            Port = port;
            UserName = userName;
            Password = password;
            Host = host;
        }
    }
}