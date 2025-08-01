namespace SipkaTemplate.Core.DTOs.HelperDTOs
{
    public class UserLoginDto
    {
        public string Email { get; set; }
        public string? NotificationToken { get; set; }
        public string Password { get; set; }
    }
}
