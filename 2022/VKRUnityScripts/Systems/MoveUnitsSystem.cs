using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(PathfindingSystem))]
public class MoveUnitsJobSystem : JobComponentSystem
{
    GameManager gameManager;

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
        DeltaTime *= EntityManager.GetComponentData<StaticComponent>(staticEntity).SimulationSpeed / 1000;

        var TranslationData = GetComponentDataFromEntity<Translation>(false);
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithNativeDisableParallelForRestriction(TranslationData)
            .ForEach((int entityInQueryIndex, Entity entity, in WaypointGoalComponent waypointGoal, in SpeedComponent speed) =>
        {
            var waypoint = waypointGoal.Waypoint;
            var translation = TranslationData[entity].Value;
            var destinationTranslation = Vector3ToFloat3(waypoint);

            float movedDistance = DeltaTime * speed.Value;
            Vector3 movementVector = Float3ToVector3(destinationTranslation - translation);

            Vector3 newTranslate = Float3ToVector3(translation);
            if (movedDistance < movementVector.magnitude)
                newTranslate += movementVector.normalized * movedDistance;
            else
            {
                newTranslate += movementVector;
                ecb.RemoveComponent<WaypointGoalComponent>(entityInQueryIndex, entity);
            }

            TranslationData[entity] = new Translation { Value = Vector3ToFloat3(newTranslate) };
        }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }

    public static float3 Vector3ToFloat3(Vector3 vector)
    {
        return new float3(vector.x, vector.y, vector.z);
    }

    public static Vector3 Float3ToVector3(float3 vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }
}
