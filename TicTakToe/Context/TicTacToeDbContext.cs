using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Context
{
    /*
   * Migration helper:
   *
   * PackageManagerConsole:
   * Add-Migration {Migration_name} -Context TicTacToeDbContext -OutputDir ./Migrations -Project TicTakToe
   *
   * terminal:
   * dotnet ef migrations add {Migration_name} -c TicTacToeDbContext -o ./Migrations --project TicTakToe.csproj --startup-project TicTakToe.csproj 
   */

    public class TicTacToeDbContext: DbContext
    {
        public TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<GameHistory> History { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Game>()
                .HasOne(t => t.Table)
                .WithOne()
                .HasForeignKey<Game>(g => g.TableId);

            modelBuilder.Entity<User>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Table>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<GameHistory>()
                .HasKey(e => e.Id);
        }
    }
}
