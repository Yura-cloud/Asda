namespace Asda.Integration.Service.Intefaces
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