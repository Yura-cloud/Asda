namespace Asda.Integration.Domain.Interfaces
{
    public interface IRepository
    {
        bool FileExists(string authorizationToken);
        string Load(string authorizationToken);
        void Save(string authorizationToken, string contents);
        void Delete(string authorizationToken);
    }
}