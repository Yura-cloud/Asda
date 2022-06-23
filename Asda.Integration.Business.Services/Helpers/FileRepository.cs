using System.IO;
using System.Linq;
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

        public bool DirectoryExists()
        {
            return Directory.Exists(_storeLocation);
        }

        public string Load(string authorizationToken)
        {
            return File.ReadAllText(Path(authorizationToken));
        }

        public string[] LoadAll()
        {
            var filesNames = Directory.GetFiles(_storeLocation, "*.json");
            return filesNames.Select(File.ReadAllText).ToArray();
        }

        public void Save(string authorizationToken, string contents)
        {
            File.WriteAllText(Path(authorizationToken), contents);
        }

        private string Path(string authorizationToken)
        {
            return string.Concat(_storeLocation, "//", authorizationToken, ".json");
        }
    }
}