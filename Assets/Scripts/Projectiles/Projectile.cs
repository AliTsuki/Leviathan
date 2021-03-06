﻿using UnityEngine;

// Controls all projectiles
public abstract class Projectile
{
    // GameObjects and Components
    public GameObject ProjectilePrefab { get; protected set; }
    public GameObject ProjectileObject { get; protected set; }
    public Rigidbody ProjectileRigidbody { get; protected set; }
    public Ship Target { get; protected set; }

    // Constructor criteria
    public uint ProjectileType { get; protected set; } = 0;
    public float Curvature { get; protected set; } = 0f;
    public float SightCone { get; protected set; } = 0f;
    public float Damage { get; protected set; } = 0f;
    public Vector3 Position { get; protected set; } = new Vector3();
    public Quaternion Rotation { get; protected set; } = new Quaternion();
    public Vector3 Velocity { get; protected set; } = new Vector3();
    public float Lifetime { get; protected set; } = 0f;
    public float Speed { get; protected set; } = 0f;
    public bool PiercingShot { get; protected set; } = false;

    // Identification fields
    public uint ID { get; protected set; }
    public GameController.IFF IFF { get; protected set; }
    public bool Alive { get; protected set; } = false;


    // Initialize
    public void Initialize()
    {
        // Set up universal projectile fields
        this.ProjectileObject.name = $@"{this.ID}";
        this.ProjectileRigidbody = this.ProjectileObject.GetComponent<Rigidbody>();
        this.ProjectileRigidbody.velocity = this.Velocity;
        this.Alive = true;
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public virtual void FixedUpdate()
    {
        // If this is alive and object still exists
        if(this.Alive == true && this.ProjectileObject != null)
        {
            // If shot has curvature
            if(this.Curvature > 0)
            {
                // If there is no current target or current target is dead
                if(this.Target == null || this.Target.Alive == false)
                {
                    // Acquire new target
                    this.Target = Targeting.AcquireForwardTarget(this.ProjectileObject.transform, this.IFF, 30, this.SightCone);
                }
                // If there is a target and it is alive
                else if(this.Target != null && this.Target.Alive == true)
                {
                    // Rotate toward target
                    this.RotateToTarget();
                }
            }
            // Accelerate forward
            this.ProjectileRigidbody.velocity = this.ProjectileObject.transform.forward * this.Speed;
        }
        // If projectile has burnt out
        else
        {
            // Set to dead
            this.Alive = false;
            // Add to projectile removal list
            GameController.AddProjectileToRemovalList(this.ID);
        }
    }

    // Rotate toward target
    private void RotateToTarget()
    {
        // Get rotation to face target
        Quaternion IntendedRotation = Targeting.GetRotationToTarget(this.ProjectileObject.transform, this.Target.ShipObject.transform.position);
        // Get current rotation
        Quaternion CurrentRotation = this.ProjectileObject.transform.rotation;
        // Get next rotation by using intended rotation and max rotation speed
        Quaternion NextRotation = Quaternion.Lerp(CurrentRotation, IntendedRotation, this.Curvature);
        // Rotate to next rotation
        this.ProjectileObject.transform.rotation = NextRotation;
    }

    // Called when receiving collision
    public void ReceivedCollision()
    {
        // Check if shot has piercing abilities
        if(!this.PiercingShot)
        {
            // Destroy projectile object on collision
            GameObject.Destroy(this.ProjectileObject);
        }
    }
}
