using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(ExchangeTaskSystem))]
public class OrderReceptionSystem : JobComponentSystem
{
    GameManager gameManager;

    EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnStartRunning()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    float timer = 0f;
    float interval = 0.1f;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        timer += Time.DeltaTime;
        if (timer < interval)
            return inputDeps;
        else
            timer = 0f;

        inputDeps.Complete();

        var workplaceData = GetComponentDataFromEntity<ParentLocationComponent>(true);
        var ordersData = GetBufferFromEntity<OrderBufferElement>(false);
        var recipesData = GetBufferFromEntity<ExchangeRecipeBufferElement>(false);
        var inventoryData = GetBufferFromEntity<InventoryBufferElement>(true);
        var ecb = ecbSystem.CreateCommandBuffer();

        Entities
            .WithReadOnly(workplaceData)
            .WithNativeDisableParallelForRestriction(ordersData)
            .WithNativeDisableParallelForRestriction(recipesData)
            .WithReadOnly(inventoryData)
            .WithAll<ReadyToTakeOrderTagComponent>()
            .ForEach((Entity employee, in EmploymentComponent employment) => {
                var workplace = workplaceData[employment.Vacancy].Location;
                if (!ordersData.HasComponent(workplace))
                    return;
                var orders = ordersData[workplace];
                var recipes = recipesData[workplace];
                var inventory = inventoryData[workplace];
                if (orders.AreThereFreeOrders())
                {
                    var order = FirstValidOrder(orders, inventory);
                    if (order.Requester == Entity.Null)
                        return;
                    var recipe = order.Recipe;
                    var paymentAmount = order.RequestedAmount / recipe.GetSourceMultiplier(order.RequestedResourceName) * recipe.SourceResourcesMultipliers[0];
                    ecb.AddComponent(employee, new ExchangeTaskComponent
                    {
                        SourceInventoryEntity = workplace,
                        Recipe = recipe,
                        Step = ExchangeTaskStep.Started,
                        ExchangeInventoryEntity = order.Requester,
                        RequestedResourceName = order.RequestedResourceName,
                        RequestedResourceAmount = order.RequestedAmount,
                        ResultInventoryEntity = workplace,
                        TaskGiver = workplace,
                        IsWorkplaceTask = true
                    });
                    orders.TakeOrder(order);
                    ecb.RemoveComponent<ReadyToTakeOrderTagComponent>(employee);
                }
            }).Run();

        return inputDeps;
    }

    private static OrderBufferElement FirstValidOrder(DynamicBuffer<OrderBufferElement> orders, DynamicBuffer<InventoryBufferElement> inventory)
    {
        for (int i = 0; i < orders.Length; i++)
        {
            var order = orders[i];
            if (!order.IsTaken && order.RequestedAmount <= inventory.GetAmountOf(order.RequestedResourceName))
                return order;
        }
        return default;
    }
}
