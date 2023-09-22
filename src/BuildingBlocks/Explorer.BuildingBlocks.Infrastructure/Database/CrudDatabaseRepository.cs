using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Explorer.BuildingBlocks.Infrastructure.Database;

public class CrudDatabaseRepository<TEntity, TDbContext> : ICrudRepository<TEntity>
    where TEntity : Entity
    where TDbContext : DbContext
{
    protected readonly TDbContext DbContext;
    private readonly DbSet<TEntity> _dbSet;

    public CrudDatabaseRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TEntity>();
    }

    public Result<PagedResult<TEntity>> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public Result<TEntity> Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) return Result.Fail(FailureCode.NotFound);
        return entity;
    }

    public Result<TEntity> Create(TEntity entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public Result<TEntity> Update(TEntity entity)
    {
        try
        {
            DbContext.Update(entity);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException)
        {
            return Result.Fail(FailureCode.NotFound);
        }
        return entity;
    }

    public Result Delete(long id)
    {
        var result = Get(id);
        if (result.IsFailed) return Result.Fail(result.Errors);

        _dbSet.Remove(result.Value);
        DbContext.SaveChanges();

        return Result.Ok();
    }
}