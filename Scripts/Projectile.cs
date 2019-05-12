using UnityEngine;

// Controls all projectiles
public class Projectile
{
    private GameController instance = GameManager.instance;

    private GameObject projectile;
    private Rigidbody projectileRigidbody;

    private int ID;
    private Vector3 Position;
    private Quaternion Rotation;
    private float Lifetime = 2.5f;
    private float Speed = 50f;
    

    public Projectile(int _id, Vector3 _position, Quaternion _rotation, float _speed, float _lifetime)
    {
        this.ID = _id;
        this.Position = _position;
        this.Rotation = _rotation;
        this.Speed = _speed;
        this.Lifetime = _lifetime;
    }


    // Start is called before the first frame update
    public void Start()
    {
        projectile = GameObject.Instantiate(GameManager.ProjectilePrefabStatic, Position, Rotation);
        projectile.name = $@"Projectile: {ID}";
        projectile.transform.parent = GameManager.ProjectileParentStatic.transform;
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void Update()
    {

    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        projectileRigidbody.velocity = projectile.transform.forward * Speed;
        GameObject.Destroy(projectile, Lifetime);
    }
}
