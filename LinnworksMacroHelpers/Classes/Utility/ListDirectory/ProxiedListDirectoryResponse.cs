using System.Collections.Generic;

namespace LinnworksMacroHelpers.Classes.Utility
{
    public class ProxiedListDirectoryResponse : ProxiedBaseResponse
    {
        /// <summary>
        /// A list of files contained in the requested directory
        /// </summary>
        public List<BaseDirectoryItem> FileList { get; set; }
    }
}
