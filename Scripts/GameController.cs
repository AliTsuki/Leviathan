using System.Collections.Generic;

using Cinemachine;

using UnityEngine;

// Keeps track of all entites and updates all systems in the game
public static class GameController
{
    // GameObjects and Components
    public static Ship Player;
    private static GameObject CamerasPrefab;
    private static GameObject Cameras;
    private static CinemachineVirtualCamera FollowCamera;
    public static System.Random r = new System.Random();

    // Entity Lists and Dicts
    public static readonly Dictionary<uint, Ship> Ships = new Dictionary<uint, Ship>();
    public static readonly Dictionary<uint, Projectile> Projectiles = new Dictionary<uint, Projectile>();
    private static readonly List<uint> ShipsToRemove = new List<uint>();
    private static readonly List<uint> ProjectilesToRemove = new List<uint>();

    // Enemy spawn fields
    private static uint EnemyCount = 0;
    private readonly static uint MaxEnemyCount = 10;
    private readonly static int MinEnemySpawnDistance = 40;
    private readonly static int MaxEnemySpawnDistance = 100;
    private readonly static int EnemyDespawnDistance = 200;
    private static Vector3 NextEnemySpawnPosition;

    // Constant references to Prefab filenames
    public const string CamerasPrefabName = "Cameras";
    public const string FollowCameraName = "Follow Camera";
    public const string UIPrefabName = "UI";
    public const string CanvasName = "Canvas";
    public const string ShieldDamageEffectName = "Shield Damage Effect";
    public const string HealthDamageEffectName = "Health Damage Effect";
    public const string PlayerUIPrefabName = "Player UI";
    public const string NPCUIPrefabName = "NPC UI";
    public const string InfoLabelName = "Info Label";
    public const string BackgroundPrefabName = "Background";
    public const string PlayerPrefabName = "Player Ship";
    public const string FriendPrefabName = "Player Ship";
    public const string EnemyPrefabName = "Enemy Ship";
    public const string ProjectilePrefabName = "Projectile";
    public const string ProjectileShieldStrikePrefabName = "ProjectileShieldStrike";
    public const string ProjectileHullStrikePrefabName = "ProjectileHullStrike";
    public const string ExplosionPrefabName = "Explosion";

    // Entity IDs
    private static uint ShipID = 0;
    private static uint ProjectileID = 0;
    private static bool ShipIDPassedMax = false;

    // Score
    public static uint Score;

    // Identify Friend or Foe
    public enum IFF
    {
        Friend,
        Enemy
    };


    // Start is called before the first frame update
    public static void Start()
    {
        // Initialize UI
        UIController.Start();
        // Spawn Camera
        CamerasPrefab = Resources.Load(CamerasPrefabName, typeof(GameObject)) as GameObject;
        Cameras = GameObject.Instantiate(CamerasPrefab);
        Cameras.name = "Cameras";
        // Spawn player
        SpawnPlayer();
        // Set up follow camera
        FollowCamera = GameObject.Find(FollowCameraName).GetComponent<CinemachineVirtualCamera>();
        FollowCamera.Follow = Player.ShipObject.transform;
        // Initialize background
        Background.Start();
    }

    // Update is called once per frame
    public static void Update()
    {
        // Update all ships
        foreach(KeyValuePair<uint, Ship> ship in Ships)
        {
            // Checks if ship is alive
            if(ship.Value.Alive)
            {
                // Update ship
                ship.Value.Update();
            }
        }
        // Checks if enemies should be spawned and calls SpawnEnemy
        if(ShouldSpawnEnemies())
        {
            // Get next random spawn position for enemy ship
            int nextXSpawn = r.Next(MinEnemySpawnDistance, MaxEnemySpawnDistance);
            int nextZSpawn = r.Next(MinEnemySpawnDistance, MaxEnemySpawnDistance);
            if(r.Next(0, 1) == 1)
            {
                nextXSpawn *= -1;
            }
            if(r.Next(0, 1) == 1)
            {
                nextZSpawn *= -1;
            }
            NextEnemySpawnPosition = new Vector3(Player.ShipObject.transform.position.x + nextXSpawn, 0, Player.ShipObject.transform.position.z + nextZSpawn);
            // Spawn enemy ship at specified position
            SpawnEnemy(NextEnemySpawnPosition);
        }
        // Update Background
        Background.Update();
        // Update UI
        UIController.Update();
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public static void FixedUpdate()
    {
        // FixedUpdate all ships
        foreach(KeyValuePair<uint, Ship> ship in Ships)
        {
            // Checks if ship is alive
            if(ship.Value.Alive)
            {
                // Physics update ship
                ship.Value.FixedUpdate();
            }
            // If ship is dead
            else
            {
                // Add dead ship to removal list
                ShipsToRemove.Add(ship.Key);
                // If ship to remove is an enemy
                if(ship.Value.IFF == IFF.Enemy)
                {
                    // Lower the enemy count
                    EnemyCount--;
                }
            }
        }
        // FixedUpdate all projectiles
        foreach(KeyValuePair<uint, Projectile> projectile in Projectiles)
        {
            // Checks if projectile is alive
            if(projectile.Value.Alive)
            {
                // Physics update projectile
                projectile.Value.FixedUpdate();
            }
            // If projectile is dead
            else
            {
                // Add projectile to removal list
                ProjectilesToRemove.Add(projectile.Key);
            }
        }
        // Remove dead ships
        if(ShipsToRemove.Count > 0)
        {
            // Loop through ship removal list
            foreach(uint ID in ShipsToRemove)
            {
                // Remove ship
                Ships.Remove(ID);
            }
            // Clear ship removal list
            ShipsToRemove.Clear();
        }
        // Remove dead projectiles
        if(ProjectilesToRemove.Count > 0)
        {
            // Loop through projectile removal list
            foreach(uint ID in ProjectilesToRemove)
            {
                // Remove projectile
                Projectiles.Remove(ID);
            }
            // Clear projectile removal list
            ProjectilesToRemove.Clear();
        }
        if(Player.Alive == true)
        {
            // Check if any enemies are too far away and despawn them
            DespawnEnemies();
        }
    }

    // Initialize player ship in world
    public static void SpawnPlayer()
    {
        Ships.Add(ShipID, new PlayerShip(ShipID));
        Player = Ships[ShipID];
        ShipID++;
        GetNextEntityID();
    }

    // Spawn friendly ships
    public static void SpawnFriendly()
    {
        Ships.Add(ShipID, new FriendlyShip(ShipID));
        ShipID++;
        GetNextEntityID();
    }

    // Spawn enemy ships
    public static void SpawnEnemy(Vector3 _startingPosition)
    {
        Ships.Add(ShipID, new EnemyShip(ShipID, _startingPosition));
        EnemyCount++;
        ShipID++;
        GetNextEntityID();
    }

    // Spawn projectiles
    public static void SpawnProjectile(IFF _iff, uint _type, float _damage, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        Projectiles.Add(ProjectileID, new Projectile(ProjectileID, _iff, _type, _damage, _position, _rotation, _velocity, _speed, _lifetime));
        ProjectileID++;
    }

    // Once entityID has passed max value and overflowed, check each ID to see if already exists recursively
    public static void GetNextEntityID()
    {
        if(ShipID == uint.MaxValue)
        {
            ShipIDPassedMax = true;
        }
        else if(ShipIDPassedMax == true)
        {
            if(Ships.ContainsKey(ShipID))
            {
                ShipID++;
                GetNextEntityID();
            }
        }
    }

    // Recieve Collision info and propogate
    public static void Collide(GameObject _collisionReporter, GameObject _collidedWith)
    {
        // Checks if the object reporting collision is a projectile
        if(_collisionReporter.tag == "Projectile")
        {
            // Get the projectile
            Projectile projectile = Projectiles[uint.Parse(_collisionReporter.name)];
            // If projectile is friendly and has collided with an enemy
            if(projectile.IFF == IFF.Friend && _collidedWith.tag == "Enemy")
            {
                // Get the enemy ship
                Ship enemy = Ships[uint.Parse(_collidedWith.name)];
                // Run ReceivedCollision for projectile
                projectile.ReceivedCollision();
                // Run ReceivedCollision for enemy
                enemy.ReceivedCollisionFromProjectile(projectile.Damage, projectile.ProjectileObject.transform.position);
            }
            // If projectile is enemy and has collided with a player
            else if(projectile.IFF == IFF.Enemy && _collidedWith.tag == "Player")
            {
                // Get player ship
                Ship player = Ships[uint.Parse(_collidedWith.name)];
                // Run ReceivedCollision for projectile
                projectile.ReceivedCollision();
                // Run ReceivedCollision for player
                player.ReceivedCollisionFromProjectile(projectile.Damage, projectile.ProjectileObject.transform.position);
            }
            // If projectile is enemy and has collided with a friendly ship
            else if(projectile.IFF == IFF.Enemy && _collidedWith.tag == "Friend")
            {
                // Get friendly ship
                Ship friend = Ships[uint.Parse(_collidedWith.name)];
                // Run ReceivedCollision for projectile
                projectile.ReceivedCollision();
                // Run ReceivedCollision for friendly ship
                friend.ReceivedCollisionFromProjectile(projectile.Damage, projectile.ProjectileObject.transform.position);
            }
        }
        // If two ships collided
        else if(_collisionReporter.tag != "Projectile" && _collidedWith.tag != "Projectile")
        {
            // Get ships
            Ship reporter = Ships[uint.Parse(_collisionReporter.name)];
            Ship collidedWith = Ships[uint.Parse(_collidedWith.name)];
            // Send collision report
            reporter.ReceivedCollisionFromShip(collidedWith.ShipRigidbody.velocity, collidedWith.IFF);
            collidedWith.ReceivedCollisionFromShip(reporter.ShipRigidbody.velocity, reporter.IFF);
        }
    }

    // Checks if enemies should be spawned
    private static bool ShouldSpawnEnemies()
    {
        // If current enemy count is below max allowed return true, otherwise false
        if(EnemyCount < MaxEnemyCount && Player.Alive == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Checks if enemies should be despawned
    private static void DespawnEnemies()
    {
        // Loop through all ships
        foreach(KeyValuePair<uint, Ship> ship in Ships)
        {
            // If ship is enemy and further from player than despawn distance
            if(ship.Value.Alive && ship.Value.IFF == IFF.Enemy && Vector3.Distance(Player.ShipObject.transform.position, ship.Value.ShipObject.transform.position) >= EnemyDespawnDistance)
            {
                // Despawn enemy ship
                ship.Value.Despawn();
            }
        }
    }
}
