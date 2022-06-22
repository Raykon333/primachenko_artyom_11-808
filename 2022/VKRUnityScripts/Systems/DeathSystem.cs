using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class DeathSystem : JobComponentSystem
{
    GameManager gameManager;

    float timer = 0f;
    float interval = 5f;

    BeginPresentationEntityCommandBufferSystem ecbSystem;

    protected override void OnStartRunning()
    {
        ecbSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        timer += Time.DeltaTime;
        if (timer < interval)
            return inputDeps;
        else
            timer -= interval;

        var employmentData = GetComponentDataFromEntity<EmploymentComponent>(true);
        var relatedUnitData = GetComponentDataFromEntity<RelatedUnitComponent>(false);
        var exchangeTaskData = GetComponentDataFromEntity<ExchangeTaskComponent>(true);
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithReadOnly(employmentData)
            .WithNativeDisableParallelForRestriction(relatedUnitData)
            .WithReadOnly(exchangeTaskData)
            .WithAll<UnitTagComponent>()
            .ForEach((int entityInQueryIndex, Entity entity, ref Scale scale, in NeedsAndDutiesComponent needsAndDuties) =>
            {
                if (needsAndDuties.HungerStatus != HungerStatus.Negative)
                    return;
                if (employmentData.HasComponent(entity))
                {
                    var vacancy = employmentData[entity].Vacancy;
                    var component = relatedUnitData[vacancy];
                    component.Entity = Entity.Null;
                    ecb.SetComponent(entityInQueryIndex, vacancy, component);
                }
                if (exchangeTaskData.HasComponent(entity))
                {
                    var task = exchangeTaskData[entity];
                    if (task.TaskGiver != Entity.Null && task.Step == ExchangeTaskStep.Started)
                    {
                        var removal = ecb.CreateEntity(entityInQueryIndex);
                        ecb.AddComponent(entityInQueryIndex, removal, new ThreadSafeOrderRemovalComponent
                        {
                            RequestedResourceName = task.RequestedResourceName,
                            Orderer = task.ExchangeInventoryEntity,
                            OrdersReceiver = task.TaskGiver,
                            Recipe = task.Recipe,
                            RequestedAmount = task.RequestedResourceAmount
                        });
                    }
                }
                scale.Value = 0f;
                ecb.RemoveComponent<UnitTagComponent>(entityInQueryIndex, entity);
            }).Schedule(inputDeps);

        return jobHandle;
    }
}
