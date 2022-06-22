using System;
using Unity.Entities;

[Serializable]
public struct ResourceAvgComponent : IComponentData
{
    public float Value;
}
