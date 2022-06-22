using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct ThreadSafeCraftComponent : IComponentData
{
    public Entity CraftStationEntity;
    public ExchangeRecipeBufferElement Recipe;
    public FixedString32 RequestedResourceName;
    public float RequestedResourceAmount;
}
