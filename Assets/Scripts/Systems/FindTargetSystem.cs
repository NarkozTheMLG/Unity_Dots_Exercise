
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
        foreach (var(localTransform,findTarget,target) in 
            SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTarget>,RefRW<Target>>()) {
            findTarget.ValueRW.timer -= Time.deltaTime;
            if(findTarget.ValueRO.timer > 0f) {
                continue;
            }
            findTarget.ValueRW.timer = findTarget.ValueRW.timerMax;
            Debug.Log("w");
            distanceHitList.Clear();
            CollisionFilter collisionFilternew = new CollisionFilter {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNITS_LAYER,
                GroupIndex = 0,
            };
            if(collisionWorld.OverlapSphere(localTransform.ValueRO.Position,findTarget.ValueRO.range,ref distanceHitList, collisionFilternew)) 
                foreach (DistanceHit distanceHit in distanceHitList) {
                    Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                    if(targetUnit.faction == findTarget.ValueRO.targetFaction) {
                        target.ValueRW.targetEntity = distanceHit.Entity;
                        break;
                    }
                }

        }
    }

 
}
