Unity DOTS & ECS Notes

Core pillars: ECS, Job System, and Burst Compiler.

1. Core Definitions & Source Generation
- Entity: An ID used to group components.
- Component: A struct implementing IComponentData (Data only).
- NativeArray: Special unmanaged array for DOTS. Standard C# arrays (objects) cannot be used in Jobs/Burst.

Partial Structs: Systems and Jobs must be marked partial because Unity’s Source Generator automatically writes the boilerplate code to handle chunk iteration and memory management.

2. MonoBehaviour & Hybrid Setup
```
Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
Plane plane = new Plane(Vector3.up, Vector3.zero);

if(plane.Raycast(mouseCameraRay, out float distance)) {
    return mouseCameraRay.GetPoint(distance);
}
```
Event Handlers (Singleton Pattern)

// Publisher (MyClass)
```
public static UnitSelectionManager Instance { get; private set; }
public event EventHandler OnSelectionAreaStart;
OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
```
// Subscriber (OtherClass)
```
MyClass.Instance.OnSelectionAreaStart += MyClass_OnSelectionAreaStart;
private void MyClass_OnSelectionAreaStart(object sender, EventArgs e) { /* ... */ }
```

3. Querying Entities
- Inside a System (SystemAPI) The easiest way to iterate data
  ```
    foreach (var (tf, speed) in __SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>>()__) {
        tf.ValueRW = tf.ValueRO.RotateY(speed.ValueRO.Value * SystemAPI.Time.DeltaTime);
    }
```
- Outside a System (Manual Query) Use EntityQueryBuilder when SystemAPI is unavailable (e.g., in a MonoBehaviour).
- ```
    EntityManager em = __World.DefaultGameObjectInjectionWorld.EntityManager__;
    EntityQuery query = new EntityQueryBuilder(Allocator.Temp) // Builder Pattern
        .WithAll<UnitMover>()
        .Build(em);
```
// Conversion to Arrays
```
NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
NativeArray<UnitMover> movers = query.ToComponentDataArray<UnitMover>(Allocator.Temp);
```
// Alternative direct creation
```
EntityQuery physicsQuery = em.CreateEntityQuery(typeof(PhysicsWorldSingleton));
```

4. DOTS Physics & Layer Filtering
- Interacting with the DOTS physics world using Raycasts and Filters.
```
PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

RaycastInput raycastInput = new RaycastInput {
    Start = cameraRay.GetPoint(0f),
    End = cameraRay.GetPoint(9999f),
    Filter = new CollisionFilter {
        BelongsTo = ~0u,                // All layers
        CollidesWith = 1u << 6,         // Collide with Layer 6
        GroupIndex = 0
    }
};

if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit)) {
    if (em.HasComponent<Unit>(hit.Entity)) {
        em.SetComponentEnabled<Selected>(hit.Entity, true);
    }
}
```

5. Job System & Performance
- _IJobEntity_: Processes entities in parallel.
- _ScheduleParallel()_: Runs across all cores.
- _Run()_: Executes on the main thread (meaningless for performance; use for debugging only).

Data Syncing Example:
Two ways to update components back to the EntityManager:

// Method A: Manual Set
```
for (int i = 0; i < unitMoverArray.Length; i++) {
    UnitMover mover = unitMoverArray[i];
    mover.targetPosition = target;
    entityManager.SetComponentData(entityArray[i], mover);
}
```
// Method B: Batch Copy __(Faster)__
```
for (int i = 0; i < unitMoverArray.Length; i++) {
    unitMoverArray[i] = new UnitMover { targetPosition = target };
}
entityQuery.CopyFromComponentDataArray(unitMoverArray);
```

6. Component Management
- _IEnableableComponent_: Allows toggling a component without "Structural Changes."
```
entityManager.SetComponentEnabled<Selected>(entity, false);
```
Additional Query Methods:
```
RefRW<LocalTransform> visual = SystemAPI.GetComponentRW<LocalTransform>(entity);
```
7. Useful Filters & Keywords

- .WithAll<T>(): Must have T.
- .WithNone<T>(): Must NOT have T.
- .WithDisabled<T>(): Include if T is disabled.
- .WithEntityAccess(): Adds Entity ID to the result tuple.
  
__WorldUpdateAllocator__:
Where: Used primarily in Systems during OnUpdate.
Why: Faster than Allocator.Temp. It's a "rewindable" memory pool that persists for the duration of the current world update (usually 1-2 frames) and is automatically cleared. No manual disposal required.
How: Use it when creating temporary NativeArrays or EntityCommandBuffers that only need to exist for the current frame.

// Example: Creating an ECB in a System
```
var ecb = new EntityCommandBuffer(WorldUpdateAllocator);
```
