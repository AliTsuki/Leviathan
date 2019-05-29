using UnityEngine;

// Controls all projectiles
public class Projectile
{
    // GameObjects and Components
    public GameObject ProjectilePrefab;
    public GameObject ProjectileObject;
    public Rigidbody ProjectileRigidbody;
    public Ship Target;

    // Constructor criteria
    public uint ProjectileType;
    public float Curvature;
    public float SightCone;
    public float Damage;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Velocity;
    public float Lifetime;
    public float Speed;
    public bool PiercingShot = false;

    // Identification fields
    public uint ID;
    public GameController.IFF IFF;
    public bool Alive = false;


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
        if(this.Alive == true)
        {
            // If projectile still exists
            if(this.ProjectileObject != null)
            {
                // If shot has curvature
                if(this.Curvature > 0)
                {
                    // If there is no current target or current target is dead
                    if(this.Target == null || this.Target.Alive == false)
                    {
                        // Acquire new target
                        this.Target = AIController.AcquireForwardTarget(this.ProjectileObject.transform, this.IFF, 30, this.SightCone);
                    }
                    // If there is a target and it is alive
                    else if(this.Target != null && this.Target.Alive == true)
                    {
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
                GameController.ProjectilesToRemove.Add(this.ID);
            }
        }
    }

    // Rotate toward target
    public void RotateToTarget()
    {
        // Get rotation to face target
        Quaternion IntendedRotation = AIController.GetRotationToTarget(this.ProjectileObject.transform, this.Target.ShipObject.transform.position);
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
