using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinnworksMacroHelpers.Classes.Email
{
    public sealed class ProxiedEmailRequest
    {       

        /// <summary>
        /// Email settings
        /// </summary>
        public EmailSettings Settings { get; set; }

        /// <summary>
        /// Sender
        /// </summary>
        public EmailAddress Sender { get; set; }

        /// <summary>
        /// From
        /// </summary>
        public EmailAddress From { get; set; }

        /// <summary>
        /// List of recipients
        /// </summary>
        public List<EmailAddress> To { get; set; }

        /// <summary>
        /// List of CC recipients
        /// </summary>
        public List<EmailAddress> CC { get; set; }

        /// <summary>
        /// List of BCC recipients
        /// </summary>
        public List<EmailAddress> BCC { get; set; }

        /// <summary>
        /// List of reply to
        /// </summary>
        public List<EmailAddress> ReplyToList { get; set; }

        /// <summary>
        /// Body of email
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Email body encoding, UTF-8 by default
        /// </summary>
        public string BodyEncoding { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Email subject encoding, UTF-8 by default
        /// </summary>
        public string SubjectEncoding { get; set; }

        /// <summary>
        /// Defines if the body of the email is HTML
        /// </summary>
        public bool IsBodyHtml { get; set; }

        /// <summary>
        /// Delivery notification options
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DeliveryNotificationOptions? DeliveryNotificationOptions { get; set; }

        /// <summary>
        /// Email headers
        /// </summary>
        public List<EmailHeader> Headers { get; set; }

        /// <summary>
        /// Encoding used with headers.
        /// </summary>
        public string HeadersEncoding { get; set; }

        /// <summary>
        /// Priority of email.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MailPriority? Priority { get; set; }

        /// <summary>
        /// Email attachements.
        /// </summary>
        public List<EmailAttachment> Attachments { get; set; }

        public ProxiedEmailRequest()
        {
            To = new List<EmailAddress>();
            CC = new List<EmailAddress>();
            BCC = new List<EmailAddress>();
            ReplyToList = new List<EmailAddress>();
            Headers = new List<EmailHeader>();
            Attachments = new List<EmailAttachment>();
        }

        public void Validate()
        {
            if (Settings == null)
                throw new Exception("Settings must be supplied");

            Settings.Validate();
            CleanCollections();

            if (To?.Any() == false && CC?.Any() == false && BCC?.Any() == false)
                throw new Exception("At lest one To, CC or BCC must be supplied");

            if (string.IsNullOrWhiteSpace(Body))
                throw new Exception("Body must have some content");

            if (string.IsNullOrWhiteSpace(Subject))
                throw new Exception("Subjet must be supplied");

            Sender?.Validate();
            From?.Validate();
            To?.Validate();
            CC?.Validate();
            BCC?.Validate();
            ReplyToList?.Validate();
            Headers?.Validate();
            Attachments?.Validate();

            if (Attachments?.Sum(s => s.Content.Length) > MAXIMUM_SUM_ATTACHMENTS_SIZE)
                throw new Exception($"Total size of attachments exceeds {(20971520 / 1024) / 1024}MB");

            if (Headers?.Count > 0 && Headers.Select(s => s.Name.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != Headers.Count)
                throw new Exception("Duplicate email headers are not allowed");
        }

        /// <summary>
        /// Maximum sum size of attachemnts that can be sent on an email
        /// </summary>
        [JsonIgnore]
        public const int MAXIMUM_SUM_ATTACHMENTS_SIZE = 20971520;

        private void CleanCollections()
        {
            To?.RemoveAll(s => s == null);
            CC?.RemoveAll(s => s == null);
            BCC?.RemoveAll(s => s == null);
            ReplyToList?.RemoveAll(s => s == null);
            Headers?.RemoveAll(s => s == null);
            Attachments?.RemoveAll(s => s == null);
        }
    }
}