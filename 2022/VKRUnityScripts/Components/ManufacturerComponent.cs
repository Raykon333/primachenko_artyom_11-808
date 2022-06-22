using Unity.Collections;
using Unity.Entities;

public struct ManufacturedResource
{
    public int RecipeHash;
    public FixedString32 ResourceName;
    public float EfficiencyMultiplier;
    public float TimeNeeded;
}

public struct ManufacturerComponent : IComponentData
{
    public FixedList128<ManufacturedResource> ManufacturedResources;
}