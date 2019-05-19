using UnityEngine;

// Controls all projectiles
public class Projectile
{
    // GameObjects and Components
    public GameObject ProjectilePrefab;
    public GameObject ProjectileObject;
    public Rigidbody ProjectileRigidbody;

    // Constructor criteria
    public uint ProjectileType;
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


    // Start is called before the first frame update
    public void Start()
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
                // Accelerate forward
                this.ProjectileRigidbody.velocity += this.ProjectileObject.transform.forward * this.Speed;
                // Set timer for projectile to burn out
                GameObject.Destroy(this.ProjectileObject, this.Lifetime);
            }
            // If projectile has burnt out
            else
            {
                // Set to dead
                this.Alive = false;
            }
        }
    }

    // Called when receiving collision
    public void ReceivedCollision()
    {
        // Check if shot has piercing abilities
        if(!this.PiercingShot)
        {
            // Destroy projectile object on collision
            GameObject.Destroy(this.ProjectileObject);
            // Set to dead
            this.Alive = false;
        }
    }
}
