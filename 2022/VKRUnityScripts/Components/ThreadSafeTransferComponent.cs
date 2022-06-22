using Unity.Collections;
using Unity.Entities;

public struct ThreadSafeTransferComponent : IComponentData
{
    public Entity Sender;
    public Entity Receiver;
    public FixedString32 ResourceName;
    public float ResourceAmount;
}
