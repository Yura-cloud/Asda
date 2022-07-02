namespace Asda.Integration.Domain.Models.Business
{
    public class RemoteFileStorageModel
    {
        public string OrdersPath { get; set; }
        public string DispatchesPath { get; set; }
        public string AcknowledgmentsPath { get; set; }
        public string CancellationsPath { get; set; }
        public string SnapInventoriesPath { get; set; }

        public RemoteFileStorageModel()
        {
        }

        public RemoteFileStorageModel(string ordersPath, string dispatchesPath, string acknowledgmentsPath,
            string cancellationsPath, string snapInventoriesPath)
        {
            OrdersPath = ordersPath;
            DispatchesPath = dispatchesPath;
            AcknowledgmentsPath = acknowledgmentsPath;
            CancellationsPath = cancellationsPath;
            SnapInventoriesPath = snapInventoriesPath;
        }
    }
}