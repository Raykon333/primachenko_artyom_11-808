using Unity.Entities;

public enum HungerStatus
{
    Full,
    Fine,
    Hungry,
    Starving,
    Negative
}

public struct NeedsAndDutiesComponent : IComponentData
{
    public bool IsWorktime;
    public HungerStatus HungerStatus;
}
