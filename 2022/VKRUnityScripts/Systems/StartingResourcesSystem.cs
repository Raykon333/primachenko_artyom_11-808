using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class StartingResourcesSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem ecbSystem;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        var jobHandle = Entities
            .ForEach((int entityInQueryIndex, Entity entity, in BuyerComponent buyerComponent, in NeedsStartingResourcesTagComponent resourcesTag) =>
        {
            /*var boughtResources = buyerComponent.BoughtResources;
            for (int i = 0; i < boughtResources.Length; i++)
            {
                var boughtResource = boughtResources[i];
                float startingAmount = boughtResource.OrderSize;
                var addition = ecb.CreateEntity(entityInQueryIndex);
                ecb.AddComponent(entityInQueryIndex, addition, new ThreadSafeAdditionComponent
                {
                    InventoryEntity = entity,
                    ResourceName = boughtResource.ResourceName,
                    AddedAmount = startingAmount
                });
            }
            ecb.RemoveComponent<NeedsStartingResourcesTagComponent>(entityInQueryIndex, entity);*/
        }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}