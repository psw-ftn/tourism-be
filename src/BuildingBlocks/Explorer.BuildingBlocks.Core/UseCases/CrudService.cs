using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using FluentResults;

namespace Explorer.BuildingBlocks.Core.UseCases;

/// <summary>
/// A base service class that offers CRUD methods for persisting TDomain objects, based on the passed TDto object.
/// </summary>
/// <typeparam name="TDto">Type of output data transfer object.</typeparam>
/// <typeparam name="TDomain">Type of domain object that maps to TDto</typeparam>
public abstract class CrudService<TDto, TDomain> : BaseService<TDto, TDomain> where TDomain : Entity
{
    protected readonly ICrudRepository<TDomain> CrudRepository;

    protected CrudService(ICrudRepository<TDomain> crudRepository, IMapper mapper) : base(mapper)
    {
        CrudRepository = crudRepository;
    }

    public Result<PagedResult<TDto>> GetPaged(int page, int pageSize)
    {
        var result = CrudRepository.GetPaged(page, pageSize);
        return MapToDto(result);
    }

    public Result<TDto> Get(int id)
    {
        var result = CrudRepository.Get(id);
        return MapResult(result.Value);
    }

    public virtual Result<TDto> Create(TDto entity)
    {
        var result = CrudRepository.Create(MapToDomain(entity));
        return MapResult(result);
    }

    public virtual Result<TDto> Update(TDto entity)
    {
        var result = CrudRepository.Update(MapToDomain(entity));
        return MapResult(result);
    }

    private Result<TDto> MapResult(Result<TDomain> result)
    {
        if (result.IsFailed) return Result.Fail(result.Errors);
        return MapToDto(result.Value);
    }

    public virtual Result Delete(int id)
    {
        var result = CrudRepository.Delete(id);
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }
}