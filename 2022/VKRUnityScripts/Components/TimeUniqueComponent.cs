using Unity.Entities;

public enum DayOfWeek
{
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday
}

public struct TimeUniqueComponent : IComponentData
{
    public int Day;
    public float Seconds;
    public DayOfWeek DayOfWeek => (DayOfWeek)(Day % 7);
}