using UnityEngine;

// Controls all projectiles
public class Projectile
{
    // GameObjects and Components
    private GameObject projectilePrefab;
    private GameObject projectile;
    private Rigidbody projectileRigidbody;

    // Constructor criteria
    private int id;
    public GameController.IFF IFF;
    public float Damage;
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 velocity;
    private float lifetime;
    private float speed;
    private bool piercingShot = false;
    
    // Alive flag
    public bool Alive = false;
    
    // Projectile constructor
    public Projectile(int _id, GameController.IFF _iff, float _damage, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        this.id = _id;
        this.IFF = _iff;
        this.Damage = _damage;
        this.position = _position;
        this.rotation = _rotation;
        this.velocity = _velocity;
        this.speed = _speed;
        this.lifetime = _lifetime;
        this.Start();
    }


    // Start is called before the first frame update
    public void Start()
    {
        this.projectilePrefab = Resources.Load(GameController.ProjectilePrefabName, typeof(GameObject)) as GameObject;
        this.projectile = GameObject.Instantiate(this.projectilePrefab, this.position, this.rotation);
        this.projectile.name = $@"{this.id}";
        this.projectileRigidbody = this.projectile.GetComponent<Rigidbody>();
        this.projectileRigidbody.velocity = this.velocity;
        this.Alive = true;
    }

    // Update is called once per frame
    public void Update()
    {

    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        if(this.projectile != null)
        {
            this.projectileRigidbody.velocity += this.projectile.transform.forward * this.speed;
            GameObject.Destroy(this.projectile, this.lifetime);
        }
        else
        {
            this.Alive = false;
        }
    }

    // Called when receiving collision
    public void ReceivedCollision()
    {
        if(!this.piercingShot)
        {
            GameObject.Destroy(this.projectile);
            this.Alive = false;
        }
    }
}
