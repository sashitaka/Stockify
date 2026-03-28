using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stockify.Models;

namespace Stockify.Data
{

    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PortfolioEntry> PortfolioEntries { get; set; }
        public DbSet<WatchlistEntry> WatchlistEntries { get; set; }
    }
}
