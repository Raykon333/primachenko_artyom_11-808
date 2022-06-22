using Unity.Collections;
using Unity.Entities;

public struct ThreadSafeExchangeAsBuyerComponent : IComponentData
{
    public Entity BuyerEntity;
    public Entity SellerEntity;
    public ExchangeRecipeBufferElement Recipe;
    public FixedString32 RequestedResourceName;
    public float RequestedResourceAmount;
    public bool IsWorkplaceTask;
}
