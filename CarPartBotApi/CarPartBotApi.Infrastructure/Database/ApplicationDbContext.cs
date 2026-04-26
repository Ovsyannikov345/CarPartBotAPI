using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using CarPartBotApi.Domain.Interfaces.Entities;
using CarPartBotApi.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace CarPartBotApi.Infrastructure.Database;

public class ApplicationDbContext(IOptionsSnapshot<InfrastructureSettings> _options) : DbContext, IApplicationDbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Car> Cars { get; set; }

    public DbSet<UserInteractionState> UserInteractionStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);

        // Add soft delete query filter.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.DeletedAt));
                var condition = Expression.Equal(property, Expression.Constant(null, typeof(DateTime?)));
                var lambda = Expression.Lambda(condition, parameter);

                entityType.SetQueryFilter(lambda);
            }
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_options.Value.ConnectionStrings.Postgres);
    }

    public IQueryable<T> Query<T>() where T : EntityBase
    {
        return Set<T>().AsQueryable();
    }

    void IApplicationDbContext.Add<T>(T entity)
    {
        Set<T>().Add(entity);
    }
}
