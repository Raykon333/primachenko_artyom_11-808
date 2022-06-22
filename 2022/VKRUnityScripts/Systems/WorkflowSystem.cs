using Unity.Entities;
using Unity.Rendering;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class WorkflowSystem : JobComponentSystem
{
    GameManager gameManager;

    EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnStartRunning()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    float productionTimer = 0f;
    float productionInterval = 1000f;
    float staticProductionDivisor = 2f;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var staticEntityCount = GetEntityQuery(typeof(TimeUniqueComponent)).CalculateEntityCount();
        if (staticEntityCount == 0)
            return inputDeps;
        var staticEntity = GetEntityQuery(typeof(TimeUniqueComponent)).GetSingletonEntity();

        var DeltaTime = Time.DeltaTime;
        DeltaTime *= EntityManager.GetComponentData<StaticComponent>(staticEntity).SimulationSpeed;

        float productionDivisor = staticProductionDivisor;
        bool productionFrame = false;
        productionTimer += DeltaTime;
        int intervals = (int)(productionTimer / productionInterval);
        if (intervals > 0)
        {
            productionFrame = true;
            productionTimer -= productionInterval * intervals;
            productionDivisor /= intervals;
        }    

        var inventoryData = GetBufferFromEntity<InventoryBufferElement>(false);
        var currentLocationData = GetComponentDataFromEntity<CurrentLocationComponent>(true);
        var manufacturerData = GetComponentDataFromEntity<ManufacturerComponent>(true);
        var exchangeTaskData = GetComponentDataFromEntity<ExchangeTaskComponent>(true);
        var needsAndDutiesData = GetComponentDataFromEntity<NeedsAndDutiesComponent>(true);
        var readyTagData = GetComponentDataFromEntity<ReadyToTakeOrderTagComponent>(true);
        var recipeData = GetBufferFromEntity<ExchangeRecipeBufferElement>(true);
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithReadOnly(currentLocationData)
            .WithReadOnly(manufacturerData)
            .WithNativeDisableParallelForRestriction(inventoryData)
            .WithReadOnly(exchangeTaskData)
            .WithReadOnly(needsAndDutiesData)
            .WithReadOnly(readyTagData)
            .WithReadOnly(recipeData)
            .ForEach((int entityInQueryIndex, Entity entity, in VacancyTypeComponent vacancyType, in ParentLocationComponent workplace, in RelatedUnitComponent employee) =>
            {
                if (employee.Entity == default)
                    return;
                var isWorktime = needsAndDutiesData[employee.Entity].IsWorktime;
                if (!inventoryData.HasComponent(workplace.Location))
                    return;
                var inventory = inventoryData[workplace.Location];
                if (vacancyType.Value == VacancyType.Producer)
                {
                    if (!isWorktime)
                        return;
                    if (productionFrame && currentLocationData.HasComponent(employee.Entity) && currentLocationData[employee.Entity].entity == workplace.Location
                    && recipeData.HasComponent(workplace.Location))
                    {
                        var manufacturer = manufacturerData[workplace.Location];
                        for (int i = 0; i < manufacturer.ManufacturedResources.Length; i++)
                        {
                            var manufacturedResource = manufacturer.ManufacturedResources[i];

                            var recipes = recipeData[workplace.Location];
                            var recipe = recipes.GetRecipe(manufacturedResource.RecipeHash);

                            var craft = ecb.CreateEntity(entityInQueryIndex);
                            ecb.AddComponent(entityInQueryIndex, craft, new ThreadSafeCraftComponent
                            {
                                CraftStationEntity = workplace.Location,
                                Recipe = recipe,
                                RequestedResourceName = manufacturedResource.ResourceName,
                                RequestedResourceAmount = manufacturedResource.EfficiencyMultiplier
                            });
                        }
                    }
                }
                if (vacancyType.Value == VacancyType.Transporter)
                {
                    if(isWorktime 
                    && !exchangeTaskData.HasComponent(employee.Entity) 
                    && !readyTagData.HasComponent(employee.Entity))
                    {
                        ecb.AddComponent(entityInQueryIndex, employee.Entity, new ReadyToTakeOrderTagComponent());
                    }
                    else if (!isWorktime && readyTagData.HasComponent(employee.Entity))
                    {
                        ecb.RemoveComponent<ReadyToTakeOrderTagComponent>(entityInQueryIndex, employee.Entity);
                    }
                }
            }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
