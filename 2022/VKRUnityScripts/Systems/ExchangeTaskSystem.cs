using System.Linq;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Collections;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ExchangeTaskSystem : JobComponentSystem
{
    GameManager gameManager;

    EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnStartRunning()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var currentLocationData = GetComponentDataFromEntity<CurrentLocationComponent>(true);
        var exchangeTaskLocationData = GetComponentDataFromEntity<ExchangeTaskLocationComponent>(false);
        var exchangeRecipesData = GetBufferFromEntity<ExchangeRecipeBufferElement>(true);
        var inventoryData = GetBufferFromEntity<InventoryBufferElement>(true);
        var buyerComponent = GetComponentDataFromEntity<BuyerComponent>(false);
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithReadOnly(currentLocationData)
            .WithNativeDisableParallelForRestriction(exchangeTaskLocationData)
            .WithReadOnly(inventoryData)
            .WithNativeDisableParallelForRestriction(buyerComponent)
            .ForEach((int entityInQueryIndex, Entity entity, ref ExchangeTaskComponent task) =>
            {
                var recipe = task.Recipe;

                if (task.Step == ExchangeTaskStep.Started)
                {
                    //Entity just got the task
                    var entityIsOnSourceLocation = GoToLocation(task.SourceInventoryEntity, entity, currentLocationData, exchangeTaskLocationData, ecb, entityInQueryIndex);
                    //Entity is at source resource location
                    if (entityIsOnSourceLocation)
                    {
                        /*var transfer = ecb.CreateEntity(entityInQueryIndex);
                        for (int i = 0; i < recipe.SourceResourcesNames.Length; i++)
                        {
                            var sourceResourceName = recipe.SourceResourcesNames[i];
                            var sourceResourceAmount = recipe.NormalizeResourceAmount(ResourceType.Source, sourceResourceName, task.RequestedResourceName, task.RequestedResourceAmount);
                            ecb.AddComponent(entityInQueryIndex, transfer, new ThreadSafeTransferComponent
                            {
                                Sender = task.SourceInventoryEntity,
                                Receiver = entity,
                                ResourceName = sourceResourceName,
                                ResourceAmount = sourceResourceAmount
                            });
                        }*/
                        var transfer = ecb.CreateEntity(entityInQueryIndex);
                        ecb.AddComponent(entityInQueryIndex, transfer, new ThreadSafeTransferComponent
                        {
                            Sender = task.SourceInventoryEntity,
                            Receiver = entity,
                            ResourceName = task.RequestedResourceName,
                            ResourceAmount = task.RequestedResourceAmount
                        });
                        task.Step = ExchangeTaskStep.SourceAcquired;

                        if (task.TaskGiver != Entity.Null)
                        {
                            var removal = ecb.CreateEntity(entityInQueryIndex);
                            ecb.AddComponent(entityInQueryIndex, removal, new ThreadSafeOrderRemovalComponent
                            {
                                RequestedResourceName = task.RequestedResourceName,
                                Recipe = task.Recipe,
                                RequestedAmount = task.RequestedResourceAmount,
                                Orderer = task.ExchangeInventoryEntity,
                                OrdersReceiver = task.TaskGiver
                            });
                        }
                    }
                }
                else if (task.Step == ExchangeTaskStep.SourceAcquired)
                {
                    //Entity got source resource
                    var entityIsOnExchangeLocation = GoToLocation(task.ExchangeInventoryEntity, entity, currentLocationData, exchangeTaskLocationData, ecb, entityInQueryIndex);
                    //Entity is at exchange location
                    if (entityIsOnExchangeLocation)
                    {
                        var exchangeInventory = inventoryData[task.ExchangeInventoryEntity];
                        var exchange = ecb.CreateEntity(entityInQueryIndex);
                        ecb.AddComponent(entityInQueryIndex, exchange, new ThreadSafeExchangeAsBuyerComponent()
                        {
                            BuyerEntity = entity,
                            SellerEntity = task.ExchangeInventoryEntity,
                            Recipe = recipe,
                            RequestedResourceName = task.RequestedResourceName,
                            RequestedResourceAmount = task.RequestedResourceAmount,
                            IsWorkplaceTask = task.IsWorkplaceTask
                        });
                        if (buyerComponent.HasComponent(task.ExchangeInventoryEntity))
                        {
                            var component = buyerComponent[task.ExchangeInventoryEntity];
                            var boughtResources = component.BoughtResources;
                            int resourceId = 0;
                            for (int i = 0; i < boughtResources.Length; i++)
                            {
                                if (boughtResources[i].RecipeHash == recipe.Hash)
                                {
                                    resourceId = i;
                                    break;
                                }
                            }
                            var boughtResource = boughtResources[resourceId]; 
                            boughtResource.OrdersOnTheWay -= task.RequestedResourceAmount;
                            boughtResources[resourceId] = boughtResource;
                            component.BoughtResources = boughtResources;
                            buyerComponent[task.ExchangeInventoryEntity] = component;
                        }
                        task.Step = ExchangeTaskStep.ResultAcquired;
                    }
                }
                else if (task.Step == ExchangeTaskStep.ResultAcquired)
                {
                    //Entity got result resource
                    var entityIsOnResultLocation = GoToLocation(task.ResultInventoryEntity, entity, currentLocationData, exchangeTaskLocationData, ecb, entityInQueryIndex);
                    //Entity is at result location
                    if (entityIsOnResultLocation)
                    {
                        /*var transfer = ecb.CreateEntity(entityInQueryIndex);
                        ecb.AddComponent(entityInQueryIndex, transfer, new ThreadSafeTransferComponent
                        {
                            Sender = entity,
                            Receiver = task.ResultInventoryEntity,
                            ResourceName = task.RequestedResourceName,
                            ResourceAmount = task.RequestedResourceAmount
                        });*/
                        var transfer = ecb.CreateEntity(entityInQueryIndex);
                        for (int i = 0; i < recipe.SourceResourcesNames.Length; i++)
                        {
                            var sourceResourceName = recipe.SourceResourcesNames[i];
                            var sourceResourceAmount = recipe.NormalizeResourceAmount(ResourceType.Source, sourceResourceName, task.RequestedResourceName, task.RequestedResourceAmount);
                            ecb.AddComponent(entityInQueryIndex, transfer, new ThreadSafeTransferComponent
                            {
                                Sender = entity,
                                Receiver = task.ResultInventoryEntity,
                                ResourceName = sourceResourceName,
                                ResourceAmount = sourceResourceAmount
                            });
                        }
                        task.Step = ExchangeTaskStep.Finished;
                    }
                }
                if (task.Step == ExchangeTaskStep.Finished || task.Step == ExchangeTaskStep.Failed)
                {
                    ecb.RemoveComponent<ExchangeTaskLocationComponent>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<ExchangeTaskComponent>(entityInQueryIndex, entity);
                }
            }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }

    //Returns true if entity is already on location, false if it is not
    private static bool GoToLocation(Entity location, Entity movedEntity, ComponentDataFromEntity<CurrentLocationComponent> currentLocationData, 
        ComponentDataFromEntity<ExchangeTaskLocationComponent> exchangeTaskLocationData, EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex)
    {
        if (location == movedEntity)
            return true;
        if (!currentLocationData.HasComponent(movedEntity) || !(currentLocationData[movedEntity].entity == location))
        {
            var exchangeTaskLocation = new ExchangeTaskLocationComponent { Entity = location };
            if (exchangeTaskLocationData.HasComponent(movedEntity))
                exchangeTaskLocationData[movedEntity] = exchangeTaskLocation;
            else
                ecb.AddComponent(entityInQueryIndex, movedEntity, exchangeTaskLocation);
            return false;
        }
        else
            return true;
    }
}
