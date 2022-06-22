using System;
using Unity.Entities;

[Serializable]
public struct ResourceMinComponent : IComponentData
{
    public float Value;
}
