using System;
using LinnworksMacroHelpers.Interfaces;

namespace LinnworksMacroHelpers.Classes.Email
{
    /// <summary>
    /// Email attachment
    /// </summary>
    public class EmailAttachment : IValidation
    {
        public EmailAttachment() { }
        public EmailAttachment(byte[] content, string contentType, string name)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content), $"{nameof(content)} must be supplied with email attachment.");

            if (content.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(content), $"{nameof(content)} must be supplied with email attachment.");

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentNullException(nameof(contentType), $"{nameof(contentType)} must be supplied with email attachment.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} must be supplied with email attachment.");

            Content = content;
            ContentType = contentType;
            Name = name;
        }

        /// <summary>
        /// Byte content of the attachment
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Content type of the attachment (e.g. 'text/plain')
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Name of the attachment
        /// </summary>
        public string Name { get; set; }

        public void Validate()
        {
            if (Content == null)
                throw new ArgumentNullException(nameof(Content), $"{nameof(Content)} must be supplied with email attachment.");

            if (Content.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(Content), $"{nameof(Content)} must be supplied with email attachment.");

            if (string.IsNullOrWhiteSpace(ContentType))
                throw new ArgumentNullException(nameof(ContentType), $"{nameof(ContentType)} must be supplied with email attachment.");

            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentNullException(nameof(Name), $"{nameof(Name)} must be supplied with email attachment.");
        }
    }
}