using Unity.Collections;
using Unity.Entities;

public struct ThreadSafeOrderRemovalComponent : IComponentData
{
    public Entity OrdersReceiver;
    public Entity Orderer;
    public FixedString32 RequestedResourceName;
    public ExchangeRecipeBufferElement Recipe;
    public float RequestedAmount;
}
