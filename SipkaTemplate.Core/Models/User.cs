using SipkaTemplate.Core.Enums;

namespace SipkaTemplate.Core.Models
{
    public class User : BaseEntity
    {
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }

        public string Name { get; set; }
        public string? ContentUrl { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? NotificationToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireAt { get; set; }
        public string? GoogleId { get; set; }
        public Role Role { get; set; }
        public string? TimeZone { get; set; }
        public string? Language { get; set; }


    }
}
