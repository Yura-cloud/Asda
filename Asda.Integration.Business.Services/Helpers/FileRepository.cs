﻿using System.IO;
using System.Linq;
using Asda.Integration.Domain.Interfaces;

namespace Asda.Integration.Business.Services.Helpers
{
    public class FileRepository : IRepository
    {
        private readonly string _userStoreLocation;

        public FileRepository(string userStoreLocation)
        {
            this._userStoreLocation = userStoreLocation;
        }

        public void Delete(string authorizationToken)
        {
            File.Delete(this.Path(authorizationToken));
        }

        public bool FileExists(string authorizationToken)
        {
            return File.Exists(this.Path(authorizationToken));
        }

        public bool DirectoryExists()
        {
            return Directory.Exists(_userStoreLocation);
        }

        public string LoadByToken(string authorizationToken)
        {
            return File.ReadAllText(this.Path(authorizationToken));
        }

        public string[] LoadAll()
        {
            var filesNames = Directory.GetFiles(_userStoreLocation, "*.json");
            return filesNames.Select(File.ReadAllText).ToArray();
        }

        public void Save(string authorizationToken, string contents)
        {
            File.WriteAllText(this.Path(authorizationToken), contents);
        }

        private string Path(string authorizationToken)
        {
            return string.Concat(this._userStoreLocation, "//", authorizationToken, ".json");
        }
    }
}