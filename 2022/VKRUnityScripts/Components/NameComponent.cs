using Unity.Entities;
using Unity.Collections;
public struct NameComponent : IComponentData
{
    public ExchangeTaskStep Step;
    public FixedString32 SourceResourceName;
    public Entity SourceInventoryEntity;
    public FixedString32 ResultResourceName;
    public Entity ExchangeInventoryEntity;
    public float ResultResourceAmount;
    public Entity ResultInventoryEntity;
}
