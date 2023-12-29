using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(15,MinimumLength = 3,ErrorMessage ="First Name must be least (2), and maximum(1) character")]
        public string? FirstName {  get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Last Name must be least (2), and maximum(1) character")]

        public string? LastName { get; set; }
        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage ="Invalid Email Address")]
        public string? Email { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password must be least (2), and maximum(1) character")]

        public string? Password { get; set; }

    }
}
