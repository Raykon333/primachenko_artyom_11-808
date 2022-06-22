using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class BankruptcySystem : JobComponentSystem
{
    GameManager gameManager;

    private readonly int checkIntervalInDays = 7;
    private int checkCount = 1;
    readonly float interval = 2f;
    float timer = 0f;

    EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnStartRunning()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var DeltaTime = Time.DeltaTime;

        timer += DeltaTime;
        if (timer < interval)
            return inputDeps;
        else
            timer -= interval;

        var staticEntityCount = GetEntityQuery(typeof(TimeUniqueComponent)).CalculateEntityCount();
        if (staticEntityCount == 0)
            return inputDeps;
        var staticEntity = GetEntityQuery(typeof(TimeUniqueComponent)).GetSingletonEntity();

        var time = EntityManager.GetComponentData<TimeUniqueComponent>(staticEntity);
        if (checkCount > time.Day / checkIntervalInDays)
            return inputDeps;

        var inventoryData = GetBufferFromEntity<InventoryBufferElement>(true);
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithAny<WorkplaceTagComponent, StoreTagComponent>()
            .WithReadOnly(inventoryData)
            .ForEach((int entityInQueryIndex, Entity location) =>
            {
                var money = inventoryData[location].GetAmountOf("money");
                //if (money < 0)
                    //ecb.DestroyEntity(entityInQueryIndex, location);
            }).Schedule(inputDeps);

        checkCount++;
        return jobHandle;
    }
}