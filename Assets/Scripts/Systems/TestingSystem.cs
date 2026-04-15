using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct TestingSystem : ISystem
{


    //public void OnUpdate(ref SystemState state)
    //{
    //    int unitCount = 0;
    //    foreach (RefRW<Friendly> friendly in SystemAPI.Query<RefRW<Friendly>>()){
    //       unitCount++; 
    //    }
    //    Debug.Log("U"+unitCount);

    //}
}
