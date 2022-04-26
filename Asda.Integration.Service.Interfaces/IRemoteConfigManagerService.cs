namespace Asda.Integration.Service.Intefaces
{
    public interface IRemoteConfigManagerService
    {
        string DispatchPath { get; }
        string AcknowledgmentPath { get; }
    }
}