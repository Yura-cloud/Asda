namespace Asda.Integration.Domain.Models.Business
{
    public class LocalFileStorageModel
    {
        public string OrderPath { get; set; }
        public string DispatchPath { get; set; }
        public string AcknowledgmentPath { get; set; }
        public string CancellationPath { get; set; }

        public LocalFileStorageModel(LocalFileStorageModel localConfigLocalFileStorage)
        {
            OrderPath = localConfigLocalFileStorage.OrderPath;
            DispatchPath = localConfigLocalFileStorage.DispatchPath;
            AcknowledgmentPath = localConfigLocalFileStorage.AcknowledgmentPath;
            CancellationPath = localConfigLocalFileStorage.CancellationPath;
        }
    }
}