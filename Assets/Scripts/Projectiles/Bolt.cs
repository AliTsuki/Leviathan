using UnityEngine;

public class Bolt : Projectile
{
    // Projectile constructor
    public Bolt(uint id, GameController.IFF iff, uint type, float curvature, float sightCone, float damage, Vector3 position, 
        Quaternion rotation, Vector3 velocity, float speed, float lifetime)
    {
        this.ID = id;
        this.IFF = iff;
        this.ProjectileType = type;
        this.Curvature = curvature;
        this.SightCone = sightCone;
        this.Damage = damage;
        this.Position = position;
        this.Rotation = rotation;
        this.Velocity = velocity;
        this.Speed = speed;
        this.Lifetime = lifetime;
        this.ProjectilePrefab = Resources.Load<GameObject>(GameController.ProjectilePrefabName + this.ProjectileType.ToString());
        this.ProjectileObject = GameObject.Instantiate(this.ProjectilePrefab, this.Position, this.Rotation);
        GameObject.Destroy(this.ProjectileObject, this.Lifetime);
        this.Initialize();
    }
}
