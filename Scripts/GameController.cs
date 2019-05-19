using System.Collections.Generic;

using Cinemachine;

using UnityEngine;

// GENERAL STUFF TO DO
// TODO: Add a main menu with a new game button, a load game button, a settings button to configure inputs and maybe graphics, and a quit button
// TODO: Add a game state, (MainMenu, Playing, Paused, etc) and change GameController.Update to depend on game state
// TODO: Add an inventory system for player and update player stats based on what inventory is equipped
// TODO: Add different player ship types to pick at beginning of game
// TODO: Add more enemy types and behaviours
// TODO: Add more projectile types and behaviours
// TODO: Add new backgrounds/areas, make maybe a map system, it loads the background specified in maybe a csv *shrug* map it out in excel
// TODO: Add enemy type based on which background tile it is generated on
// TODO: Add space stations where you can purchase inventory items
// TODO: Add gameover conditions
// TODO: Add story stuff
// TODO: Eventualllly add multiplayer support

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

    // Constant references to Prefab filenames
    public const string CamerasPrefabName = "Cameras";
    public const string FollowCameraName = "Follow Camera";
    public const string UIPrefabName = "UI";
    public const string CanvasName = "Canvas";
    public const string ShieldDamageEffectName = "Shield Damage Effect";
    public const string HealthDamageEffectName = "Health Damage Effect";
    public const string GameOverScreenName = "Game Over Screen";
    public const string PlayerUIPrefabName = "Player UI";
    public const string NPCUIPrefabName = "NPC UI";
    public const string InfoLabelName = "Info Label";
    public const string BackgroundPrefabName = "Background";
    public const string PlayerPrefabName = "Player Ship";
    public const string FriendPrefabName = "Player Ship";
    public const string EnemyPrefabName = "Enemy Ship";
    public const string ProjectilePrefabName = "Projectile";
    public const string BombPrefabName = "Bomb";
    public const string ProjectileShieldStrikePrefabName = "ProjectileShieldStrike";
    public const string ProjectileHullStrikePrefabName = "ProjectileHullStrike";
    public const string BombExplostionPrefabName = "Bomb Explosion";
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

    // Game State fields
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused
    }
    public static GameState CurrentGameState;
    public static bool GameInitialized = false;


    // Start is called before the first frame update
    public static void Start()
    {
        CurrentGameState = GameState.Playing;
    }

    // Update is called once per frame
    public static void Update()
    {
        if(CurrentGameState == GameState.Playing)
        {
            if(GameInitialized == false)
            {
                InitializeGameState();
                GameInitialized = true;
            }
            ProcessShipUpdate();
            EnemySpawnUpdate();
            Background.Update();
            UIController.Update();
        }
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public static void FixedUpdate()
    {
        if(CurrentGameState == GameState.Playing)
        {
            ProcessShipPhysicsUpdate();
            ProcessProjectilePhysicsUpdate();
            CleanupShipList();
            CleanupProjectileList();
            EnemyDespawnUpdate();
        }
    }

    // Initialize game state
    public static void InitializeGameState()
    {
        SpawnPlayer();
        InitializeCamera();
        Background.Start();
        UIController.Start();
    }

    private static void InitializeCamera()
    {
        // If player has been instantiated
        if(Player != null)
        {
            // Spawn camera
            CamerasPrefab = Resources.Load(CamerasPrefabName, typeof(GameObject)) as GameObject;
            Cameras = GameObject.Instantiate(CamerasPrefab);
            Cameras.name = "Cameras";
            // Set up follow camera
            FollowCamera = GameObject.Find(FollowCameraName).GetComponent<CinemachineVirtualCamera>();
            FollowCamera.Follow = Player.ShipObject.transform;
        }
    }

    // Initialize player ship in world
    private static void SpawnPlayer()
    {
        Ships.Add(ShipID, new PlayerShip(ShipID));
        Player = Ships[ShipID];
        GetNextShipID();
    }

    // Spawn friendly ship
    private static void SpawnFriendly()
    {
        Ships.Add(ShipID, new FriendlyShip(ShipID));
        GetNextShipID();
    }

    // Spawn enemy ship
    private static void SpawnEnemy(Vector3 _startingPosition)
    {
        Ships.Add(ShipID, new EnemyShip(ShipID, _startingPosition));
        EnemyCount++;
        GetNextShipID();
    }

    // Spawn projectile
    public static void SpawnProjectile(IFF _iff, uint _type, float _damage, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        Projectiles.Add(ProjectileID, new Bolt(ProjectileID, _iff, _type, _damage, _position, _rotation, _velocity, _speed, _lifetime));
        GetNextProjectileID();
    }

    // Spawn bomb
    public static Bomb SpawnBomb(PlayerShip _player, IFF _iff, float _damage, float _radius, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        Projectiles.Add(ProjectileID, new Bomb(_player, ProjectileID, _iff, _damage, _radius, _position, _rotation, _velocity, _speed, _lifetime));
        Bomb bomb = Projectiles[ProjectileID] as Bomb;
        GetNextProjectileID();
        return bomb;
    }

    // Gets next ship ID. Shouln't ever need to be used, is only applicable if play session is long enough that 4 billion ships have been spawned... consider removing
    private static void GetNextShipID()
    {
        // If ship ID is equal to max value
        if(ShipID == uint.MaxValue)
        {
            // Mark that ship ID has hit maximum
            ShipIDPassedMax = true;
        }
        // If ship ID has not yet passed max value
        if(ShipIDPassedMax == false)
        {
            // Increment ship ID
            ShipID++;
        }
        // If ship ID has passed max value
        else if(ShipIDPassedMax == true)
        {
            // Check if current ID is being used, if so...
            if(Ships.ContainsKey(ShipID))
            {
                // Increment ship ID and recursively rerun this method until finding an unused ship ID
                ShipID++;
                GetNextShipID();
            }
        }
    }

    // Gets next projectile ID. Doesn't really do anything but increment projectile ID... consider removing
    private static void GetNextProjectileID()
    {
        // Increment projectile ID
        ProjectileID++;
    }

    // Process ship updates
    private static void ProcessShipUpdate()
    {
        // Loop through all ships
        foreach(KeyValuePair<uint, Ship> ship in Ships)
        {
            // Update ship
            ship.Value.Update();
        }
    }

    // Process ship physics updates
    private static void ProcessShipPhysicsUpdate()
    {
        // Loop through all ships
        foreach(KeyValuePair<uint, Ship> ship in Ships)
        {
            // Physics update ship
            ship.Value.FixedUpdate();
        }
    }

    // Process projectile physics updates
    private static void ProcessProjectilePhysicsUpdate()
    {
        // Loop through all projectiles
        foreach(KeyValuePair<uint, Projectile> projectile in Projectiles)
        {
            // Physics update projectile
            projectile.Value.FixedUpdate();
        }
    }

    // Cleans up ship list
    private static void CleanupShipList()
    {
        // Loop through all ships
        foreach(KeyValuePair<uint, Ship> ship in Ships)
        {
            // If ship is dead and not a player
            if(ship.Value.Alive == false && ship.Value.IsPlayer == false)
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
    }

    // Clean up projectile list
    private static void CleanupProjectileList()
    {
        // Loop through all projectiles
        foreach(KeyValuePair<uint, Projectile> projectile in Projectiles)
        {
            // If projectile is dead
            if(projectile.Value.Alive == false)
            {
                // Add projectile to removal list
                ProjectilesToRemove.Add(projectile.Key);
            }
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
    }

    // Checks if enemies should be spawned
    private static bool ShouldSpawnEnemy()
    {
        // If current enemy count is below max allowed and player is alive return true, otherwise false
        if(EnemyCount < MaxEnemyCount && Player.Alive == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Gets next enemy spawn position
    private static Vector3 GetNextEnemySpawnPosition()
    {
        // Get a random number between min enemy spawn distance and max for X and Z values
        int nextXSpawn = r.Next(MinEnemySpawnDistance, MaxEnemySpawnDistance);
        int nextZSpawn = r.Next(MinEnemySpawnDistance, MaxEnemySpawnDistance);
        // 50% chance that X or Z value is negative instead of positive
        if(r.Next(0, 2) == 1)
        {
            nextXSpawn *= -1;
        }
        if(r.Next(0, 2) == 1)
        {
            nextZSpawn *= -1;
        }
        // Add X and Z values to player position to get a final random position that is around player within spawn distance limits
        return new Vector3(Player.ShipObject.transform.position.x + nextXSpawn, 0, Player.ShipObject.transform.position.z + nextZSpawn);
    }

    // Enemy spawn update
    private static void EnemySpawnUpdate()
    {
        // If enemy should be spawned
        if(ShouldSpawnEnemy() == true)
        {
            // Run GetNextEnemySpawnPosition and spawn enemy at location it returns
            SpawnEnemy(GetNextEnemySpawnPosition());
        }
    }

    // Enemy despawn update
    private static void EnemyDespawnUpdate()
    {
        // If player is currently alive
        //if(Player.Alive == true) // TODO: FIX THIS
        {
            // Loop through all ships
            foreach(KeyValuePair<uint, Ship> ship in Ships)
            {
                // If ship is alive, is an enemy, and is further from player than despawn distance
                if(ship.Value.Alive && ship.Value.IFF == IFF.Enemy && Vector3.Distance(Player.ShipObject.transform.position, ship.Value.ShipObject.transform.position) >= EnemyDespawnDistance)
                {
                    // Despawn enemy ship
                    ship.Value.Despawn();
                }
            }
        }
    }

    // Recieve Collision info and propogate, this is referenced by the CollisionHandler script placed on all Ship and Projectile prefabs
    public static void Collide(GameObject _collisionReporter, GameObject _collidedWith)
    {
        // If the object reporting collision is a projectile
        if(_collisionReporter.tag == "Projectile")
        {
            // Get the projectile
            Projectile projectile = Projectiles[uint.Parse(_collisionReporter.name)];
            // If projectile is friendly and has collided with an enemy
            if(projectile.IFF == IFF.Friend && _collidedWith.tag == "Enemy")
            {
                // Run ReceivedCollision for projectile
                projectile.ReceivedCollision();
                // Run ReceivedCollision for enemy
                Ships[uint.Parse(_collidedWith.name)].ReceivedCollisionFromProjectile(projectile.Damage, projectile.ProjectileObject.transform.position);
            }
            // If projectile is enemy and has collided with a player
            else if(projectile.IFF == IFF.Enemy && _collidedWith.tag == "Player")
            {
                // Run ReceivedCollision for projectile
                projectile.ReceivedCollision();
                // Run ReceivedCollision for player
                Ships[uint.Parse(_collidedWith.name)].ReceivedCollisionFromProjectile(projectile.Damage, projectile.ProjectileObject.transform.position);
            }
            // If projectile is enemy and has collided with a friendly ship
            else if(projectile.IFF == IFF.Enemy && _collidedWith.tag == "Friend")
            {
                // Run ReceivedCollision for projectile
                projectile.ReceivedCollision();
                // Run ReceivedCollision for friendly ship
                Ships[uint.Parse(_collidedWith.name)].ReceivedCollisionFromProjectile(projectile.Damage, projectile.ProjectileObject.transform.position);
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
}
