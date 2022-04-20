namespace Asda.Integration.Domain.Models.Business
{
    public class LocalSettingsModel
    {
        public string LocalFilePath { get; }

        public LocalSettingsModel(string localFilePath)
        {
            LocalFilePath = localFilePath;
        }
    }
}