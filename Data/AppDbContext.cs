using Microsoft.EntityFrameworkCore;
using GameApi.Models;
namespace GameApi.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Character> Characters => Set<Character>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Account>(e => {
            e.Property(x => x.Username).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Username).IsUnique();
            e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(255);
        });
        b.Entity<Character>(e => {
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.Property(x => x.Class).IsRequired().HasMaxLength(30);
            e.HasOne(x => x.Account).WithMany(x => x.Characters).HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
