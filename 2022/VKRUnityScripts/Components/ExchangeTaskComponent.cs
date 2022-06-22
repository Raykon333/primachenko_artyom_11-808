using Unity.Collections;
using Unity.Entities;

public enum ExchangeTaskStep
{
    Started,
    SourceAcquired,
    ResultAcquired,
    Finished,
    Failed
}

public struct ExchangeTaskComponent : IComponentData
{
    public ExchangeTaskStep Step;

    public Entity SourceInventoryEntity;
    public Entity ExchangeInventoryEntity;
    public Entity ResultInventoryEntity;

    public ExchangeRecipeBufferElement Recipe;
    public FixedString32 RequestedResourceName;
    public float RequestedResourceAmount;

    public bool IsWorkplaceTask;

    //Only for wokrflow
    public Entity TaskGiver;
}