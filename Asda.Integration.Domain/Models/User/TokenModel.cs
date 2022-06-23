using System;

namespace Asda.Integration.Domain.Models.User
{
    public class TokenModel
    {
        public Guid Token { get; set; }
        public string Email { get; set; }
        public Guid UserId { get; set; }
    }
}