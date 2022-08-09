namespace Catalog.Auth.ViewModel
{
    using System.ComponentModel.DataAnnotations;

    public class SignUpModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Firstname is required")]
        [MaxLength(20,ErrorMessage ="Your name should not be more than 20 characters long")]
        public string Fullname { get; set; } = string.Empty;

    }
}
