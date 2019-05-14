using UnityEngine;

// Controls the enemy ships
public class EnemyShip //: Ship
{
    // GameObjects and Components
    public GameObject ShipObject;
    private GameObject enemyPrefab;
    private Rigidbody shipRigidbody;

    // Ship stats
    // Health/Armor/Shields
    private float health = 0f;
    private float maxHealth = 100f;
    private float armor = 0f;
    private float maxArmor = 100f;
    private float shields = 0f;
    private float maxShields = 100f;
    // Current/Max energy
    private float energy = 0f;
    private float maxEnergy = 100f;
    // Speed/Acceleration
    private float impulseAcceleration = 25f;
    private float warpAccelMultiplier = 3f;
    private float maxImpulseSpeed = 50f;
    private float maxWarpSpeed = 150f;
    // Weapon stats
    private float shotDamage = 10f;
    private float shotSpeed = 10f;
    private float shotLifetime = 2.5f;
    private float shotCurvature = 0f;
    // Cooldowns
    private float shotCooldownTime = 0.25f;
    private float shieldCooldownTime = 10f;
    private float bombCooldownTime = 10f;
    private float scannerCooldownTime = 10f;
    // Energy cost
    private float warpEnergyCost = 5f;
    private float shotEnergyCost = 5f;

    // Constructor criteria
    private int id;
    public GameController.IFF IFF;

    // Alive flag
    public bool Alive = false;

    // Enemy ship constructor
    public EnemyShip(int _id)
    {
        this.id = _id;
        this.IFF = GameController.IFF.enemy;
        this.Start();
    }


    // Start is called before the first frame update
    public void Start()
    {
        this.enemyPrefab = Resources.Load(GameController.EnemyPrefabName, typeof(GameObject)) as GameObject;
        this.ShipObject = GameObject.Instantiate(this.enemyPrefab, new Vector3(0, 0, 15), Quaternion.identity);
        this.ShipObject.name = $@"{this.id}";
        this.shipRigidbody = this.ShipObject.GetComponent<Rigidbody>();
        this.Alive = true;
        this.health = this.maxHealth;
        this.armor = this.maxArmor;
        this.shields = this.maxShields;
        this.energy = this.maxEnergy;
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        if(this.health <= 0)
        {
            this.Kill();
        }
    }

    // Called when receiving collision
    public void ReceivedCollision(float _damage)
    {
        this.health -= _damage;
    }

    // Called when entity is destroyed
    private void Kill()
    {
        this.Alive = false;
        GameObject.Destroy(this.ShipObject);
    }
}
