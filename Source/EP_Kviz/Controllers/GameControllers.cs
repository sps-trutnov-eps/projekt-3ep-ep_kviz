using Microsoft.AspNetCore.Mvc;

public class GamesController : Controller
{
    public IActionResult Vyber()
    {
        return View();
    }

    // Metoda pro 1v1 mód
    public IActionResult OneVOne(int pid)
    {
        int gameId = GenerateRandomGameId();
        ViewBag.GameId = gameId;
        ViewBag.PlayerId = pid;
        ViewBag.Mode = "1v1";
        return View("~/Views/Games/Modes/1v1.cshtml");
    }

    // Metoda pro 2v2 mód
    public IActionResult TwoVTwo(int pid)
    {
        int gameId = GenerateRandomGameId();
        ViewBag.GameId = gameId;
        ViewBag.PlayerId = pid;
        ViewBag.Mode = "2v2";
        return View("~/Views/Games/Modes/2v2.cshtml");
    }

    // Metoda pro Procvičení
    public IActionResult Procviceni(int pid)
    {
        int gameId = GenerateRandomGameId();
        ViewBag.GameId = gameId;
        ViewBag.PlayerId = pid;
        ViewBag.Mode = "Procvičení";
        return View("~/Views/Games/Modes/Procviceni.cshtml");
    }

    // Generování náhodného Game ID
    private int GenerateRandomGameId()
    {
        var random = new Random();
        return random.Next(100000, 999999); // 6-místné číslo
    }
}