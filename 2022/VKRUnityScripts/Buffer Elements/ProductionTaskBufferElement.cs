using Unity.Collections;
using Unity.Entities;

public struct ProductionTaskBufferElement : IBufferElementData
{
    public FixedString32 ResourceName;
    public float ResourceAmount;
    public float ProductionTime;
    public TaskPriorityTag PriorityTag;
}
