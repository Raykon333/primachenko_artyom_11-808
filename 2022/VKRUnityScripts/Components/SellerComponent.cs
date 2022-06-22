using Unity.Collections;
using Unity.Entities;

public struct SoldResource
{
    public FixedString32 ResourceName;
    public int RecipeHash;
}

public struct SellerComponent : IComponentData
{
    public FixedList128<SoldResource> SoldResources;
}
