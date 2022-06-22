using Unity.Entities;

public class TimeSystem : ComponentSystem
{
    GameManager gameManager;

    public static readonly float secondsInDay = 86400;

    protected override void OnUpdate()
    {
        var staticEntityCount = GetEntityQuery(typeof(TimeUniqueComponent)).CalculateEntityCount();
        if (staticEntityCount == 0)
            return;
        var staticEntity = GetEntityQuery(typeof(TimeUniqueComponent)).GetSingletonEntity();

        var deltaTime = Time.DeltaTime;
        deltaTime *= EntityManager.GetComponentData<StaticComponent>(staticEntity).SimulationSpeed;

        var time = EntityManager.GetComponentData<TimeUniqueComponent>(staticEntity);

        var day = time.Day;
        var seconds = time.Seconds;

        seconds += deltaTime;

        while (seconds >= secondsInDay)
        {
            day++;
            seconds -= secondsInDay;
        }

        EntityManager.SetComponentData(staticEntity, new TimeUniqueComponent { Day = day, Seconds = seconds });
    }
}
