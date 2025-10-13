using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using FluentResults;

namespace Explorer.Tours.Core.UseCases.Administration;

public class EquipmentService : IEquipmentService
{
    private readonly ICrudRepository<Equipment> _crudRepository;
    private readonly IMapper _mapper;

    public EquipmentService(ICrudRepository<Equipment> repository, IMapper mapper)
    {
        _crudRepository = repository;
        _mapper = mapper;
    }

    private Result<PagedResult<EquipmentDto>> MapToDto(Result<PagedResult<Equipment>> result)
    {
        if (result.IsFailed) return Result.Fail(result.Errors);

        var items = result.Value.Results.Select(_mapper.Map<EquipmentDto>).ToList();
        return new PagedResult<EquipmentDto>(items, result.Value.TotalCount);
    }

    public Result<PagedResult<EquipmentDto>> GetPaged(int page, int pageSize)
    {
        var result = _crudRepository.GetPaged(page, pageSize);
        return MapToDto(result);
    }

    public Result<EquipmentDto> Create(EquipmentDto entity)
    {
        try
        {
            var result = _crudRepository.Create(_mapper.Map<Equipment>(entity));
            return _mapper.Map<EquipmentDto>(result);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<EquipmentDto> Update(EquipmentDto entity)
    {
        try
        {
            var result = _crudRepository.Update(_mapper.Map<Equipment>(entity));
            return _mapper.Map<EquipmentDto>(result);
        }
        catch (KeyNotFoundException e)
        {
            return Result.Fail(FailureCode.NotFound).WithError(e.Message);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result Delete(long id)
    {
        try
        {
            _crudRepository.Delete(id);
            return Result.Ok();
        }
        catch (KeyNotFoundException e)
        {
            return Result.Fail(FailureCode.NotFound).WithError(e.Message);
        }
    }
}