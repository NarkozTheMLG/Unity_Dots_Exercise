using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct HealthDeadTestSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged); // This automatically handles buffer!
            
            /*new EntityCommandBuffer(Allocator.Temp);*/
        foreach ((RefRO<Health> health,Entity entity) in SystemAPI.Query<RefRO<Health>>().WithEntityAccess()) {
            if (health.ValueRO.healthAmount <= 0) {
                entityCommandBuffer.DestroyEntity(entity);
                //state.EntityManager.DestroyEntity(entity); // THIS WILL THROW ERROR USE ENTITYCOMMANDBUFFER
            }
        //entityCommandBuffer.Playback(state.EntityManager); // INSTEAD OF THIS USE SYSTEM SINGLETON
        }
    }

  
}
