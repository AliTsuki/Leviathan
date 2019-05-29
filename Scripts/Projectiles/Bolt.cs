using UnityEngine;

public class Bolt : Projectile
{
    // Projectile constructor
    public Bolt(uint _id, GameController.IFF _iff, uint _type, float _curvature, float _sightCone, float _damage, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        this.ID = _id;
        this.IFF = _iff;
        this.ProjectileType = _type;
        this.Curvature = _curvature;
        this.SightCone = _sightCone;
        this.Damage = _damage;
        this.Position = _position;
        this.Rotation = _rotation;
        this.Velocity = _velocity;
        this.Speed = _speed;
        this.Lifetime = _lifetime;
        this.ProjectilePrefab = Resources.Load<GameObject>(GameController.ProjectilePrefabName + this.ProjectileType.ToString());
        this.ProjectileObject = GameObject.Instantiate(this.ProjectilePrefab, this.Position, this.Rotation);
        GameObject.Destroy(this.ProjectileObject, this.Lifetime);
        this.Initialize();
    }
}
