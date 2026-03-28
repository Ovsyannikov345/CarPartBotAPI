using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using CarPartBotApi.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CarPartBotApi.Infrastructure.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> _options) : DbContext(_options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Car> Cars { get; set; }

    public DbSet<UserInteractionState> UserInteractionStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}
