using System.ComponentModel.DataAnnotations;

namespace EP_Kviz.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Uživatelské jméno je povinné")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Heslo je povinné")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Heslo musí mít alespoň 6 znaků")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Heslo musí obsahovat velké písmeno, malé písmeno, číslo a speciální znak.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Hesla se neshodují")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Potvrzení hesla je povinné")]
        public string ConfirmPassword { get; set; }
    }
}