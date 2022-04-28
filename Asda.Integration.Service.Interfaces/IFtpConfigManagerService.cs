namespace Asda.Integration.Service.Interfaces
{
    public interface IFtpConfigManagerService
    {
        int Port { get; }
        string UserName { get; }
        string Password { get; }
        string Host { get; }
        string ServerFilePath { get; }
    }
}