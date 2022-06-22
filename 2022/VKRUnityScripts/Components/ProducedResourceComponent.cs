using Unity.Collections;
using Unity.Entities;

public struct ProducedResourceComponent : IComponentData
{
    public FixedString32 ResourceName;
}
