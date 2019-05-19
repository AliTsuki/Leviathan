using UnityEngine;

public class Bolt : Projectile
{
    // Projectile constructor
    public Bolt(uint _id, GameController.IFF _iff, uint _type, float _damage, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        this.ID = _id;
        this.IFF = _iff;
        this.ProjectileType = _type;
        this.Damage = _damage;
        this.Position = _position;
        this.Rotation = _rotation;
        this.Velocity = _velocity;
        this.Speed = _speed;
        this.Lifetime = _lifetime;
        this.ProjectilePrefab = Resources.Load(GameController.ProjectilePrefabName + this.ProjectileType.ToString(), typeof(GameObject)) as GameObject;
        this.ProjectileObject = GameObject.Instantiate(this.ProjectilePrefab, this.Position, this.Rotation);
        this.Start();
    }
}
