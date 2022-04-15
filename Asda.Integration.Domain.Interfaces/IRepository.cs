namespace Asda.Integration.Domain.Interfaces
{
    public interface IRepository
    {
        bool Exists(string authorizationToken);
        string Load(string authorizationToken);
        void Save(string authorizationToken, string contents);

        void Delete(string authorizationToken);

    }
}
