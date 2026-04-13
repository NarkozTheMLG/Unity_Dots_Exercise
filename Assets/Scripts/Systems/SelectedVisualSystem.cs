using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SelectedVisualSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
        {
            RefRW<LocalTransform> visualTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
            visualTransform.ValueRW.Scale = 0f;
        }
        foreach (var selected in SystemAPI.Query<RefRO<Selected>>())
        {
            RefRW<LocalTransform> visualTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
            visualTransform.ValueRW.Scale = selected.ValueRO.showScale;
        }

    }


}
