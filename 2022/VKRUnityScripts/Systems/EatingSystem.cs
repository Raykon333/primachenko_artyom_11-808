using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EatingSystem : JobComponentSystem
{
    GameManager gameManager;

    EndSimulationEntityCommandBufferSystem ecbSystem;

    static readonly float portion = 20f;

    protected override void OnStartRunning()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var staticEntityCount = GetEntityQuery(typeof(TimeUniqueComponent)).CalculateEntityCount();
        if (staticEntityCount == 0)
            return inputDeps;
        var staticEntity = GetEntityQuery(typeof(TimeUniqueComponent)).GetSingletonEntity();

        var inventoryData = GetBufferFromEntity<InventoryBufferElement>(false);
        var exchangeTaskData = GetComponentDataFromEntity<ExchangeTaskComponent>(false);
        var storesData = GetBufferFromEntity<StoreBufferElement>(true);
        var translationData = GetComponentDataFromEntity<Translation>(true);
        var currentLocationData = GetComponentDataFromEntity<CurrentLocationComponent>(true);
        var movementTargetData = GetComponentDataFromEntity<MovementTargetComponent>(false);
        var homeLocationData = GetComponentDataFromEntity<ParentLocationComponent>(true);
        var recipesData = GetBufferFromEntity<ExchangeRecipeBufferElement>(true);
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithNativeDisableParallelForRestriction(inventoryData)
            .WithNativeDisableParallelForRestriction(exchangeTaskData)
            .WithReadOnly(storesData)
            .WithReadOnly(translationData)
            .WithReadOnly(currentLocationData)
            .WithNativeDisableParallelForRestriction(movementTargetData)
            .WithReadOnly(homeLocationData)
            .WithReadOnly(recipesData)
            .ForEach((int entityInQueryIndex, Entity entity, ref HungerComponent hunger, in NeedsAndDutiesComponent needsAndDuties) => {
                if (!inventoryData.HasComponent(entity))
                    return;
                var inventory = inventoryData[entity];
                if (needsAndDuties.HungerStatus == HungerStatus.Fine || needsAndDuties.HungerStatus == HungerStatus.Full 
                || (needsAndDuties.IsWorktime && needsAndDuties.HungerStatus != HungerStatus.Starving))
                    return;
                var cookedFood = inventory.GetAmountOf("cookedFood");
                if (cookedFood >= portion)
                {
                    inventory.SubtractAmountOf("cookedFood", portion);
                    hunger.Satiety += portion;
                }
                else if (inventory.GetAmountOf("rawFood") >= portion)
                {
                    var home = homeLocationData[entity];
                    if (currentLocationData.HasComponent(entity) && currentLocationData[entity].entity == home.Location)
                    {
                        inventory.SubtractAmountOf("rawFood", portion);
                        inventory.AddAmountOf("cookedFood", portion);
                    }
                    else if (movementTargetData.HasComponent(entity))
                    {
                        if (movementTargetData[entity].Destination == home.Location)
                            return;
                        else
                            movementTargetData[entity] = new MovementTargetComponent { Destination = home.Location };
                    }
                    else
                        ecb.AddComponent(entityInQueryIndex, entity, new MovementTargetComponent { Destination = home.Location });
                }
                else
                {
                    if (!exchangeTaskData.HasComponent(entity))
                    {
                        var stores = storesData[staticEntity].AsNativeArray();
                        var closestStore = ClosestStoreWith(stores, "rawFood", entity, translationData, inventoryData);
                        if (closestStore == Entity.Null)
                            return;
                        var recipes = recipesData[closestStore];
                        var recipe = recipes.GetRecipe("money", "rawFood");
                        var price = portion * recipe.GetSourceMultiplier("money");
                        if (inventory.GetAmountOf("money") < price)
                            return;
                        ecb.AddComponent(entityInQueryIndex, entity, new ExchangeTaskComponent
                        {
                            SourceInventoryEntity = entity,
                            Step = ExchangeTaskStep.Started,
                            Recipe = recipe,
                            RequestedResourceName = "rawFood",
                            RequestedResourceAmount = portion,
                            ResultInventoryEntity = entity,
                            ExchangeInventoryEntity = closestStore,
                            IsWorkplaceTask = false
                        });
                    }
                }
        }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }

    private static Entity ClosestStoreWith(NativeArray<StoreBufferElement> stores, FixedString32 resourceName, Entity entity, 
        ComponentDataFromEntity<Translation> translationData, BufferFromEntity<InventoryBufferElement> inventoryData)
    {
        Entity closestStore = default;
        float closestStoreDistance = 0f;
        bool firstFlag = true;
        var entityTranslation = translationData[entity].Value;
        for (int i = 0; i < stores.Length; i++)
        {
            var store = stores[i];
            var inventory = inventoryData[store.Location];
            if (inventory.GetAmountOf(resourceName) >= portion)
            {
                var storeDistance = Distance(translationData[store.Location].Value, entityTranslation);
                if (firstFlag || storeDistance < closestStoreDistance)
                {
                    closestStore = store.Location;
                    closestStoreDistance = Distance(entityTranslation, translationData[store.Location].Value);
                    firstFlag = false;
                }
            }
        }
        return closestStore;
    }

    private static float Distance(float3 point1, float3 point2)
    {
        var vector1 = new Vector3(point1.x, point1.y, point1.z);
        var vector2 = new Vector3(point2.x, point2.y, point2.z);
        return Vector3.Distance(vector1, vector2);
    }
}
