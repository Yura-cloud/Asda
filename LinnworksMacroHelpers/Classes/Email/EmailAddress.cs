using System;
using LinnworksMacroHelpers.Interfaces;

namespace LinnworksMacroHelpers.Classes.Email
{
    public class EmailAddress : IValidation
    {
        public EmailAddress() { }

        public EmailAddress(string address)
        {
            Address = address;
        }

        public EmailAddress(string address, string displayName)
            : this(address)
        {
            DisplayName = displayName;
        }


        /// <summary>
        /// Email address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Address))
                throw new Exception("Email Address must be supplied");
        }

        public static implicit operator EmailAddress(string emailAddress)
        {
            return new EmailAddress(emailAddress);
        }
    }
}