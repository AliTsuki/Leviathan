using Cinemachine;

using System.Collections.Generic;

using UnityEngine;

// Keeps track of all entites and updates all systems in the game
public static class GameController
{
    // GameObjects and Components
    public static PlayerShip Player;
    private static CinemachineVirtualCamera followCamera;

    // Entity Lists and Dicts
    private static readonly Dictionary<int, PlayerShip> players = new Dictionary<int, PlayerShip>();
    private static readonly Dictionary<int, Ship> friendlies = new Dictionary<int, Ship>();
    private static readonly Dictionary<int, EnemyShip> enemies = new Dictionary<int, EnemyShip>();
    private static readonly Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static List<int> projectilesToRemove = new List<int>();
    private static List<int> enemiesToRemove = new List<int>();

    // Constants
    public const string FollowCameraName = "Follow Camera";
    public const string BackgroundPrefabName = "Background";
    public const string PlayerPrefabName = "Player Ship";
    public const string EnemyPrefabName = "Enemy Ship";
    public const string ProjectilePrefabName = "Projectile";

    // Entity IDs
    private static int playerID = 0;
    private static int enemyID = 0;
    private static int projectileID = 0;

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
        Player = players[0];
        // Set up follow camera
        followCamera = GameObject.Find(FollowCameraName).GetComponent<CinemachineVirtualCamera>();
        followCamera.Follow = players[0].ShipObject.transform;
        // Initialize background
        Background.Start();
        // Spawn enemy
        SpawnEnemy();
    }

    // Update is called once per frame
    public static void Update()
    {
        // Update all entities
        foreach(KeyValuePair<int, PlayerShip> player in players)
        {
            if(player.Value.Alive)
            {
                player.Value.Update();
            }
        }
        //foreach(KeyValuePair<int, Ship> friendly in friendlies)
        //{
        //    if(friendly.Value.Alive)
        //    {
        //        friendly.Value.Update();
        //    }
        //}
        foreach(KeyValuePair<int, EnemyShip> enemy in enemies)
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
        // Update Background
        Background.Update();
    }

    // Fixed Update is called a fixed number of times per second
    public static void FixedUpdate()
    {
        // FixedUpdate all entities
        foreach(KeyValuePair<int, PlayerShip> player in players)
        {
            if(player.Value.Alive)
            {
                player.Value.FixedUpdate();
            }
        }
        //foreach(KeyValuePair<int, Ship> friendly in friendlies)
        //{
        //    if(friendly.Value.Alive)
        //    {
        //        friendly.Value.FixedUpdate();
        //    }
        //}
        foreach(KeyValuePair<int, EnemyShip> enemy in enemies)
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
        if(enemiesToRemove.Count > 0)
        {
            foreach(int enemyID in enemiesToRemove)
            {
                enemies.Remove(enemyID);
            }
            enemiesToRemove.Clear();
        }
        if(projectilesToRemove.Count > 0)
        {
            foreach(int projectileID in projectilesToRemove)
            {
                projectiles.Remove(projectileID);
            }
            projectilesToRemove.Clear();
        }
    }

    // Initialize player ship in world
    public static void SpawnPlayer()
    {
        players.Add(playerID, new PlayerShip(playerID));
        playerID++;
        if(playerID == int.MaxValue)
        {
            playerID = 0;
        }
    }

    // Spawn enemies
    public static void SpawnEnemy()
    {
        enemies.Add(enemyID, new EnemyShip(enemyID));
        enemyID++;
        if(enemyID == int.MaxValue)
        {
            enemyID = 0;
        }
    }

    // Spawn projectiles
    public static void SpawnProjectile(IFF _iff, float _damage, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _speed, float _lifetime)
    {
        projectiles.Add(projectileID, new Projectile(projectileID, _iff, _damage, _position, _rotation, _velocity, _speed, _lifetime));
        projectileID++;
        if(projectileID == int.MaxValue)
        {
            projectileID = 0;
        }
    }

    // Recieve Collision info and propogate
    public static void Collide(GameObject _collisionReporter, GameObject _collidedWith)
    {
        if(_collisionReporter.tag == "Projectile")
        {
            Projectile projectile = projectiles[int.Parse(_collisionReporter.name)];
            if(projectile.IFF == IFF.friend && _collidedWith.tag == "Enemy")
            {
                projectile.ReceivedCollision();
                enemies[int.Parse(_collidedWith.name)].ReceivedCollision(projectile.Damage);
            }
            if(projectile.IFF == IFF.enemy && _collidedWith.tag == "Player")
            {
                projectile.ReceivedCollision();
                players[int.Parse(_collidedWith.name)].ReceivedCollision(projectile.Damage);
            }
        }
    }
}
