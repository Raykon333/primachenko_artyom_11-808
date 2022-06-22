using Unity.Entities;
using Unity.Mathematics;

public struct VacancyInfoBufferElement : IBufferElementData
{
    public Entity Vacancy;
    public bool IsFree;
    public Entity Employee;
}
