using System.Collections.Generic;
using Asda.Integration.Domain.Models.Business;

namespace Asda.Integration.Service.Interfaces
{
    public interface IXmlService
    {
        List<XmlError> CreateXmlFilesOnFtp<T>(List<T> list);

    }
}