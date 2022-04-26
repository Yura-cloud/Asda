namespace Asda.Integration.Domain.Models.Business
{
    public class LocalFileStorageModel
    {
        public string OrderPath { get; }
        public string DispatchPath { get; }
        public string AcknowledgmentPath { get; }

        public LocalFileStorageModel(string orderPath, string dispatchPath, string acknowledgmentPath)
        {
            OrderPath = orderPath;
            DispatchPath = dispatchPath;
            AcknowledgmentPath = acknowledgmentPath;
        }
    }
}