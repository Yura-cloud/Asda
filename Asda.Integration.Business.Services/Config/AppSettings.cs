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
                if(!Directory.Exists(this.UserStoreLocation))
                {
                    Directory.CreateDirectory(this.UserStoreLocation);
                }

                return new FileRepository(this.UserStoreLocation);
            }
        }
    }
}
