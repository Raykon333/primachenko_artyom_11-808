using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct StatTrackingComponent : IComponentData
{
    public FixedString32 TrackedResource;
    public FixedString32 Name;
}
