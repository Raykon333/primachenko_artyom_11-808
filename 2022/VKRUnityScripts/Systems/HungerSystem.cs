using Unity.Entities;
using Unity.Jobs;

[UpdateBefore(typeof(DecisionMakingSystem))]
public class HungerSystem : JobComponentSystem
{
    GameManager gameManager;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var staticEntityCount = GetEntityQuery(typeof(TimeUniqueComponent)).CalculateEntityCount();
        if (staticEntityCount == 0)
            return inputDeps;
        var staticEntity = GetEntityQuery(typeof(TimeUniqueComponent)).GetSingletonEntity();

        var DeltaTime = Time.DeltaTime;
        DeltaTime *= EntityManager.GetComponentData<StaticComponent>(staticEntity).SimulationSpeed;

        var needsAndDutiesData = GetComponentDataFromEntity<NeedsAndDutiesComponent>(false);
        var currentLocation = GetComponentDataFromEntity<CurrentLocationComponent>(true);
        var homeLocation = GetComponentDataFromEntity<ParentLocationComponent>(true);

        float metabolismMultiplier = 1 / 3600f;

        JobHandle jobHandle = Entities
            .WithNativeDisableParallelForRestriction(needsAndDutiesData)
            .WithReadOnly(currentLocation)
            .WithReadOnly(homeLocation)
            .WithAll<UnitTagComponent>()
            .ForEach((Entity entity, ref HungerComponent hunger) =>
            {
                var satietyLoss = DeltaTime * metabolismMultiplier * hunger.MetabolismSpeed;
                if (currentLocation.HasComponent(entity) && currentLocation[entity].entity == homeLocation[entity].Location)
                    satietyLoss *= 0.2f;
                hunger.Satiety -= satietyLoss;

                if (needsAndDutiesData.HasComponent(entity))
                {
                    HungerStatus hs = default;
                    switch (hunger.Satiety)
                    {
                        case float n when n >= 100:
                            hs = HungerStatus.Full;
                            break;

                        case float n when n < 100 && n >= 90:
                            hs = HungerStatus.Fine;
                            break;

                        case float n when n < 90 && n >= 50:
                            hs = HungerStatus.Hungry;
                            break;

                        case float n when n < 50 && n >= 0:
                            hs = HungerStatus.Starving;
                            break;

                        case float n when n < 0:
                            hs = HungerStatus.Negative;
                            break;
                    }
                    var component = needsAndDutiesData[entity];
                    component.HungerStatus = hs;
                    needsAndDutiesData[entity] = component;
                }
            }).Schedule(inputDeps);

        return jobHandle;
    }
}
