using Microsoft.EntityFrameworkCore;

class StackDb : DbContext
{
    public StackDb(DbContextOptions<StackDb> options)
        : base(options) { }

    public DbSet<Tech> Techs => Set<Tech>();
}