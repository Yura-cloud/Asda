using System.IO;
using Asda.Integration.Domain.Interfaces;

namespace Asda.Integration.Business.Services.Helpers
{
    public class FileRepository : IRepository
    {
        private readonly string _storeLocation;

        public FileRepository(string storeLocation)
        {
            if (!Directory.Exists(storeLocation))
            {
                Directory.CreateDirectory(storeLocation);
            }

            _storeLocation = storeLocation;
        }

        public void Delete(string authorizationToken)
        {
            File.Delete(GetFilePath(authorizationToken));
        }

        public bool FileExists(string authorizationToken)
        {
            return File.Exists(GetFilePath(authorizationToken));
        }

        public string Load(string authorizationToken)
        {
            return File.ReadAllText(GetFilePath(authorizationToken));
        }

        public void Save(string authorizationToken, string contents)
        {
            File.WriteAllText(GetFilePath(authorizationToken), contents);
        }

        private string GetFilePath(string name)
        {
            return Path.Combine(_storeLocation, string.Concat(name, ".json"));
        }
    }
}