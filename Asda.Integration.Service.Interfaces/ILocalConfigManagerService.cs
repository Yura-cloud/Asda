namespace Asda.Integration.Service.Intefaces
{
    public interface ILocalConfigManagerService
    {
        string OrderPath { get; }
        string DispatchPath { get; }
        string AcknowledgmentPath { get; }
    }
}