using System;
using Unity.Collections;
using Unity.Entities;

public struct ThreadSafeAdditionComponent : IComponentData
{
    public Entity InventoryEntity;
    public FixedString32 ResourceName;
    public float AddedAmount;
}
