using Unity.Collections;
using Unity.Entities;

public struct InventoryBufferElement : IBufferElementData
{
    public FixedString32 ResourceName;
    public float ResourceAmount;

    public void Add(float amount)
    {
        ResourceAmount += amount;
    }
}
