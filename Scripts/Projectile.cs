using UnityEngine;

// Controls all projectiles
public class Projectile
{
    private GameObject ProjectilePrefab;
    private GameObject projectile;
    private Rigidbody projectileRigidbody;

    private int id;
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 velocity;
    private float lifetime;
    private float speed;
    public bool Alive = false;
    

    public Projectile(int _id, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        this.id = _id;
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
        this.ProjectilePrefab = Resources.Load(GameController.ProjectilePrefabName, typeof(GameObject)) as GameObject;
        this.projectile = GameObject.Instantiate(this.ProjectilePrefab, this.position, this.rotation);
        this.projectile.name = $@"Projectile: {this.id}";
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
}
