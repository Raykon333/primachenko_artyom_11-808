using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct SatietyTrackingComponent : IComponentData
{
    public FixedString32 Name;
}

