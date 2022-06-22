using Unity.Entities;

public class PopulationSystem : ComponentSystem
{
    GameManager gameManager;

    float timer = 0f;
    float interval = 1f;

    protected override void OnUpdate()
    {
        timer += Time.DeltaTime;
        if (timer < interval)
            return;
        timer -= interval;

        var staticEntityCount = GetEntityQuery(typeof(TimeUniqueComponent)).CalculateEntityCount();
        if (staticEntityCount == 0)
            return;
        var staticEntity = GetEntityQuery(typeof(TimeUniqueComponent)).GetSingletonEntity();

        var staticComponent = EntityManager.GetComponentData<StaticComponent>(staticEntity);
        staticComponent.UnitsCount = GetEntityQuery(typeof(UnitTagComponent)).CalculateEntityCount();

        EntityManager.SetComponentData(staticEntity, staticComponent);
    }
}
