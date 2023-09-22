using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Equipment : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
}