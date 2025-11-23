# Scoreboard Feature - Dokumentace

## Přehled

Tato implementace přidává do EPKvíz aplikace scoreboard (tabulku skóre), který ukládá výsledky hráčů podle času dokončení hry. Scoreboard podporuje různé herní módy a zobrazuje žebříček nejlepších výsledků.

## Databázová struktura

### Tabulka `Scores`

Nová tabulka obsahuje následující pole:

```sql
- Id (int, PK, Identity) - Primární klíč
- PlayerId (nvarchar(max), nullable) - FK na AspNetUsers (email/username)
- Username (nvarchar(100), nullable) - Zobrazované uživatelské jméno
- Score (int) - Skóre pro třídění (v ms)
- ElapsedMs (bigint) - Přesný čas v milisekundách
- CreatedAt (datetime2) - Datum a čas vytvoření záznamu
- GameMode (nvarchar(50), nullable) - Herní mód (Procvičení, 1v1, 2v2)
```

### Aplikování migrace

Pro vytvoření tabulky v databázi spusťte:

```bash
cd Source/EP_Kviz
dotnet ef database update
```

## API Endpointy

### POST /api/scores

Přidá nový záznam skóre do databáze.

**Request Body:**
```json
{
  "playerId": "user@example.com",  // nullable
  "username": "JménoHráče",        // nullable
  "elapsedMs": 123456,             // povinné, > 0
  "gameMode": "Procvičení"         // nullable
}
```

**Poznámka:** Musí být zadáno buď `playerId` nebo `username`.

**Response (úspěch):**
```json
{
  "success": true,
  "scoreId": 1,
  "message": "Skóre úspěšně uloženo"
}
```

**Response (chyba):**
```json
{
  "error": "Popis chyby"
}
```

### GET /api/scores

Získá žebříček nejlepších skóre.

**Query parametry:**
- `top` (int, volitelné, default 50) - Počet vrácených záznamů
- `page` (int, volitelné) - Číslo stránky pro stránkování
- `gameMode` (string, volitelné) - Filtr podle herního módu

**Příklady:**
```
GET /api/scores                           // Top 50 ze všech módů
GET /api/scores?top=10                    // Top 10 ze všech módů
GET /api/scores?gameMode=Procvičení       // Top 50 z módu Procvičení
GET /api/scores?page=2&gameMode=1v1       // 2. stránka z módu 1v1
```

**Response:**
```json
{
  "success": true,
  "count": 50,
  "scores": [
    {
      "rank": 1,
      "id": 1,
      "playerId": "user@example.com",
      "username": "JménoHráče",
      "elapsedMs": 123456,
      "score": 123456,
      "gameMode": "Procvičení",
      "createdAt": "2024-11-23T15:30:00Z",
      "timeFormatted": "02:03.456"
    },
    ...
  ]
}
```

## Web UI

### Scoreboard stránka

Přístup přes:
- **URL:** `/Scoreboard/Index`
- **Tlačítko:** "Tabulka skóre" v hlavním menu

**Funkce:**
- Zobrazení top 50 záznamů
- Filtrování podle herního módu (Všechny, Procvičení, 1v1, 2v2)
- Stránkování pro více než 50 záznamů
- Formátované zobrazení času (mm:ss.sss)
- Zvýraznění top 3 míst (zlatá, stříbrná, bronzová)
- Moderní design konzistentní s aplikací

## Integrace do hry

### Automatické měření času

Timer se spustí automaticky při prvním tahu hry a zastaví se při výhře.

### Ukládání skóre

- Skóre se automaticky ukládá **pouze pro vítězného hráče**
- Ukládání probíhá asynchronně na pozadí
- Při chybě se zobrazí pouze konzolová zpráva (neruší uživatele)
- Ukládá se: playerId (email), username, elapsedMs, gameMode

### Implementace v kódu

V souboru `Views/Games/Modes/Play.cshtml`:

```javascript
// Timer se spouští při prvním tahu
if (hasEnoughPlayers && !timerStarted && state.grid && state.grid.length > 0) {
    gameStartTime = Date.now();
    timerStarted = true;
}

// Při výhře se automaticky ukládá skóre
if (state.winnerId == myId) {
    const elapsedMs = gameEndTime - gameStartTime;
    submitScore(elapsedMs, state.mode);
}
```

## Pravidla třídění

- **Nižší čas = lepší výsledek**
- Řazení vzestupně podle `ElapsedMs`
- Score = ElapsedMs (pro jednoduché třídění v databázi)

## Změna pravidel skórování

Pokud chcete změnit pravidlo převodu času na score, upravte soubor:
`Source/EP_Kviz/Services/ScoreService.cs`

```csharp
// Aktuálně:
Score = (int)elapsedMs

// Příklad změny - převod na sekundy:
Score = (int)Math.Round(elapsedMs / 1000.0)

// Příklad změny - inverze (vyšší = lepší):
Score = int.MaxValue - (int)elapsedMs
```

**Poznámka:** Po změně pravidla může být potřeba upravit i řazení v metodě `GetTopScoresAsync()`.

## Servisní vrstva

### IScoreService interface

```csharp
Task<ScoreEntry> AddScoreAsync(string? playerId, string? username, long elapsedMs, string? gameMode);
Task<List<ScoreEntry>> GetTopScoresAsync(int top = 50, string? gameMode = null);
Task<List<ScoreEntry>> GetScoresAsync(int page = 1, int pageSize = 50, string? gameMode = null);
Task<int> GetScoreCountAsync(string? gameMode = null);
```

### Validace

ScoreService provádí následující validace:
- `elapsedMs` musí být > 0
- Musí být zadán `playerId` nebo `username`
- `username` nesmí být delší než 100 znaků
- `pageSize` je omezen na max 100 záznamů

## Registrace služby

Služba je registrována v `Program.cs`:

```csharp
builder.Services.AddScoped<EP_Kviz.Services.IScoreService, EP_Kviz.Services.ScoreService>();
```

## Testování

### Manuální test

1. Spusťte aplikaci: `dotnet run --project Source/EP_Kviz/EP_Kviz.csproj`
2. Přihlaste se jako test uživatel (test@epkviz.cz / Test@123)
3. Spusťte hru v módu "Procvičení"
4. Vyhrajte hru
5. Otevřete scoreboard a ověřte uložený záznam

### Test API pomocí curl

```bash
# Přidání skóre
curl -X POST http://localhost:7078/api/scores \
  -H "Content-Type: application/json" \
  -d '{
    "username": "TestPlayer",
    "elapsedMs": 123456,
    "gameMode": "Procvičení"
  }'

# Získání top 10
curl http://localhost:7078/api/scores?top=10

# Získání podle módu
curl http://localhost:7078/api/scores?gameMode=Procvičení
```

## Bezpečnost

- API je veřejně přístupné (bez autentizace) pro jednoduchost
- Validace vstupů na úrovni služby i controlleru
- Používá EF Core parametrizované dotazy (ochrana proti SQL injection)
- Rate limiting není implementován (můžete přidat middleware)

## Možná vylepšení

1. **Autentizace:** Vyžadovat přihlášení pro ukládání skóre
2. **Rate limiting:** Omezit počet požadavků z jedné IP
3. **Duplicity:** Detekce a prevence duplicitních záznamů
4. **Statistiky:** Grafy vývoje času, průměrné časy, osobní statistiky
5. **Export:** Možnost exportu žebříčku do CSV/JSON
6. **Herní módy:** Podpora různých obtížností a variant
7. **Achievementy:** Odznaky za různé úspěchy
8. **Real-time:** SignalR pro živou aktualizaci scoreboardu

## Řešení problémů

### Migrace selže
```bash
# Zkontrolujte connection string v appsettings.json
# Ujistěte se, že máte SQL Server LocalDB nainstalovaný
# Zkuste smazat databázi a vytvořit novou:
dotnet ef database drop
dotnet ef database update
```

### Skóre se neuloží
- Otevřete konzoli prohlížeče (F12) a zkontrolujte chyby
- Ověřte, že API endpoint je dostupný
- Zkontrolujte logy aplikace

### Scoreboard stránka nezobrazuje data
- Ověřte, že migrace byla aplikována
- Zkontrolujte, že existují nějaké záznamy v databázi
- Ověřte connection string a připojení k databázi

## Autor a podpora

Implementováno pro projekt EPKvíz - SPŠ Trutnov (obor EP)
Datum: Listopad 2024

Pro otázky nebo problémy otevřete issue na GitHubu.
