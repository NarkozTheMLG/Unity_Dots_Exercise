using Unity.Entities;
using UnityEngine;
using UnityEngine.VFX;

public class SelectedAuthoring : MonoBehaviour
{
    public GameObject visualEntity;
    public float showScale;

    public class NewBakerScriptBaker : Baker<SelectedAuthoring>
    {
        public override void Bake(SelectedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selected()
            {
                visualEntity = GetEntity(authoring.visualEntity, TransformUsageFlags.Dynamic),
                showScale = authoring.showScale,
            });
            SetComponentEnabled<Selected>(entity, false);
        }
    }

}

public struct Selected : IComponentData, IEnableableComponent
{
    public Entity visualEntity;
    public float showScale;

    public bool onSelect;
    public bool onDeselect;
}
