using Cinemachine;

using System.Collections.Generic;

using UnityEngine;

// Keeps track of all entites and updates all systems in the game
public static class GameController
{
    // GameObjects and Components
    public static IEntity Player;
    private static CinemachineVirtualCamera followCamera;

    // Entity List and Dicts
    private static readonly Dictionary<uint, IEntity> entities = new Dictionary<uint, IEntity>();
    private static readonly Dictionary<uint, Projectile> projectiles = new Dictionary<uint, Projectile>();
    private static readonly List<uint> entitiesToRemove = new List<uint>();
    private static readonly List<uint> projectilesToRemove = new List<uint>();

    // Constants
    public const string FollowCameraName = "Follow Camera";
    public const string BackgroundPrefabName = "Background";
    public const string PlayerPrefabName = "Player Ship";
    public const string FriendPrefabName = "Player Ship";
    public const string EnemyPrefabName = "Enemy Ship";
    public const string ProjectilePrefabName = "Projectile";

    // Entity IDs
    private static uint entityID = 0;
    private static uint projectileID = 0;
    private static bool entityIDPassedMax = false;

    public enum IFF
    {
        friend,
        enemy
    };

    // Start is called before the first frame update
    public static void Start()
    {
        // Spawn player
        SpawnPlayer();
        // Set up follow camera
        followCamera = GameObject.Find(FollowCameraName).GetComponent<CinemachineVirtualCamera>();
        followCamera.Follow = Player.ShipObject.transform;
        // Initialize background
        Background.Start();
        // Spawn enemy
        SpawnEnemy();
    }

    // Update is called once per frame
    public static void Update()
    {
        // Update all entities
        foreach(KeyValuePair<uint, IEntity> entity in entities)
        {
            if(entity.Value.Alive)
            {
                entity.Value.Update();
            }
            else
            {
                entitiesToRemove.Add(entity.Key);
            }
        }
        // Update Background
        Background.Update();
    }

    // Fixed Update is called a fixed number of times per second
    public static void FixedUpdate()
    {
        // FixedUpdate all entities
        foreach(KeyValuePair<uint, IEntity> entity in entities)
        {
            if(entity.Value.Alive)
            {
                entity.Value.FixedUpdate();
            }
            else
            {
                entitiesToRemove.Add(entity.Key);
            }
        }
        foreach(KeyValuePair<uint, Projectile> projectile in projectiles)
        {
            if(projectile.Value.Alive)
            {
                projectile.Value.FixedUpdate();
            }
            else
            {
                entitiesToRemove.Add(projectile.Key);
            }
        }
        // Remove dead entities
        if(entitiesToRemove.Count > 0)
        {
            foreach(uint ID in entitiesToRemove)
            {
                entities.Remove(ID);
            }
            entitiesToRemove.Clear();
        }
        if(projectilesToRemove.Count > 0)
        {
            foreach(uint ID in projectilesToRemove)
            {
                projectiles.Remove(ID);
            }
            projectilesToRemove.Clear();
        }
    }

    // Initialize player ship in world
    public static void SpawnPlayer()
    {
        entities.Add(entityID, new PlayerShip(entityID));
        Player = entities[entityID];
        entityID++;
        GetNextEntityID();
    }

    // Spawn friendly ships
    public static void SpawnFriendly()
    {
        entities.Add(entityID, new FriendlyShip(entityID));
        entityID++;
        GetNextEntityID();
    }

    // Spawn enemies
    public static void SpawnEnemy()
    {
        entities.Add(entityID, new EnemyShip(entityID));
        entityID++;
        GetNextEntityID();
    }

    // Spawn projectiles
    public static void SpawnProjectile(IFF _iff, float _damage, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        projectiles.Add(projectileID, new Projectile(projectileID, _iff, _damage, _position, _rotation, _velocity, _speed, _lifetime));
        projectileID++;
    }

    // Once entityID has passed max value and overflowed, since we are starting from 
    public static void GetNextEntityID()
    {
        if(entityID == uint.MaxValue)
        {
            entityIDPassedMax = true;
        }
        else if(entityIDPassedMax == true)
        {
            if(entities.ContainsKey(entityID))
            {
                entityID++;
                GetNextEntityID();
            }
        }
    }

    // Recieve Collision info and propogate
    public static void Collide(GameObject _collisionReporter, GameObject _collidedWith)
    {
        if(_collisionReporter.tag == "Projectile")
        {
            Projectile projectile = projectiles[uint.Parse(_collisionReporter.name)];
            if(projectile.IFF == IFF.friend && _collidedWith.tag == "Enemy")
            {
                try
                {
                    IEntity enemy = entities[uint.Parse(_collidedWith.name)];
                    projectile.ReceivedCollision();
                    enemy.ReceivedCollision(projectile.Damage);
                }
                catch(System.Exception e)
                {
                    Debug.Log($@"Projectile collided with entity that doesn't exist anymore...");
                }
            }
            else if(projectile.IFF == IFF.enemy && _collidedWith.tag == "Player")
            {
                try
                {
                    IEntity player = entities[uint.Parse(_collidedWith.name)];
                    projectile.ReceivedCollision();
                    player.ReceivedCollision(projectile.Damage);
                }
                catch(System.Exception e)
                {
                    Debug.Log($@"Projectile collided with entity that doesn't exist anymore...");
                }
            }
            else if(projectile.IFF == IFF.enemy && _collidedWith.tag == "Friend")
            {
                try
                {
                    IEntity friend = entities[uint.Parse(_collidedWith.name)];
                    projectile.ReceivedCollision();
                    friend.ReceivedCollision(projectile.Damage);
                }
                catch(System.Exception e)
                {
                    Debug.Log($@"Projectile collided with entity that doesn't exist anymore...");
                }
            }
        }
    }
}
