using Unity.Collections;
using Unity.Entities;

public struct ExchangeTaskBufferElement : IBufferElementData
{
    public Entity SourceInventoryEntity;
    public Entity ExchangeInventoryEntity;
    public Entity ResultInventoryEntity;

    public ExchangeRecipeBufferElement Recipe;
    public FixedString32 RequestedResourceName;
    public float RequestedResourceAmount;
    public TaskPriorityTag PriorityTag;
}
