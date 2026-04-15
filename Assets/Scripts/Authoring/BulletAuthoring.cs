using Unity.Entities;
using UnityEngine;
public class BulletAuthoring : MonoBehaviour {
    public float speed;
    public int damage;

    public class Baker : Baker<BulletAuthoring> {
        public override void Bake(BulletAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bullet() {
                speed = authoring.speed,
                damage = authoring.damage,
            });
        }
    }

}
public struct Bullet : IComponentData {
    public float speed;
    public int damage;
}