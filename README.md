# EP Kvíz

Tato verze známe hry "AZ Kvíz", nabídne neskutečnou zábavu a pobavení, dozvíte se nové a naučíte se nové vědomsti o SPŠ Trutnov, konkrétně z pohledu oboru EP.

Nabídneme vám otázky a informace, které jen tak nikde neseženete.
Máme připravené herní módy až pro dva tými o dvou hráčích.
Unikátní herní pole.
Ukládání vašeho skóre, ukládané na vašem účtu.

## Nové funkce

### 🏆 Scoreboard (Tabulka skóre)
- **Automatické měření času**: Timer se spouští při prvním tahu a zastavuje při výhře
- **Žebříček nejlepších**: Top výsledky seřazené podle času dokončení hry
- **Filtrování podle módů**: Zobrazení výsledků podle herního módu (Procvičení, 1v1, 2v2)
- **Persitentní úložiště**: Skóre ukládáno do SQL Server databáze
- **Moderní UI**: Graficky atraktivní design s podporou stránkování

Pro více informací o scoreboardu viz [SCOREBOARD.md](SCOREBOARD.md).

## Důležité!
Pokud budete chtít hrát z více zařízení musíte mít povolený firewall na počítači kde běží stránka s portem 7078

## Spuštění aplikace

1. Ujistěte se, že máte nainstalovaný .NET 8.0 SDK
2. Ujistěte se, že máte SQL Server LocalDB
3. Přejděte do složky projektu:
   ```bash
   cd Source/EP_Kviz
   ```
4. Aplikujte databázové migrace:
   ```bash
   dotnet ef database update
   ```
5. Spusťte aplikaci:
   ```bash
   dotnet run
   ```
6. Otevřete prohlížeč a přejděte na `http://localhost:7078`

## Testovací účet

- Email: `test@epkviz.cz`
- Heslo: `Test@123`

#
zdrojový kód uchováván v složce source
