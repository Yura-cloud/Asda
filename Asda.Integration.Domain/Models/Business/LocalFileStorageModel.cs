namespace Asda.Integration.Domain.Models.Business
{
    public class LocalFileStorageModel
    {
        public string OrderPath { get; }
        public string DispatchPath { get; }
        public string AcknowledgmentPath { get; }
        public string CancellationPath { get; }
        public string SnapInventoryPath { get; }


        public LocalFileStorageModel(string orderPath, string dispatchPath, string acknowledgmentPath,
            string cancellationPath, string snapInventoryPath)
        {
            OrderPath = orderPath;
            DispatchPath = dispatchPath;
            AcknowledgmentPath = acknowledgmentPath;
            CancellationPath = cancellationPath;
            SnapInventoryPath = snapInventoryPath;
        }
    }
}