using Unity.Collections;
using Unity.Entities;

public struct BoughtResource
{
    public FixedString32 ResourceName;
    public float OrderSize;
    public float ReserveSize;
    public float OrdersOnTheWay;
    public Entity Provider;
    public int RecipeHash;
}

public struct BuyerComponent : IComponentData
{
    public FixedList512<BoughtResource> BoughtResources;
}
