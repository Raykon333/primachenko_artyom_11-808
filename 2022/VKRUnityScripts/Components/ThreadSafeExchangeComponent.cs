using Unity.Collections;
using Unity.Entities;

public struct ThreadSafeExchangeComponent : IComponentData
{
    public Entity SourceEntity;
    public Entity ResultEntity;
    public FixedString32 SourceResourceName;
    public FixedString32 ResultResourceName;
    public float ResultResourceAmount;
    public float ExchangeRatio;
}
