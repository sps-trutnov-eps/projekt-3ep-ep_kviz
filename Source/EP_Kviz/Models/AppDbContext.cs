namespace EP_Kviz.Models;

using EP_Kviz.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
public class AppDbContext:IdentityDbContext<UzivateleModel>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

}

