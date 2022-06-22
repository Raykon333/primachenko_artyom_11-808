using Unity.Collections;
using Unity.Entities;

public enum GiveOrReceiveType
{
    Give,
    Receive
}

public struct GiveOrReceiveTaskBufferElement : IBufferElementData
{
    public GiveOrReceiveType Type;
    public Entity TargetEntity;
    public FixedString32 ResourceName;
    public float ResourceAmount;
    public TaskPriorityTag PriorityTag;
}
