using Cinemachine;

using System.Collections.Generic;

using UnityEngine;

// Keeps track of all entites and updates all systems in the game
public class GameController
{
    public string FollowCameraName = "Follow Camera";
    public string BackgroundPrefabName = "Background";
    public string PlayerPrefabName = "Player Ship";
    public string EnemyPrefabName = "Player Ship";
    public string ProjectilePrefabName = "Projectile";

    private CinemachineVirtualCamera FollowCamera;

    public Dictionary<int, PlayerShip> playerShips = new Dictionary<int, PlayerShip>();
    private Dictionary<int, EnemyShip> enemyShips = new Dictionary<int, EnemyShip>();
    private Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();

    private readonly Background background = new Background();

    public int playerID = 0;
    public int enemyID = 0;
    public int projectileID = 0;

    // Start is called before the first frame update
    public void Start()
    {
        // Spawn player
        this.SpawnPlayer();
        // Start for all entities
        foreach(KeyValuePair<int, PlayerShip> player in this.playerShips)
        {
            player.Value.Start();
        }
        foreach(KeyValuePair<int, EnemyShip> enemy in this.enemyShips)
        {
            enemy.Value.Start();
        }
        foreach(KeyValuePair<int, Projectile> projectile in this.projectiles)
        {
            projectile.Value.Start();
        }
        // Set up follow camera
        this.FollowCamera = GameObject.Find(this.FollowCameraName).GetComponent<CinemachineVirtualCamera>();
        this.FollowCamera.Follow = this.playerShips[0].ship.transform;
        // Initialize background
        this.background.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        // Update all entities
        foreach(KeyValuePair<int, PlayerShip> player in this.playerShips)
        {
            player.Value.Update();
        }
        foreach(KeyValuePair<int, EnemyShip> enemy in this.enemyShips)
        {
            enemy.Value.Update();
        }
        foreach(KeyValuePair<int, Projectile> projectile in this.projectiles)
        {
            projectile.Value.Update();
        }
        // Update Background
        this.background.Update();
    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        // FixedUpdate all entities
        foreach(KeyValuePair<int, PlayerShip> player in this.playerShips)
        {
            player.Value.FixedUpdate();
        }
        foreach(KeyValuePair<int, EnemyShip> enemy in this.enemyShips)
        {
            enemy.Value.FixedUpdate();
        }
        foreach(KeyValuePair<int, Projectile> projectile in this.projectiles)
        {
            projectile.Value.FixedUpdate();
        }
    }

    // Initialize player ship in world
    public void SpawnPlayer()
    {
        this.playerShips.Add(this.playerID, new PlayerShip(this.playerID));
        Debug.Log($@"Added new Player: ID {this.playerID}");
        this.playerID++;
    }

    // Spawn enemies
    public void SpawnEnemy()
    {
        this.enemyShips.Add(this.enemyID, new EnemyShip(this.enemyID));
        Debug.Log($@"Added new Enemy: ID {this.enemyID}");
        this.enemyID++;
    }

    // Spawn projectiles
    public void SpawnProjectile(Vector3 _position, Quaternion _rotation, float _speed, float _lifetime)
    {
        this.projectiles.Add(this.projectileID, new Projectile(this.projectileID, _position, _rotation, _speed, _lifetime));
        Debug.Log($@"Added new Projectile: ID {this.projectileID}");
        this.projectileID++;
    }
}
