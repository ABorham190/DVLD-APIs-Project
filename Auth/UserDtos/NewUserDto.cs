using System.ComponentModel.DataAnnotations;

namespace dvld_api.Auth.UserDtos
{
    public class NewUserDto
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
    }
}
