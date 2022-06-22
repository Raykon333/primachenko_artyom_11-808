using Unity.Entities;
using UnityEngine;

public struct StaticComponent : IComponentData
{
    public int UnitsCount;
    public int LocationsCount;
    public float SimulationSpeed;
    public float SpacingMultiplier;
}
