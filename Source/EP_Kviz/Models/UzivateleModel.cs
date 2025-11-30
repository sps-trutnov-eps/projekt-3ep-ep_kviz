using Microsoft.AspNetCore.Identity;

namespace EP_Kviz.Models
{
    public class UzivateleModel:IdentityUser
    {


        /// Celkový počet her, které uživatel odehrál.
        public int PocetOdehranychHer { get; set; } = 0;

        /// Celkový počet her, které uživatel vyhrál.
        public int PocetVyhranychHer { get; set; } = 0;
    }
}
