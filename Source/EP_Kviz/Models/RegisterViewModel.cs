using System.ComponentModel.DataAnnotations;

namespace EP_Kviz.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "U�ivatelsk� jm�no je povinn�")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email je povinn�")]
        [EmailAddress(ErrorMessage = "Neplatn� email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Heslo je povinn�")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Heslo mus� m�t alespo� 6 znak�")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Heslo mus� obsahovat velk� p�smeno, mal� p�smeno, ��slo a speci�ln� znak.")]
        public string Password { get; set; }
    }
}