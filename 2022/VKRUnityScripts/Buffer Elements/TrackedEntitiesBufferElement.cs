using Unity.Entities;

public struct TrackedEntitiesBufferElement : IBufferElementData
{
    public Entity TrackedEntity;
}
