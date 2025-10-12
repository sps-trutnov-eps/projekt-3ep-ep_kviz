using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EP_Kviz.Views.Games.Modes
{
    public class _2v2Model : PageModel
    {
        public void OnGet()
        {
        }

        // V PageModel
        public int GameId { get; set; }
        public void OnGet(int id)
        {
            GameId = id;
        }
    }
}
