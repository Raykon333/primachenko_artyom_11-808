using Unity.Entities;
using Unity.Jobs;

[UpdateBefore(typeof(DecisionMakingSystem))]
public class WorktimeSystem : JobComponentSystem
{
    GameManager gameManager;

    float timer = 0f;
    float interval = 0.1f;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        timer += Time.DeltaTime;
        if (timer < interval)
            return inputDeps;
        else
            timer = 0;

        var staticEntityCount = GetEntityQuery(typeof(TimeUniqueComponent)).CalculateEntityCount();
        if (staticEntityCount == 0)
            return inputDeps;
        var staticEntity = GetEntityQuery(typeof(TimeUniqueComponent)).GetSingletonEntity();

        var time = EntityManager.GetComponentData<TimeUniqueComponent>(staticEntity);

        var weekScheduleData = GetComponentDataFromEntity<WeekScheduleComponent>(true);
        var employmentData = GetComponentDataFromEntity<EmploymentComponent>(true);

        JobHandle jobHandle = Entities
            .WithReadOnly(weekScheduleData)
            .WithReadOnly(employmentData)
            .WithAll<UnitTagComponent>()
            .ForEach((Entity entity, ref NeedsAndDutiesComponent needsAndDuties) =>
            {
                bool isWorktime = false;

                if (employmentData.HasComponent(entity))
                {
                    var weekSchedule = weekScheduleData[employmentData[entity].Vacancy];
                    TimeInterval todaysSchedule = weekSchedule[time.Day];
                    if (time.Seconds >= (todaysSchedule.StartHour * 60 + todaysSchedule.StartMinute) * 60
                        && time.Seconds <= (todaysSchedule.EndHour * 60 + todaysSchedule.EndMinute) * 60)
                    {
                        isWorktime = true;
                    }
                }

                needsAndDuties.IsWorktime = isWorktime;
            }).Schedule(inputDeps);

        return jobHandle;
    }
}
