namespace LinnworksAPI
{
    public interface IBaseController
    {
        string GetResponse(string extension, string body, string httpMethod = "POST", int? timeout = null);
    }
}