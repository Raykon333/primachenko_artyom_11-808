using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class RestockSystem : JobComponentSystem
{
    GameManager gameManager;

    EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        var inventoryData = GetBufferFromEntity<InventoryBufferElement>(true);
        var ordersData = GetBufferFromEntity<OrderBufferElement>(true);
        var recipesData = GetBufferFromEntity<ExchangeRecipeBufferElement>(true);
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithReadOnly(inventoryData)
            .WithReadOnly(recipesData)
            .WithReadOnly(ordersData)
            .ForEach((int entityInQueryIndex, Entity store, in BuyerComponent buyer) => {
                if (!inventoryData.HasComponent(store))
                    return;
                var inventory = inventoryData[store];
                var boughtResources = buyer.BoughtResources;
                for(int i = 0; i < boughtResources.Length; i++)
                {
                    var boughtResource = boughtResources[i];
                    var orderSize = boughtResource.OrderSize;

                    var provider = boughtResource.Provider;
                    if (!ordersData.HasComponent(provider))
                        continue;
                    var orders = ordersData[provider];
                    var recipes = recipesData[provider];
                    var recipe = recipes.GetRecipe(boughtResource.RecipeHash);
                    var price = recipe.NormalizeResourceAmount(ResourceType.Source, "money", boughtResource.ResourceName, orderSize);
                    var ordersOnTheWay = boughtResource.OrdersOnTheWay;
                    var moneyAmount = inventory.GetAmountOf("money") - ordersOnTheWay * price;
                    if (moneyAmount >= price)
                    {
                        if (moneyAmount > price & !orders.OrderExists(store, recipe.ResultResourcesNames[0]))
                        {
                            var order = ecb.CreateEntity(entityInQueryIndex);
                            ecb.AddComponent(entityInQueryIndex, order, new ThreadSafeNewOrderComponent
                            {
                                RequestedResourceName = recipe.ResultResourcesNames[0],
                                Orderer = store,
                                OrdersReceiver = provider,
                                Recipe = recipe,
                                RequestedAmount = orderSize
                            });
                        }
                    }
                }
            }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
