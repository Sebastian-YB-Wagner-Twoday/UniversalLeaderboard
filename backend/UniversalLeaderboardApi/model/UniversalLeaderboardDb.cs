using Microsoft.EntityFrameworkCore;

class UniversalLeaderboardDb : DbContext
{
    public UniversalLeaderboardDb(DbContextOptions<UniversalLeaderboardDb> options)
        : base(options) { }

    public DbSet<Contest> Contests => Set<Contest>();

    public DbSet<User> Users => Set<User>();



}