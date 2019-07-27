using System;
using System.Collections.Generic;

using UnityEngine;

// GENERAL STUFF TO DO
// TODO: Add leveling system / stats
// TODO: Add an inventory system for player and update player stats based on what inventory is equipped
// TODO: Add a save system for progress
// TODO: Add object pooling MAYBE, don't need to right now as performance is fine, but could be a good idea for the future
// TODO: Add a value for enemies so large enemies are worth more than small enemies for Enemy Count purposes maybe...
// TODO: Add different player ship types to pick at beginning of game
// TODO: Add more enemy types and behaviours
// TODO: Add boss enemies
// TODO: Add more projectile types and behaviours
// TODO: Add space stations where you can purchase inventory items
// TODO: Either add story stuff like missions and NPCs to talk to, or make rogue-lite style where each run is randomized and bosses appear after a set time
// TODO: Eventualllly add multiplayer support

// Keeps track of all entites and updates all systems in the game
public static class GameController
{
    // Version
    public const string Version = "0.0.16c";

    // GM reference
    private static GameManager gm = GameManager.Instance;

    // Random number generator
    public static System.Random RandomNumGen { get; private set; } = new System.Random();

    // Entity Lists and Dicts
    public static Dictionary<uint, Ship> Ships { get; private set; } = new Dictionary<uint, Ship>();
    private static Dictionary<uint, Ship> ShipsToAdd = new Dictionary<uint, Ship>();
    private static Dictionary<uint, Projectile> Projectiles = new Dictionary<uint, Projectile>();
    private static List<uint> ShipsToRemove = new List<uint>();
    private static List<uint> ProjectilesToRemove = new List<uint>();

    // Cameras
    private static GameObject Cameras;

    // Player fields
    public static Ship Player { get; private set; }
    private static PlayerShip.PlayerShipType PlayerShipType;

    // Enemy spawn fields
    private static uint EnemyCount = 0;
    private const uint MaxEnemyCount = 20;
    private const int MinEnemySpawnDistance = 40;
    private const int MaxEnemySpawnDistance = 200;
    private const int EnemyDespawnDistance = 300;

    // Entity IDs
    private static uint ShipID = 0;
    private static uint ProjectileID = 0;

    // Score
    public static uint Score { get; private set; } = 0;
    public static float TimeStarted { get; private set; } = 0f;

    // Identify Friend or Foe
    public enum IFF
    {
        Friend,
        Enemy
    }

    // Game State fields
    public enum GameState
    {
        Menus,
        Playing,
        Paused,
    }
    public static GameState CurrentGameState { get; private set; } = GameState.Menus;
    private static bool GameplayInitialized = false;

    // Constant references to Prefab filenames
    // Cursor
    public const string AimCursorTextureName = "Cursor/AimCursor";
    // Background tiles
    public const string TilemapName = "Tilemap/Tilemap";
    public const string BackgroundPrefabName = "Environment/Background";
    // Ships
    public const string PlayerPrefabName = "Ships/Player Ships/Player Ship";
    public const string DronePrefabName = "Ships/Drone Ships/Drone Ship";
    public const string EnemyShipPrefabName = "Ships/Enemy Ships/Enemy Ship";
    // Ship parts
    public const string ImpulseEngineObjectName = "Impulse Engine";
    public const string WarpEngineObjectName = "Warp Engine";
    public const string GunBarrelObjectName = "Gun Barrel";
    public const string GunBarrelLightsObjectName = "Gun Barrel Lights";
    public const string ShieldObjectName = "Shield";
    public const string BarrierObjectName = "Barrier";
    public const string ShieldOverchargeObjectName = "Overcharge";
    public const string EMPObjectName = "EMP";
    // Projectiles
    public const string ProjectilePrefabName = "Projectiles/Projectile";
    public const string BombPrefabName = "Projectiles/Bomb";
    // Visual FX
    public const string ProjectileShieldStrikePrefabName = "VisualFX/Projectile Shield Strike";
    public const string ProjectileHullStrikePrefabName = "VisualFX/Projectile Hull Strike";
    public const string BombExplostionPrefabName = "VisualFX/Bomb Explosion";
    public const string ExplosionPrefabName = "VisualFX/Explosion";
    public const string ElectricityEffectPrefabName = "VisualFX/Electricity Effect";


    // Initialize is called before the first frame update
    public static void Initialize()
    {
        Logger.Initialize();
        UIControllerNew.Initialize();
        InitializeCamera();
    }

    // Update is called once per frame
    public static void Update()
    {
        PlayerInput.Update();
        // If gamestate is playing
        if(CurrentGameState == GameState.Playing)
        {
            // If gameplay has yet to be initialized
            if(GameplayInitialized == false)
            {
                InitializeGameplay();
                GameplayInitialized = true;
            }
            FollowCamera();
            ProcessShipUpdate();
            EnemyDespawnUpdate();
            EnemySpawnUpdate();
            CleanupShipList();
            CleanupProjectileList();
            Background.Update();
        }
        UIControllerNew.Update();
        Logger.Update();
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public static void FixedUpdate()
    {
        // If gamestate is playing
        if(CurrentGameState == GameState.Playing)
        {
            ProcessShipPhysicsUpdate();
            ProcessProjectilePhysicsUpdate();
        }
    }

    // Change game state
    public static void ChangeGameState(GameState _newGameState)
    {
        UIControllerNew.ChangeGameState(_newGameState);
        CurrentGameState = _newGameState;
    }

    // Change player ship type
    public static void ChangePlayerShipType(PlayerShip.PlayerShipType _type)
    {
        PlayerShipType = _type;
    }

    // On application quit
    public static void OnApplicationQuit()
    {
        Logger.OnApplicationQuit();
    }

    // On restart
    public static void Restart()
    {
        // Loop through ships
        foreach(KeyValuePair<uint, Ship> ship in Ships)
        {
            // Destroy ships
            GameObject.Destroy(ship.Value.ShipObject);
        }
        // Reset player reference
        Player = null;
        // Clear ship lists
        Ships.Clear();
        ShipsToRemove.Clear();
        // Loop through projectiles
        foreach(KeyValuePair<uint, Projectile> projectile in Projectiles)
        {
            // Destroy projectiles
            GameObject.Destroy(projectile.Value.ProjectileObject);
        }
        // Clear projectile lists
        Projectiles.Clear();
        ProjectilesToRemove.Clear();
        // Call restart method in background and UI
        Background.Restart();
        UIControllerNew.StartNewGame();
        // Set gameplay initialized to default value of false
        GameplayInitialized = false;
        CurrentGameState = GameState.Playing;
    }

    // Initialize camera
    private static void InitializeCamera()
    {
        // Get camera
        Cameras = gm.Cameras;
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

    // Reset camera to default position
    private static void ResetCamera()
    {
        Cameras.transform.position = new Vector3(0, 0, 0);
    }

    // Initialize gameplay
    private static void InitializeGameplay()
    {
        // Clear lists
        Ships.Clear();
        ShipsToRemove.Clear();
        Projectiles.Clear();
        ProjectilesToRemove.Clear();
        // Reset counts
        EnemyCount = 0;
        ShipID = 0;
        ProjectileID = 0;
        Score = 0;
        // Get time started
        TimeStarted = Time.time;
        // Initialize player, follow camera, and backgrounds
        SpawnPlayer(PlayerShipType);
        FollowCamera();
        Background.Initialize();
    }

    // Initialize player ship in world
    private static void SpawnPlayer(PlayerShip.PlayerShipType _type)
    {
        NextShipID();
        // Spawn appropriate ship to player ship type
        if(_type == PlayerShip.PlayerShipType.Bomber)
        {
            Ships.Add(ShipID, new PSBomber(ShipID));
        }
        else if(_type == PlayerShip.PlayerShipType.Engineer)
        {
            Ships.Add(ShipID, new PSEngineer(ShipID));
        }
        else if(_type == PlayerShip.PlayerShipType.Scout)
        {
            Ships.Add(ShipID, new PSScout(ShipID));
        }
        // Get reference to player ship
        Player = Ships[ShipID];
    }

    // Spawn friendly ship
    private static void SpawnFriendly()
    {
        NextShipID();
        //Ships.Add(ShipID, new FriendlyShip(ShipID));
    }

    // Spawn enemy ship
    private static void SpawnEnemy(Vector3 _startingPosition)
    {
        NextShipID();
        Ships.Add(ShipID, Zone.SpawnEnemy(ShipID, _startingPosition));
        EnemyCount++;
    }

    // Spawn projectile
    public static void SpawnProjectile(IFF _iff, uint _type, float _curvature, float _sightCone, float _damage, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        NextProjectileID();
        Projectiles.Add(ProjectileID, new Bolt(ProjectileID, _iff, _type, _curvature, _sightCone, _damage, _position, _rotation, _velocity, _speed, _lifetime));
    }

    // Spawn bomb
    public static Bomb SpawnBomb(Ship _player, IFF _iff, float _damage, float _radius, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        NextProjectileID();
        Bomb bomb = new Bomb(_player, ProjectileID, _iff, _damage, _radius, _position, _rotation, _velocity, _speed, _lifetime);
        Projectiles.Add(ProjectileID, bomb);
        return bomb;
    }

    // Spawn drone
    public static DroneShip SpawnDrone(Ship _parent, List<DroneShip> _parentDroneList, DroneShip.DroneShipType _type, Vector3 _startingPosition, float _maxHealth, float _maxShields, float _maxSpeed, uint _gunShotProjectileType, float _gunCooldownTime, uint _gunShotAmount, float _gunShotDamage, float _gunShotAccuracy, float _gunShotSpeed, float _gunShotLifetime, float _maxTargetAcquisitionDistance, float _maxStrafeDistance, float _maxLeashDistance)
    {
        NextShipID();
        DroneShip droneShip = new DroneShip(ShipID, _parent, _parentDroneList, _type, _startingPosition, _maxHealth, _maxShields, _maxSpeed, _gunShotProjectileType, _gunCooldownTime, _gunShotAmount, _gunShotDamage, _gunShotAccuracy, _gunShotSpeed, _gunShotLifetime, _maxTargetAcquisitionDistance, _maxStrafeDistance, _maxLeashDistance);
        // This gets called during a ship update, so it can't modify the ships list directly. As such, it adds to a list of ships to add and gets added after ships list finishes enumerating
        ShipsToAdd.Add(ShipID, droneShip);
        return droneShip;
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

    // Add to score
    public static void AddToScore(uint _exp)
    {
        Score += _exp;
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
        // Loop through all ships to add to scene
        foreach(KeyValuePair<uint, Ship> ship in ShipsToAdd)
        {
            // Add to ships list
            Ships.Add(ship.Key, ship.Value);
        }
        // Clear ships to add list
        ShipsToAdd.Clear();
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

    // Add ship to removal list
    public static void AddShipToRemovalList(uint _id)
    {
        ShipsToRemove.Add(_id);
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

    // Add projectile to removal list
    public static void AddProjectileToRemovalList(uint _id)
    {
        ProjectilesToRemove.Add(_id);
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
        int nextXSpawn = RandomNumGen.Next(MinEnemySpawnDistance, MaxEnemySpawnDistance + 1);
        int nextZSpawn = RandomNumGen.Next(MinEnemySpawnDistance, MaxEnemySpawnDistance + 1);
        // 50% chance that X or Z value is negative instead of positive
        if(RandomNumGen.Next(0, 2) == 1)
        {
            nextXSpawn *= -1;
        }
        if(RandomNumGen.Next(0, 2) == 1)
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
        if(_collisionReporter.tag == "Projectile" && _collidedWith.tag != "Projectile")
        {
            try
            {
                // Get the projectile
                Projectile projectile = Projectiles[uint.Parse(_collisionReporter.name)];
                Ship collidedWith = Ships[uint.Parse(_collidedWith.name)];
                // If projectile is friendly and has collided with an enemy
                if(projectile.IFF == IFF.Friend && collidedWith.IFF != IFF.Friend)
                {
                    // Run ReceivedCollision for projectile
                    projectile.ReceivedCollision();
                    // Run ReceivedCollision for enemy
                    Ships[uint.Parse(_collidedWith.name)].ReceivedCollisionFromProjectile(projectile.Damage, projectile.ProjectileObject.transform.position);
                }
                // If projectile is enemy and has collided with a player
                else if(projectile.IFF == IFF.Enemy && collidedWith.IFF != IFF.Enemy)
                {
                    // Run ReceivedCollision for projectile
                    projectile.ReceivedCollision();
                    // Run ReceivedCollision for player
                    Ships[uint.Parse(_collidedWith.name)].ReceivedCollisionFromProjectile(projectile.Damage, projectile.ProjectileObject.transform.position);
                }
            }
            catch(Exception e)
            {
                Debug.Log($@"Projectile Collision Error");
                Debug.Log(e.ToString());
                Logger.Log($@"Projectile Collision Error");
                Logger.Log(e);
            }
        }
        // If two ships collided
        else if(_collisionReporter.tag != "Projectile" && _collidedWith.tag != "Projectile")
        {
            try
            {
                // Get ships
                Ship reporter = Ships[uint.Parse(_collisionReporter.name)];
                Ship collidedWith = Ships[uint.Parse(_collidedWith.name)];
                // If both ships are alive
                if(reporter.Alive == true && collidedWith.Alive == true && reporter.Parent != collidedWith && reporter != collidedWith.Parent)
                {
                    // Send collision report
                    reporter.ReceivedCollisionFromShip(collidedWith.ShipObject.transform.position, collidedWith.IFF);
                    collidedWith.ReceivedCollisionFromShip(reporter.ShipObject.transform.position, reporter.IFF);
                }
            }
            catch(Exception e)
            {
                Debug.Log($@"Ship Collision Error");
                Debug.Log(e.ToString());
                Logger.Log($@"Ship Collision Error");
                Logger.Log(e);
            }
        }
    }
}
