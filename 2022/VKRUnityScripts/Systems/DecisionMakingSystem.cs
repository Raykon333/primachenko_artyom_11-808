using Unity.Entities;
using Unity.Jobs;

public class DecisionMakingSystem : JobComponentSystem
{
    GameManager gameManager;

    readonly float interval = 300f;
    float timer = 0f;

    EndSimulationEntityCommandBufferSystem ecbSystem;

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

        var DeltaTime = Time.DeltaTime;
        DeltaTime *= EntityManager.GetComponentData<StaticComponent>(staticEntity).SimulationSpeed;

        timer += DeltaTime;
        if (timer < interval)
            return inputDeps;
        else
            timer -= interval;

        var time = EntityManager.GetComponentData<TimeUniqueComponent>(staticEntity);
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();
        var MovementTargetData = GetComponentDataFromEntity<MovementTargetComponent>(true);
        var currentLocationData = GetComponentDataFromEntity<CurrentLocationComponent>(true);
        var employmentData = GetComponentDataFromEntity<EmploymentComponent>(true);
        var homeLocationData = GetComponentDataFromEntity<ParentLocationComponent>(true);
        var exchangeTaskLocationData = GetComponentDataFromEntity<ExchangeTaskLocationComponent>(true);

        JobHandle jobHandle = Entities
            .WithReadOnly(MovementTargetData)
            .WithReadOnly(currentLocationData)
            .WithReadOnly(employmentData)
            .WithReadOnly(homeLocationData)
            .WithReadOnly(exchangeTaskLocationData)
            .WithAll<UnitTagComponent>()
            .ForEach((int entityInQueryIndex, Entity entity, in NeedsAndDutiesComponent needAndDuties) =>
            {
                Entity newDestination = default;
                Entity currentLocation = default;
                Entity currentDestination = default;
                if (currentLocationData.HasComponent(entity))
                    currentLocation = currentLocationData[entity].entity;
                if (MovementTargetData.HasComponent(entity))
                    currentDestination = MovementTargetData[entity].Destination;

                if (exchangeTaskLocationData.HasComponent(entity))
                {
                    newDestination = exchangeTaskLocationData[entity].Entity;
                }
                else if (needAndDuties.IsWorktime)
                {
                    newDestination = homeLocationData[employmentData[entity].Vacancy].Location;
                }
                else
                {
                    newDestination = homeLocationData[entity].Location;
                }

                if (newDestination != default && newDestination != currentLocation && newDestination != currentDestination)
                {
                    if (!MovementTargetData.HasComponent(entity))
                        ecb.AddComponent(entityInQueryIndex, entity, new MovementTargetComponent { Destination = newDestination });
                    else
                        ecb.SetComponent(entityInQueryIndex, entity, new MovementTargetComponent { Destination = newDestination });
                }
            }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
