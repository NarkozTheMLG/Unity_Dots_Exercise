Questions?:
- What are partial structs and what does it have to do with Source Generation?

NOTES:
Some MonoBehaviour notes:

- Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition); it takes mouse position and converts it to screen position, an easy way to check if it is in the plane
      Plane plane = new Plane(Vector3.up,Vector3.zero);
      if(plane.Raycast(mouseCameraRay, out float distance))
          return mouseCameraRay.GetPoint(distance);

- EventHandlers:
    MyClass:
     public static UnitSelectionManager Instance {get; private set;} // SINGLETON
     public event EventHandler OnSelectionAreaStart;
     OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
    OtherClass:
     MyClass.Instance.OnSelectionAreaStart += MyClass_OnSelectionAreaStart;
     private void MyClass_OnSelectionAreaStart(object sender, EventArgs e){...}





Some DOTS related methods:
- "World.DefaultGameOnjectInjectionWorld.EntityManager" // I dont know how we can create other worlds?
    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

- You cannot access SystemAPI outside of a System so we need to manually create a queery 
    new EntityQueryBuilder(Allocator.Temp)
    new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover>() // as name suggests..
    new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover>().Build(entityManager); // it takes in entity manager parameter
or you can do it like this
    entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));


- Native Array is a special array for DOTS you cannot use C# default array becuase it's object
  
Some Jobs related stuff:
- unitMoverJob.Run(); // this runs on main thread so kinda meaningless, use it for debugging

ECS Component Methods:
- public struct Selected : IComponentData, IEnableableComponent // this Interface makes it enableable component
- entityManager.SetComponentEnabled<Selected>(entity, false);
- RefRW<LocalTransform> visualTransform = 
SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity); // an additional way to querry

DOTS Physics:
    entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton)); // returns unity's physics thing
    PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();// gets it
    CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;// returns collision world of unity
    UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition); // convert mouse pos to ray
    int unitsLayer = 6; // this is the layer of unit
    RaycastInput raycastInput = new RaycastInput()
    {
        Start = cameraRay.GetPoint(0f),
        End = cameraRay.GetPoint(9999f),
        Filter = new CollisionFilter
        {
            BelongsTo = ~0u, // inverts 0s to 1s ->> 111111
            CollidesWith = 1u << unitsLayer, // means 0000 0001 -> 0100 0000
            GroupIndex = 0, // it can be anything i guess doesnt really matter
        }
    };
    if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))// gets only one thing it hits
    {
        if (entityManager.HasComponent<Unit>(raycastHit.Entity)) 
        {
            entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
        }
    }
  
  //////////////////////
  void Update()
  {
      if (Input.GetMouseButtonDown(1))
      {
          Vector3 mouseWorldPositon = MouseWorldPosition.Instance.GetPosition();

          EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
          EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover>().Build(entityManager);

          NativeArray<Entity> entitiyArray = entityQuery.ToEntityArray(Allocator.Temp);
          NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);

          for (int i = 0; i < unitMoverArray.Length; i++)
          {
              UnitMover unitMover = unitMoverArray[i];
              unitMover.targetPosition = mouseWorldPositon;

              entityManager.SetComponentData(entitiyArray[i], unitMover);
          }
      }
  }

  Alternetive solution
             for (int i = 0; i < unitMoverArray.Length; i++)
           {
               UnitMover unitMover = unitMoverArray[i];
               unitMover.targetPosition = mouseWorldPositon;

              unitMoverArray[i] = unitMover;
           }
            entityQuery.CopyFromComponentDataArray(unitMoverArray);
