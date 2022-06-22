using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(DecisionMakingSystem))]
public class PathfindingSystem : JobComponentSystem
{
    GameManager gameManager;

    EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnStartRunning()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        var TranslationData = GetComponentDataFromEntity<Translation>(false);

        Entities
            .ForEach((Entity entity, in ParentLocationComponent home, in RadialComponent radial) =>
            {

                TranslationData[entity] = new Translation
                {
                    Value = TranslationData[home.Location].Value
                    + Vector3ToFloat3(BonusVector(radial))
                };
            }).Run();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var staticEntityCount = GetEntityQuery(typeof(TimeUniqueComponent)).CalculateEntityCount();
        if (staticEntityCount == 0)
            return inputDeps;
        var staticEntity = GetEntityQuery(typeof(TimeUniqueComponent)).GetSingletonEntity();

        var spacingMultiplier = EntityManager.GetComponentData<StaticComponent>(staticEntity).SpacingMultiplier;

        var TranslationData = GetComponentDataFromEntity<Translation>(true);
        var currentLocationData = GetComponentDataFromEntity<CurrentLocationComponent>(true);
        var locationTagData = GetComponentDataFromEntity<LocationTagComponent>(true);
        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle jobHandle = Entities
            .WithNone<WaypointGoalComponent>()
            .WithReadOnly(currentLocationData)
            .WithReadOnly(TranslationData)
            .WithReadOnly(locationTagData)
            .ForEach((int entityInQueryIndex, Entity entity, in MovementTargetComponent movementTarget, in RadialComponent radial) => {
                if (movementTarget.Destination == Entity.Null)
                {
                    ecb.RemoveComponent<MovementTargetComponent>(entityInQueryIndex, entity);
                    return;
                }
                if (currentLocationData.HasComponent(entity))
                {
                    if (currentLocationData[entity].entity == movementTarget.Destination)
                    {
                        ecb.RemoveComponent<MovementTargetComponent>(entityInQueryIndex, entity);
                        return;
                    }
                    ecb.RemoveComponent<CurrentLocationComponent>(entityInQueryIndex, entity);
                }

                var translation = TranslationData[entity].Value;
                var locationTranslation = TranslationData[movementTarget.Destination].Value;
                var waypoint = GetWaypoint(translation, locationTranslation, radial, spacingMultiplier, locationTagData.HasComponent(movementTarget.Destination));

                ecb.AddComponent(entityInQueryIndex, entity, new WaypointGoalComponent { Waypoint = waypoint });

                if (Float3ToVector3(translation) == waypoint)
                {
                    ecb.RemoveComponent<MovementTargetComponent>(entityInQueryIndex, entity);
                    ecb.AddComponent(entityInQueryIndex, entity, new CurrentLocationComponent { entity = movementTarget.Destination });
                }
        }).Schedule(inputDeps);

        ecbSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }

    public static Vector3 GetWaypoint(Vector3 translation, Vector3 destination, RadialComponent radial, float spacingMultiplier, bool addBonusVector)
    {
        Vector3 waypoint;
        float error = 0.01f;

        if (Mathf.Abs(translation.x - destination.x) <= spacingMultiplier / 2 + error
            && Mathf.Abs(translation.z - destination.z) <= spacingMultiplier / 2 + error)
        {
            waypoint = destination;
            if (addBonusVector)
                waypoint += BonusVector(radial);
        }
        else
        {
            var step = spacingMultiplier / 2;
            var closeX = destination.x + (translation.x > destination.x ? step : -step);
            var closeZ = destination.z + (translation.z > destination.z ? step : -step);
            var sign = translation.z > destination.z ? 1 : -1;
            var blocksAway = (int)((translation.z - destination.z + step * sign) / step / 2) - sign;

            bool isOnCloseVertical = translation.x == closeX;
            bool isOnCloseHorizontal = translation.z == closeZ;
            bool isOnFarVertical = (translation.x - closeX) % spacingMultiplier < error;
            bool isOnFarHorizontal = (translation.z - closeZ) % spacingMultiplier < error;


            if (isOnCloseVertical)
                waypoint = new Vector3(translation.x, 0, destination.z);
            else if (isOnCloseHorizontal)
                waypoint = new Vector3(destination.x, 0, translation.z);
            else if (isOnFarVertical)
                waypoint = new Vector3(translation.x, 0, closeZ);
            else if (isOnFarHorizontal)
                waypoint = new Vector3(closeX, 0, translation.z);
            else
                waypoint = new Vector3(translation.x, 0, closeZ + step * 2 * blocksAway);
        }

        return waypoint;
    }

    public static Vector3 BonusVector(RadialComponent radial)
    {
        return new Vector3(1.5f * Mathf.Sin(radial.Angle) * radial.RadiusMult, 0.05f, -0.3f + 0.7f * Mathf.Cos(radial.Angle) * radial.RadiusMult);
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
