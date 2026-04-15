using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityComEntityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<LocalTransform> localTransform, RefRO<Bullet> bullet, RefRO<Target> target, Entity entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Bullet>, RefRO<Target>>().WithEntityAccess()) {

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position,targetLocalTransform.Position);
            
            float3 moveDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float distanceAfterq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);

            if(distanceBeforeSq < distanceAfterq)
                localTransform.ValueRW.Position = targetLocalTransform.Position;
            float destroyDistanceSq = 0.2f;
            if (math.distancesq(localTransform.ValueRO.Position,targetLocalTransform.Position) < destroyDistanceSq) {
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damage;
            
                entityComEntityCommandBuffer.DestroyEntity(entity);
            }
        }
    }


}
