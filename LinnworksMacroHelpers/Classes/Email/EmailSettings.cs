using System;
using LinnworksMacroHelpers.Interfaces;

namespace LinnworksMacroHelpers.Classes.Email
{
    public class EmailSettings : IValidation
    {
        public EmailSettings() { }

        public EmailSettings(string host, int port, string username, string password, bool enableSsl)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException(nameof(host), $"{nameof(host)} must be supplied with email settings.");

            if (port <= 0)
                throw new ArgumentOutOfRangeException(nameof(port), $"{nameof(host)} must be supplied with email settings.");

            Host = host;
            Port = port;
            Username = username;
            Password = password;
            EnableSsl = enableSsl;
        }

        /// <summary>
        /// Server host used to send email
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port to send request over.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Ssl required.
        /// </summary>
        public bool EnableSsl { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Host))
                throw new ArgumentNullException(nameof(Host), $"{nameof(Host)} must be supplied with email settings.");

            if (Port <= 0)
                throw new ArgumentOutOfRangeException(nameof(Port), $"{nameof(Port)} must be supplied with email settings.");
        }
    }
}