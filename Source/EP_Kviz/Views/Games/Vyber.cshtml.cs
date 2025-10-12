using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EP_Kviz.Views.Games
{
    public class Vyber : PageModel
    {
        public void OnGet()
        {
        }

        public int GenerateRandomGameId()
        {
            var random = new Random();
            return random.Next(100000, 999999); // 6-místné èíslo
        }
    }
}

