using System.ComponentModel.DataAnnotations;

namespace EP_Kviz.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Uživatelské jméno je povinné")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email je povinný")]
        [EmailAddress(ErrorMessage = "Neplatný email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Heslo je povinné")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Heslo musí mít alespoò 6 znakù")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hesla se neshodují")]
        [Display(Name = "Potvrzení hesla")]
        public string ConfirmPassword { get; set; }
    }
}