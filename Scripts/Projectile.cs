using UnityEngine;

// Controls all projectiles
public class Projectile
{
    // GameObjects and Components
    private GameObject ProjectilePrefab;
    private GameObject ProjectileObject;
    private Rigidbody ProjectileRigidbody;

    // Constructor criteria
    private uint ProjectileType;
    public float Damage;
    private Vector3 Position;
    private Quaternion Rotation;
    private Vector3 Velocity;
    private readonly float Lifetime;
    private readonly float Speed;
    private bool PiercingShot = false;

    // Identification fields
    public uint ID;
    public GameController.IFF IFF;
    public bool Alive = false;

    // Projectile constructor
    public Projectile(uint _id, GameController.IFF _iff, uint _type, float _damage, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
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
        this.Start();
    }


    // Start is called before the first frame update
    public void Start()
    {
        // Set up universal projectile fields
        this.ProjectilePrefab = Resources.Load(GameController.ProjectilePrefabName + this.ProjectileType.ToString(), typeof(GameObject)) as GameObject;
        this.ProjectileObject = GameObject.Instantiate(this.ProjectilePrefab, this.Position, this.Rotation);
        this.ProjectileObject.name = $@"{this.ID}";
        this.ProjectileRigidbody = this.ProjectileObject.GetComponent<Rigidbody>();
        this.ProjectileRigidbody.velocity = this.Velocity;
        this.Alive = true;
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public void FixedUpdate()
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
