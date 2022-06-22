using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class InitializeInventoriesSystem : JobComponentSystem
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
        var buyerData = GetComponentDataFromEntity<BuyerComponent>(true);

        var jobHandle = Entities
            .WithReadOnly(buyerData)
            .ForEach((int entityInQueryIndex, Entity entity, in NeedsInventoryComponent inventoryTag) =>
        {
            var inventory = ecb.AddBuffer<InventoryBufferElement>(entityInQueryIndex, entity);
            float startingMoney = inventoryTag.StartingCapital;
            inventory.AddAmountOf("money", startingMoney);
            ecb.RemoveComponent<NeedsInventoryComponent>(entityInQueryIndex, entity);
            ecb.AddComponent<HasInventoryTagComponent>(entityInQueryIndex, entity);

            if (buyerData.HasComponent(entity))
            {
                var boughtResources = buyerData[entity].BoughtResources;
                for (int i = 0; i < boughtResources.Length; i++)
                {
                    var boughtResource = boughtResources[i];
                    float startingAmount = boughtResource.OrderSize * 2;
                    inventory.AddAmountOf(boughtResource.ResourceName, startingAmount);
                }
                ecb.RemoveComponent<NeedsStartingResourcesTagComponent>(entityInQueryIndex, entity);
            }
        }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
