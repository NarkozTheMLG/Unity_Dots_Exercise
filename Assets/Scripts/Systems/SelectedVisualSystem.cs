using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(ResetEventsSystem))]
partial struct SelectedVisualSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>()) {
            if (selected.ValueRO.onDeselect) {
                RefRW<LocalTransform> visualTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualTransform.ValueRW.Scale = 0f;
            }
            if (selected.ValueRO.onSelect) {
                RefRW<LocalTransform> visualTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualTransform.ValueRW.Scale = selected.ValueRO.showScale;
            }
        }
    }
}
