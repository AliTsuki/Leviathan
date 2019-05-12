using Cinemachine;

using System.Collections.Generic;

using UnityEngine;

// Keeps track of all entites and updates all systems in the game
public static class GameController
{
    public static PlayerShip Player;
    private static CinemachineVirtualCamera followCamera;

    private static readonly Dictionary<int, PlayerShip> playerShips = new Dictionary<int, PlayerShip>();
    private static readonly Dictionary<int, EnemyShip> enemyShips = new Dictionary<int, EnemyShip>();
    private static readonly Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static List<int> projectilesToRemove = new List<int>();
    private static List<int> enemiesToRemove = new List<int>();

    public const string FollowCameraName = "Follow Camera";
    public const string BackgroundPrefabName = "Background";
    public const string PlayerPrefabName = "Player Ship";
    public const string EnemyPrefabName = "Player Ship";
    public const string ProjectilePrefabName = "Projectile";

    private static int playerID = 0;
    private static int enemyID = 0;
    private static int projectileID = 0;

    // Start is called before the first frame update
    public static void Start()
    {
        // Spawn player
        SpawnPlayer();
        Player = playerShips[0];
        // Set up follow camera
        followCamera = GameObject.Find(FollowCameraName).GetComponent<CinemachineVirtualCamera>();
        followCamera.Follow = playerShips[0].Ship.transform;
        // Initialize background
        Background.Start();
    }

    // Update is called once per frame
    public static void Update()
    {
        // Update all entities
        foreach(KeyValuePair<int, PlayerShip> player in playerShips)
        {
            if(player.Value.Alive)
            {
                player.Value.Update();
            }
        }
        foreach(KeyValuePair<int, EnemyShip> enemy in enemyShips)
        {
            if(enemy.Value.Alive)
            {
                enemy.Value.Update();
            }
            else
            {
                enemiesToRemove.Add(enemy.Key);
            }
        }
        foreach(KeyValuePair<int, Projectile> projectile in projectiles)
        {
            if(projectile.Value.Alive)
            {
                projectile.Value.Update();
            }
            else
            {
                projectilesToRemove.Add(projectile.Key);
            }
            
        }
        // Remove dead enemies/projectiles from dicts
        foreach(int enemyID in enemiesToRemove)
        {
            enemyShips.Remove(enemyID);
        }
        enemiesToRemove.Clear();
        foreach(int projectileID in projectilesToRemove)
        {
            projectiles.Remove(projectileID);
        }
        projectilesToRemove.Clear();
        // Update Background
        Background.Update();
    }

    // Fixed Update is called a fixed number of times per second
    public static void FixedUpdate()
    {
        // FixedUpdate all entities
        foreach(KeyValuePair<int, PlayerShip> player in playerShips)
        {
            if(player.Value.Alive)
            {
                player.Value.FixedUpdate();
            }
        }
        foreach(KeyValuePair<int, EnemyShip> enemy in enemyShips)
        {
            if(enemy.Value.Alive)
            {
                enemy.Value.FixedUpdate();
            }
            else
            {
                enemiesToRemove.Add(enemy.Key);
            }
        }
        foreach(KeyValuePair<int, Projectile> projectile in projectiles)
        {
            if(projectile.Value.Alive)
            {
                projectile.Value.FixedUpdate();
            }
            else
            {
                projectilesToRemove.Add(projectile.Key);
            }
        }
        // Remove dead enemies/projectiles from dicts
        foreach(int enemyID in enemiesToRemove)
        {
            enemyShips.Remove(enemyID);
        }
        enemiesToRemove.Clear();
        foreach(int projectileID in projectilesToRemove)
        {
            projectiles.Remove(projectileID);
        }
        projectilesToRemove.Clear();
    }

    // Initialize player ship in world
    public static void SpawnPlayer()
    {
        playerShips.Add(playerID, new PlayerShip(playerID));
        playerID++;
    }

    // Spawn enemies
    public static void SpawnEnemy()
    {
        enemyShips.Add(enemyID, new EnemyShip(enemyID));
        enemyID++;
    }

    // Spawn projectiles
    public static void SpawnProjectile(Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        projectiles.Add(projectileID, new Projectile(projectileID, _position, _rotation, _velocity, _speed, _lifetime));
        projectileID++;
    }
}
