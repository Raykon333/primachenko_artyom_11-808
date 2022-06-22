using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ThreadSafeOrderSystem : JobComponentSystem
{
    GameManager gameManager;

    EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnStartRunning()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ordersData = GetBufferFromEntity<OrderBufferElement>(false);
        var ecb = ecbSystem.CreateCommandBuffer();

        inputDeps.Complete();

        Entities
            .WithNativeDisableParallelForRestriction(ordersData)
            .ForEach((Entity entity, in ThreadSafeNewOrderComponent order) =>
            {
                var orders = ordersData[order.OrdersReceiver];
                orders.Add(new OrderBufferElement
                {
                    Requester = order.Orderer,
                    RequestedAmount = order.RequestedAmount,
                    RequestedResourceName = order.RequestedResourceName,
                    Recipe = order.Recipe
                });
                ecb.DestroyEntity(entity);
            }).Run();

        Entities
            .ForEach((Entity entity, in ThreadSafeOrderRemovalComponent removal) => {
                var orders = ordersData[removal.OrdersReceiver];
                var index = ordersData[removal.OrdersReceiver].OrderIndex(removal.Orderer, removal.RequestedResourceName, removal.Recipe, removal.RequestedAmount);
                if (index != -1)
                    orders.RemoveAt(index);
                ecb.DestroyEntity(entity);
            }).Run();

        return default;
    }
}
