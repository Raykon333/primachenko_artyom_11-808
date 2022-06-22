using System;
using Unity.Entities;

[Serializable]
public struct ResourceMaxComponent : IComponentData
{
    public float Value;
}
