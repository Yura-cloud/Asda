using System;
using LinnworksMacroHelpers.Interfaces;

namespace LinnworksMacroHelpers.Classes.Email
{
    public class EmailHeader : IValidation
    {
        public EmailHeader() { }
        public EmailHeader(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} must be provided for an email header");

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), $"{nameof(value)} must be provided for an email header");

            Name = name;
            Value = value;
        }

        /// <summary>
        /// Header name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Header value
        /// </summary>
        public string Value { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentNullException("Name must be supplied for header");

            if (string.IsNullOrWhiteSpace(Value))
                throw new ArgumentNullException("Value must be supplied for header");
        }
    }
}