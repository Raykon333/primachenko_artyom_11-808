using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class SalarySystem : JobComponentSystem
{
    GameManager gameManager;

    private readonly int daysToSalary = 7;
    private int salaryCounter = 0;
    readonly float interval = 1f;
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

        if (salaryCounter > (time.Day - 1) / daysToSalary)
            return inputDeps;

        var parentLocationData = GetComponentDataFromEntity<ParentLocationComponent>(true);
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithAll<VacancyTagComponent>()
            .WithReadOnly(parentLocationData)
            .ForEach((int entityInQueryIndex, Entity vacancy, in RelatedUnitComponent employee, in PaymentComponent salary) =>
            {
                if (employee.Entity.Equals(Entity.Null))
                    return;
                var transfer = ecb.CreateEntity(entityInQueryIndex);
                ecb.AddComponent(entityInQueryIndex, transfer, new ThreadSafeTransferComponent
                {
                    Sender = parentLocationData[vacancy].Location,
                    Receiver = employee.Entity,
                    ResourceName = "money",
                    ResourceAmount = salary.Value
                });
            }).Schedule(inputDeps);

        salaryCounter++;
        return jobHandle;
    }
}