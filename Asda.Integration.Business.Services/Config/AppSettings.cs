using System.IO;
using Asda.Integration.Business.Services.Helpers;
using Asda.Integration.Domain.Interfaces;

namespace Asda.Integration.Business.Services.Config
{
    public class AppSettings
    {
        public string UserStoreLocation { get; set; }

        public virtual IRepository FileRepository
        {
            get
            {
                if(!Directory.Exists(UserStoreLocation))
                {
                    Directory.CreateDirectory(UserStoreLocation);
                }

                return new FileRepository(UserStoreLocation);
            }
        }
    }
}
