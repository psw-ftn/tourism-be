using Explorer.BuildingBlocks.Core.Domain;
using FluentResults;

namespace Explorer.BuildingBlocks.Core.UseCases;

public interface ICrudRepository<TEntity> where TEntity : Entity
{
    Result<PagedResult<TEntity>> GetPaged(int page, int pageSize);
    Result<TEntity> Get(long id);
    Result<TEntity> Create(TEntity entity);
    Result<TEntity> Update(TEntity entity);
    Result Delete(long id);
}