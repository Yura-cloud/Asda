namespace Asda.Integration.Domain.Models.Business
{
    public class RemoteFileStorageModel
    {
        public string DispatchPath { get; }

        public RemoteFileStorageModel(string dispatchPath)
        {
            DispatchPath = dispatchPath;
        }
    }
}