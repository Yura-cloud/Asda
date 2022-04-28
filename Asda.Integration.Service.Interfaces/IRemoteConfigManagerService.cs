namespace Asda.Integration.Service.Interfaces
{
    public interface IRemoteConfigManagerService
    {
        string DispatchPath { get; }
        string AcknowledgmentPath { get; }
        string CancellationPath { get; }
    }
}