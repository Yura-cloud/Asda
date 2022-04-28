using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Interfaces
{
    public interface ILocalConfigManagerService
    {
        string OrderPath { get; }
        string DispatchPath { get; }
        string AcknowledgmentPath { get; }
        string CancellationPath { get; }

    }
}