using Unity.Entities;

public enum VacancyType
{
    Producer,
    Transporter
}

public struct VacancyTypeComponent : IComponentData
{
    public VacancyType Value;
}
