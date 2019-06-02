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
// TODO: Add more projectile types and behaviours
// TODO: Add enemy type based on which background tile it is generated on
// TODO: Add space stations where you can purchase inventory items
// TODO: Add gameover conditions
// TODO: Add story stuff
// TODO: Eventualllly add multiplayer support

// Keeps track of all entites and updates all systems in the game
public static class GameController
{
    // Version
    public static string Version = "0.0.11e";
    // GameObjects and Components
    public static Ship Player;
    private static GameObject Cameras;
    public static System.Random r = new System.Random();

    // Entity Lists and Dicts
    public static Dictionary<uint, Ship> Ships = new Dictionary<uint, Ship>();
    public static Dictionary<uint, Projectile> Projectiles = new Dictionary<uint, Projectile>();
    public static List<uint> ShipsToRemove = new List<uint>();
    public static List<uint> ProjectilesToRemove = new List<uint>();

    // Player fields
    private static PlayerShip.PlayerShipType PlayerShipType;

    // Enemy spawn fields
    private static uint EnemyCount = 0;
    private static uint MaxEnemyCount = 20;
    private static int MinEnemySpawnDistance = 40;
    private static int MaxEnemySpawnDistance = 200;
    private static int EnemyDespawnDistance = 300;

    // Constant references to Prefab filenames
    // Cameras
    public const string CamerasPrefabName = "Cameras";
    public const string FollowCameraName = "Follow Camera";
    // UI
    public const string MainMenuName = "Main Menu";
    public const string UIName = "UI";
    public const string CanvasName = "UI Canvas";
    public const string ShieldDamageEffectName = "Shield Damage Effect";
    public const string HealthDamageEffectName = "Health Damage Effect";
    public const string PauseMenuScreenName = "Pause Menu Screen";
    public const string GameOverScreenName = "Game Over Screen";
    public const string GameOverTextName = "Game Over Text";
    public const string GameOverRestartButtonName = "Restart Button";
    public const string PlayerUIName = "Player UI";
    public const string NPCUIPrefabName = "NPC UI";
    public const string MinimapCoordsName = "Minimap Coords";
    public const string InfoLabelName = "Info Label";
    // Background tiles
    public const string TilemapName = "Tilemap/Tilemap";
    public const string BackgroundPrefabName = "Environment/Background";
    // Ships
    public const string PlayerPrefabName = "Ships/Player Ships/Player Ship";
    public const string DronePrefabName = "Ships/Drone Ships/Drone Ship";
    public const string FriendPrefabName = "Ships/Player Ships/Player Ship";
    public const string EnemyShipPrefabName = "Ships/Enemy Ships/Enemy Ship";
    // Ship parts
    public const string ImpulseEngineName = "Impulse Engine";
    public const string WarpEngineName = "Warp Engine";
    public const string GunBarrelName = "Gun Barrel";
    public const string GunBarrelLightsName = "Gun Barrel Lights";
    public const string ShieldName = "Barrier";
    // Projectiles
    public const string ProjectilePrefabName = "Projectiles/Projectile";
    public const string BombPrefabName = "Projectiles/Bomb";
    // Visual FX
    public const string ProjectileShieldStrikePrefabName = "VisualFX/Projectile Shield Strike";
    public const string ProjectileHullStrikePrefabName = "VisualFX/Projectile Hull Strike";
    public const string BombExplostionPrefabName = "VisualFX/Bomb Explosion";
    public const string ExplosionPrefabName = "VisualFX/Explosion";

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
    }

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
        // TODO: add an option in settings menu to select a different controller type
        PlayerInput.Controller = PlayerInput.ControllerType.XboxController;
        // TODO: in new game menu have option to select different player ship type
        PlayerShipType = PlayerShip.PlayerShipType.Bomber;
        UIController.Initialize();
        InitializeCamera();
    }

    // Update is called once per frame
    public static void Update()
    {
        // Get player inputs
        PlayerInput.Update();
        // If gamestate is playing
        if(CurrentGameState == GameState.Playing)
        {
            // If gameplay has yet to be initialized
            if(GameplayInitialized == false)
            {
                // Initialize gameplay, get start time, and finally set gameplay to initialized
                InitializeGameplay();
                TimeStarted = Time.time;
                GameplayInitialized = true;
            }
            // Set camera to follow player
            FollowCamera();
            // Process ship updates, despawn distant enemies, spawn new enemies
            ProcessShipUpdate();
            EnemyDespawnUpdate();
            EnemySpawnUpdate();
            // Clean up lists
            CleanupShipList();
            CleanupProjectileList();
            // Update background tiles
            Background.Update();
        }
        // Update UI
        UIController.Update();
        Logger.Update();
    }

    // Fixed Update is called a fixed number of times per second, Physics updates should be done in FixedUpdate
    public static void FixedUpdate()
    {
        // If gamestate is playing
        if(CurrentGameState == GameState.Playing)
        {
            // Process physics updates for ships and projectiles
            ProcessShipPhysicsUpdate();
            ProcessProjectilePhysicsUpdate();
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
        UIController.Restart();
        // Set gameplay initialized to default value of false
        GameplayInitialized = false;
    }

    // Initialize camera
    private static void InitializeCamera()
    {
        // Get camera
        Cameras = GameObject.Find(CamerasPrefabName);
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
        // Reset counts
        EnemyCount = 0;
        ShipID = 0;
        ProjectileID = 0;
        Score = 0;
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
        Ships.Add(ShipID, new FriendlyShip(ShipID));
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
    public static Bomb SpawnBomb(PSBomber _player, IFF _iff, float _damage, float _radius, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        NextProjectileID();
        Bomb bomb = new Bomb(_player, ProjectileID, _iff, _damage, _radius, _position, _rotation, _velocity, _speed, _lifetime);
        Projectiles.Add(ProjectileID, bomb);
        return bomb;
    }

    // Spawn drone
    public static Tuple<uint, DroneShip> SpawnDrone(PSEngineer _parent, DroneShip.DroneShipType _type, Vector3 _startingPosition, float _maxHealth, float _maxShields, float _maxSpeed, uint _gunShotProjectileType, float _gunCooldownTime, uint _gunShotAmount, float _gunShotDamage, float _gunShotAccuracy, float _gunShotSpeed, float _gunShotLifetime, float _maxTargetAcquisitionDistance, float _maxStrafeDistance, float _maxLeashDistance)
    {
        NextShipID();
        DroneShip droneShip = new DroneShip(ShipID, _parent, _type, _startingPosition, _maxHealth, _maxShields, _maxSpeed, _gunShotProjectileType, _gunCooldownTime, _gunShotAmount, _gunShotDamage, _gunShotAccuracy, _gunShotSpeed, _gunShotLifetime, _maxTargetAcquisitionDistance, _maxStrafeDistance, _maxLeashDistance);
        Ships.Add(ShipID, droneShip);
        return new Tuple<uint, DroneShip>(ShipID, droneShip);
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
