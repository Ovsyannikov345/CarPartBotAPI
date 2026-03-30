using CarPartBotApi.Domain.Entities;

namespace CarPartBotApi.Domain.Interfaces.Data;

public interface IApplicationDbContext
{
    public IQueryable<T> Query<T>() where T : EntityBase;

    public void Add<T>(T entity) where T : EntityBase;

    public Task<int> SaveChangesAsync(CancellationToken ct = default);
}
