Questions?:
- What are partial structs and what does it have to do with Source Generation?


Some DOTS related methods:
- "World.DefaultGameOnjectInjectionWorld.EntityManager" // I dont know how we can create other worlds?


- You cannot access SystemAPI outside of a System so we need to manually create a queery 
new EntityQueryBuilder(Allocator.Temp)
new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover>() // as name suggests..
new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover>().Build(entityManager); // it takes in entity manager parameter

- Native Array is a special array for DOTS you cannot use C# default array becuase it's object
  
Some Jobs related stuff:
- unitMoverJob.Run(); // this runs on main thread so kinda meaningless, use it for debugging

  
  
  
  
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
