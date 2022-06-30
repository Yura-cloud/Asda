using System.IO;
using Asda.Integration.Domain.Interfaces;

namespace Asda.Integration.Business.Services.Helpers
{
    public class FileRepository : IRepository
    {
        private readonly string _storeLocation;

        public FileRepository(string storeLocation)
        {
            _storeLocation = storeLocation;
        }

        public void Delete(string authorizationToken)
        {
            File.Delete(Path(authorizationToken));
        }

        public bool FileExists(string authorizationToken)
        {
            return File.Exists(Path(authorizationToken));
        }

        private bool DirectoryExists()
        {
            return Directory.Exists(_storeLocation);
        }

        public string Load(string authorizationToken)
        {
            return File.ReadAllText(Path(authorizationToken));
        }

        public void Save(string authorizationToken, string contents)
        {
            File.WriteAllText(Path(authorizationToken), contents);
        }

        private string Path(string authorizationToken)
        {
            if (!DirectoryExists())
            {
                Directory.CreateDirectory(_storeLocation);
            }

            return string.Concat(_storeLocation, "//", authorizationToken, ".json");
        }
    }
}