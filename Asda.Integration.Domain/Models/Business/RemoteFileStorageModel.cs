namespace Asda.Integration.Domain.Models.Business
{
    public class RemoteFileStorageModel
    {
        public string DispatchPath { get; }
        public string Acknowledgment { get; }

        public RemoteFileStorageModel(string dispatchPath, string acknowledgment)
        {
            DispatchPath = dispatchPath;
            Acknowledgment = acknowledgment;
        }
    }
}