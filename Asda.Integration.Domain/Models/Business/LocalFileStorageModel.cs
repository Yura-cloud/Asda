namespace Asda.Integration.Domain.Models.Business
{
    public class LocalFileStorageModel
    {
        public string OrderPath { get; }
        public string DispatchPath { get; }

        public LocalFileStorageModel(string orderPath, string dispatchPath)
        {
            OrderPath = orderPath;
            DispatchPath = dispatchPath;
        }
    }
}