namespace EP_Kviz.Models;
using EP_Kviz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<UzivateleModel>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seed Test User
        var testUserId = "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p";

        var testUser = new UzivateleModel
        {
            Id = testUserId,
            UserName = "test.acc",
            NormalizedUserName = "TEST.ACC",
            PasswordHash = "AQAAAAIAAYagAAAAEJ8SHVpTXGhxT+OLvVlCJHNvbKGJWQvnNzVaLkOqBfJQXZBGHgBbU0qU9zZ3lA3fxA=="       //heslo je Test@123
        };

        builder.Entity<UzivateleModel>().HasData(testUser);
    }
}