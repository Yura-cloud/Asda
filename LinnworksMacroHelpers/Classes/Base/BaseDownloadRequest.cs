namespace LinnworksMacroHelpers.Classes
{
    public class BaseDownloadRequest<T> where T : BaseSettings
    {
        public BaseDownloadRequest(T settings)
        {
            Settings = settings;
        }

        public T Settings { get; set; }
    }
}