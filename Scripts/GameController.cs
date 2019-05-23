using System.Collections.Generic;

using UnityEngine;

// GENERAL STUFF TO DO
// TODO: Add a main menu with a new game button, a load game button, a settings button to configure inputs and maybe graphics, and a quit button
// TODO: Add a game state, (MainMenu, Playing, Paused, etc) and change GameController.Update to depend on game state
// TODO: Add object pooling
// TODO: Add an inventory system for player and update player stats based on what inventory is equipped
// TODO: Add different player ship types to pick at beginning of game
// TODO: Add more enemy types and behaviours
// TODO: Set up a field for how many guns a ship has, then during initialization loop through that amount and find all gun barrel objects instead of hardcoding it
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
    // Version
    public static string Version = "0.0.9c";
    // GameObjects and Components
    public static Ship Player;
    private static GameObject CamerasPrefab;
    private static GameObject Cameras;
    public static System.Random r = new System.Random();

    // Entity Lists and Dicts
    public static readonly Dictionary<uint, Ship> Ships = new Dictionary<uint, Ship>();
    public static readonly Dictionary<uint, Projectile> Projectiles = new Dictionary<uint, Projectile>();
    public static readonly List<uint> ShipsToRemove = new List<uint>();
    public static readonly List<uint> ProjectilesToRemove = new List<uint>();

    // Enemy spawn fields
    private static uint EnemyCount = 0;
    private readonly static uint MaxEnemyCount = 20;
    private readonly static int MinEnemySpawnDistance = 40;
    private readonly static int MaxEnemySpawnDistance = 200;
    private readonly static int EnemyDespawnDistance = 300;

    // Constant references to Prefab filenames
    public const string MainMenuName = "Main Menu";
    public const string CamerasPrefabName = "Cameras";
    public const string FollowCameraName = "Follow Camera";
    public const string UIName = "UI";
    public const string CanvasName = "UI Canvas";
    public const string ShieldDamageEffectName = "Shield Damage Effect";
    public const string HealthDamageEffectName = "Health Damage Effect";
    public const string GameOverScreenName = "Game Over Screen";
    public const string GameOverTextName = "Game Over Text";
    public const string GameOverRestartButtonName = "Restart Button";
    public const string PlayerUIName = "Player UI";
    public const string NPCUIPrefabName = "NPC UI";
    public const string MinimapCoordsName = "Minimap Coords";
    public const string InfoLabelName = "Info Label";
    public const string BackgroundPrefabName = "Background";
    public const string PlayerPrefabName = "Player Ship";
    public const string FriendPrefabName = "Player Ship";
    public const string EnemyStandardPrefabName = "Enemy Ship 0";
    public const string EnemyRammingPrefabName = "Enemy Ship 1";
    public const string EnemyBroadsidePrefabName = "Enemy Ship 2";
    public const string ImpulseEngineName = "Impulse Engine";
    public const string WarpEngineName = "Warp Engine";
    public const string GunBarrelName = "Gun Barrel";
    public const string GunBarrelLightsName = "Gun Barrel Lights";
    public const string ShieldName = "Barrier";
    public const string ScannerName = "Scanner";
    public const string ProjectilePrefabName = "Projectile";
    public const string BombPrefabName = "Bomb";
    public const string ProjectileShieldStrikePrefabName = "Projectile Shield Strike";
    public const string ProjectileHullStrikePrefabName = "Projectile Hull Strike";
    public const string BombExplostionPrefabName = "Bomb Explosion";
    public const string ExplosionPrefabName = "Explosion";

    // Entity IDs
    private static uint ShipID = 0;
    private static uint ProjectileID = 0;

    // Score
    public static uint Score;
    public static float TimeStarted;

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
    public static bool GameplayInitialized = false;


    // Initialize is called before the first frame update
    public static void Initialize()
    {
        Logger.Initialize();
        CurrentGameState = GameState.MainMenu;
        UIController.Initialize();
        InitializeCamera();
    }

    // Update is called once per frame
    public static void Update()
    {
        if(CurrentGameState == GameState.MainMenu)
        {
            UIController.ShowMainMenu();
        }
        if(CurrentGameState == GameState.Playing)
        {
            if(GameplayInitialized == false)
            {
                InitializeGameplay();
                TimeStarted = Time.time;
                GameplayInitialized = true;
            }
            ProcessShipUpdate();
            EnemySpawnUpdate();
            FollowCamera();
            Background.Update();
            UIController.Update();
        }
        else if(CurrentGameState == GameState.Paused)
        {
            //UIController.ShowPauseMenu();
        }
        Logger.Update();
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

    // On application quit
    public static void OnApplicationQuit()
    {
        Logger.OnApplicationQuit();
    }

    // On restart
    public static void Restart()
    {
        foreach(KeyValuePair<uint, Ship> ship in Ships)
        {
            GameObject.Destroy(ship.Value.ShipObject);
        }
        Player = null;
        Ships.Clear();
        ShipsToRemove.Clear();
        foreach(KeyValuePair<uint, Projectile> projectile in Projectiles)
        {
            GameObject.Destroy(projectile.Value.ProjectileObject);
        }
        Projectiles.Clear();
        ProjectilesToRemove.Clear();
        Background.Restart();
        UIController.Restart();
        GameplayInitialized = false;
    }

    // Initialize camera
    private static void InitializeCamera()
    {
        // Spawn camera
        CamerasPrefab = Resources.Load<GameObject>(CamerasPrefabName);
        Cameras = GameObject.Instantiate(CamerasPrefab);
        Cameras.name = "Cameras";
    }

    // Set up follow camera
    private static void FollowCamera()
    {
        // If player has been instantiated
        if(Player != null)
        {
            // Set up follow camera
            Cameras.transform.position = Player.ShipObject.transform.position + new Vector3(0, 120, 0);
        }
    }

    // Initialize gameplay
    private static void InitializeGameplay()
    {
        // Clear lists
        Ships.Clear();
        ShipsToRemove.Clear();
        Projectiles.Clear();
        ProjectilesToRemove.Clear();
        EnemyCount = 0;
        ShipID = 0;
        ProjectileID = 0;
        Score = 0;
        // Initialize player, follow camera, and backgrounds
        SpawnPlayer();
        FollowCamera();
        Background.Initialize();
    }

    // Initialize player ship in world
    private static void SpawnPlayer()
    {
        NextShipID();
        Ships.Add(ShipID, new PlayerShip(ShipID));
        Player = Ships[ShipID];
    }

    // Spawn friendly ship
    private static void SpawnFriendly()
    {
        NextShipID();
        Ships.Add(ShipID, new FriendlyShip(ShipID));
    }

    // Spawn enemy ship
    private static void SpawnEnemy(Vector3 _startingPosition)
    {
        NextShipID();
        int enemyTypeSelector = r.Next(0, 100);
        if(enemyTypeSelector <= 80)
        {
            Ships.Add(ShipID, new EnemyShipStandard(ShipID, _startingPosition));
        }
        else if(enemyTypeSelector > 80 && enemyTypeSelector <= 90)
        {
            Ships.Add(ShipID, new EnemyShipRamming(ShipID, _startingPosition));
        }
        else if(enemyTypeSelector > 90)
        {
            Ships.Add(ShipID, new EnemyShipBroadside(ShipID, _startingPosition));
        }
        EnemyCount++;
    }

    // Spawn projectile
    public static void SpawnProjectile(IFF _iff, uint _type, float _curvature, float _damage, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        NextProjectileID();
        Projectiles.Add(ProjectileID, new Bolt(ProjectileID, _iff, _type, _curvature, _damage, _position, _rotation, _velocity, _speed, _lifetime));
    }

    // Spawn bomb
    public static Bomb SpawnBomb(PlayerShip _player, IFF _iff, float _curvature, float _damage, float _radius, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        NextProjectileID();
        Projectiles.Add(ProjectileID, new Bomb(_player, ProjectileID, _iff, _curvature, _damage, _radius, _position, _rotation, _velocity, _speed, _lifetime));
        return Projectiles[ProjectileID] as Bomb;
    }

    // Gets next ship ID.
    private static void NextShipID()
    {
        // Increment ship ID
        ShipID++;
    }

    // Gets next projectile ID.
    private static void NextProjectileID()
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
        // If ships to remove has values
        if(ShipsToRemove.Count > 0)
        {
            // Loop through ship removal list
            foreach(uint ID in ShipsToRemove)
            {
                // If ship is enemy
                if(Ships[ID].IFF == IFF.Enemy)
                {
                    // Decrement enemy count
                    EnemyCount--;
                }
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
        int nextXSpawn = r.Next(MinEnemySpawnDistance, MaxEnemySpawnDistance + 1);
        int nextZSpawn = r.Next(MinEnemySpawnDistance, MaxEnemySpawnDistance + 1);
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
            // If both ships are alive
            if(reporter.Alive == true && collidedWith.Alive == true)
            {
                // Send collision report
                reporter.ReceivedCollisionFromShip(collidedWith.ShipRigidbody.velocity, collidedWith.IFF);
                collidedWith.ReceivedCollisionFromShip(reporter.ShipRigidbody.velocity, reporter.IFF);
            }
        }
    }
}
