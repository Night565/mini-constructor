using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<HeroStat> HeroStats => Set<HeroStat>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();
    public DbSet<Direction> Directions => Set<Direction>();
    public DbSet<HeroContent> HeroContents => Set<HeroContent>();
    public DbSet<Benefit> Benefits => Set<Benefit>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
}
