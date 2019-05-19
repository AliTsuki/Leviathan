using System.Collections.Generic;

using UnityEngine;

public class Bomb : Projectile
{
    // Constructor criteria
    private readonly PlayerShip Player;
    private readonly GameObject BombExplosionPrefab;
    private GameObject BombExplosionObject;
    private readonly float FiredTime;
    private readonly float Radius;

    // Projectile constructor
    public Bomb(PlayerShip _player, uint _id, GameController.IFF _iff, float _damage, float _radius, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        this.Player = _player;
        this.ID = _id;
        this.IFF = _iff;
        this.Damage = _damage;
        this.Radius = _radius;
        this.Position = _position;
        this.Rotation = _rotation;
        this.Velocity = _velocity;
        this.Speed = _speed;
        this.Lifetime = _lifetime;
        this.ProjectilePrefab = Resources.Load(GameController.BombPrefabName, typeof(GameObject)) as GameObject;
        this.ProjectileObject = GameObject.Instantiate(this.ProjectilePrefab, this.Position, this.Rotation);
        this.BombExplosionPrefab = Resources.Load(GameController.BombExplostionPrefabName, typeof(GameObject)) as GameObject;
        this.FiredTime = Time.time;
        this.Start();
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public override void FixedUpdate()
    {
        if(Time.time - this.FiredTime <= this.Lifetime)
        {
            // If projectile still exists
            if(this.ProjectileObject != null)
            {
                // Accelerate forward
                this.ProjectileRigidbody.velocity += this.ProjectileObject.transform.forward * this.Speed;
            }
            // If projectile has burnt out
            else
            {
                // Set to dead
                this.Alive = false;
            }
        }
        else
        {
            this.Player.BombInFlight = false;
            this.Detonate();
        }

    }

    // Detonate bomb
    public void Detonate()
    {
        this.BombExplosionObject = GameObject.Instantiate(this.BombExplosionPrefab, this.ProjectileObject.transform.position, Quaternion.identity);
        GameObject.Destroy(this.BombExplosionObject, 1);
        foreach(KeyValuePair<uint, Ship> ship in GameController.Ships)
        {
            if(ship.Value.IFF != this.IFF && ship.Value.Alive == true)
            {
                float distance = Vector3.Distance(this.ProjectileObject.transform.position, ship.Value.ShipObject.transform.position);
                if(distance <= this.Radius)
                {
                    ship.Value.TakeDamage(this.Damage - ((distance / this.Radius) * this.Damage));
                }
            }
        }
        this.Alive = false;
        GameObject.Destroy(this.ProjectileObject);
    }
}
