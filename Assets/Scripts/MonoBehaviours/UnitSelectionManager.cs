using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPositon = MouseWorldPosition.Instance.GetPosition();
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);

            for (int i = 0; i < unitMoverArray.Length; i++)
            {
                UnitMover unitMover = unitMoverArray[i];
                unitMover.targetPosition = mouseWorldPositon;

                entityManager.SetComponentData(entityArray[i], unitMover);
            }
        }
    }
}
