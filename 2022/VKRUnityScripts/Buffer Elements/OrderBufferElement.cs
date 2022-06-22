using Unity.Collections;
using Unity.Entities;

public struct OrderBufferElement : IBufferElementData
{
    public ExchangeRecipeBufferElement Recipe;
    public FixedString32 RequestedResourceName;
    public float RequestedAmount;
    public Entity Requester;
    public bool IsTaken;
}
