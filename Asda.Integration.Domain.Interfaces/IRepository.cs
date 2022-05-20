namespace Asda.Integration.Domain.Interfaces
{
    public interface IRepository
    {
        bool FileExists(string authorizationToken);
        string LoadByToken(string authorizationToken);
        void Save(string authorizationToken, string contents);

        void Delete(string authorizationToken);

        string[] LoadAll();
        bool DirectoryExists();
    }
}
