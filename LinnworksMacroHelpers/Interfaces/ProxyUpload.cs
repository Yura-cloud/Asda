using LinnworksMacroHelpers.Classes;
using System;
using System.Text;

namespace LinnworksMacroHelpers.Interfaces
{
    public interface ProxyUpload<TSettings, TResult> : IDisposable
        where TSettings : BaseSettings
        where TResult : BaseUploadResult
    {
        /// <summary>
        /// Writes supplied bytes to the FTP proxy.
        /// </summary>
        /// <param name="value"></param>
        void Write(byte[] value);

        /// <summary>
        /// Writes the supplied string to the FTP proxy.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        void Write(string value, Encoding encoding = null);

        TResult CompleteUpload();
    }
}