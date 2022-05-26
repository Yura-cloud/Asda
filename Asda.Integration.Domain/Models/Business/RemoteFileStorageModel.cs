namespace Asda.Integration.Domain.Models.Business
{
    public class RemoteFileStorageModel
    {
        public string OrderPath { get; set; }
        public string DispatchPath { get; set; }
        public string AcknowledgmentPath { get; set; }
        public string CancellationPath { get; set; }
        public string SnapInventoryPath { get; set; }

        public RemoteFileStorageModel()
        {
            
        }
        public RemoteFileStorageModel(string orderPath, string dispatchPath, string acknowledgmentPath,
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