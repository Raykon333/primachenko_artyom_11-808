using Unity.Entities;
using Unity.Jobs;

public class ResourceStatTrackingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var resourceMinData = GetComponentDataFromEntity<ResourceMinComponent>(false);
        var resourceMaxData = GetComponentDataFromEntity<ResourceMaxComponent>(false);
        var resourceAvgData = GetComponentDataFromEntity<ResourceAvgComponent>(false);
        var trackedEntitiesData = GetBufferFromEntity<TrackedEntitiesBufferElement>(true);
        var inventoryData = GetBufferFromEntity<InventoryBufferElement>(true);

        JobHandle jobHandle = Entities
            .WithReadOnly(trackedEntitiesData)
            .WithReadOnly(inventoryData)
            .WithNativeDisableParallelForRestriction(resourceMinData)
            .WithNativeDisableParallelForRestriction(resourceMaxData)
            .WithNativeDisableParallelForRestriction(resourceAvgData)
            .ForEach((int entityInQueryIndex, Entity entity, in StatTrackingComponent stat) =>
        {
            if (!trackedEntitiesData.HasComponent(entity))
                return;
            var trackedEntities = trackedEntitiesData[entity];
            if (trackedEntities.Length == 0)
                return;
            
            float min = 0;
            float max = 0;
            float sum = 0;
            for (int i = 0; i < trackedEntities.Length; i++)
            {
                if (!inventoryData.HasComponent(trackedEntities[i].TrackedEntity))
                    continue;
                var inventory = inventoryData[trackedEntities[i].TrackedEntity];

                var value = inventory.GetAmountOf(stat.TrackedResource);
                sum += value;
                if (i == 0)
                {
                    min = value;
                    max = value;
                }
                else
                {
                    if (value < min)
                        min = value;
                    if (value > max)
                        max = value;
                }
            }
            var avg = sum / trackedEntities.Length;

            if (resourceMinData.HasComponent(entity))
                resourceMinData[entity] = new ResourceMinComponent { Value = min };
            if (resourceMaxData.HasComponent(entity))
                resourceMaxData[entity] = new ResourceMaxComponent { Value = max };
            if (resourceAvgData.HasComponent(entity))
                resourceAvgData[entity] = new ResourceAvgComponent { Value = avg };
        }).Schedule(inputDeps);

        return jobHandle;
    }
}
