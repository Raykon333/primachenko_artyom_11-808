using Unity.Entities;

public struct TravelTaskBufferElement : IBufferElementData
{
    public Entity TravelTo;
    public TaskPriorityTag PriorityTag;
}