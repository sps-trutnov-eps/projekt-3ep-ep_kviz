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
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Heslo musí obsahovat velké písmeno, malé písmeno, èíslo a speciální znak.")]
        public string Password { get; set; }
    }
}