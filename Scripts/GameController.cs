using System.Collections.Generic;

using UnityEngine;

// Keeps track of all entites and updates all systems in the game
public class GameController
{
    public Dictionary<int, PlayerShip> playerShips = new Dictionary<int, PlayerShip>();
    public Dictionary<int, EnemyShip> enemyShips = new Dictionary<int, EnemyShip>();
    public Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();

    private Background background = new Background();

    public int playerID = 0;
    public int enemyID = 0;
    public int projectileID = 0;

    // Start is called before the first frame update
    public void Start()
    {
        // Initialize Player, Follow Camera, and Background
        SpawnPlayer();
        GameManager.FollowCameraStatic.Follow = playerShips[0].ship.transform;
        background.Start();
        // Start for all entities
        foreach(KeyValuePair<int, PlayerShip> player in playerShips)
        {
            player.Value.Start();
        }
        foreach(KeyValuePair<int, EnemyShip> enemy in enemyShips)
        {
            enemy.Value.Start();
        }
        foreach(KeyValuePair<int, Projectile> projectile in projectiles)
        {
            projectile.Value.Start();
        }
    }

    // Update is called once per frame
    public void Update()
    {
        // Update Background
        background.Update();
        // Update all entities
        foreach(KeyValuePair<int, PlayerShip> player in playerShips)
        {
            player.Value.Update();
        }
        foreach(KeyValuePair<int, EnemyShip> enemy in enemyShips)
        {
            enemy.Value.Update();
        }
        foreach(KeyValuePair<int, Projectile> projectile in projectiles)
        {
            projectile.Value.Update();
        }
    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        // FixedUpdate all entities
        foreach(KeyValuePair<int, PlayerShip> player in playerShips)
        {
            player.Value.FixedUpdate();
        }
        foreach(KeyValuePair<int, EnemyShip> enemy in enemyShips)
        {
            enemy.Value.FixedUpdate();
        }
        foreach(KeyValuePair<int, Projectile> projectile in projectiles)
        {
            projectile.Value.FixedUpdate();
        }
    }

    // Initialize player ship in world
    public void SpawnPlayer()
    {
        playerShips.Add(playerID, new PlayerShip(playerID));
        Debug.Log($@"Added new Player: ID {playerID}");
        playerID++;
    }

    // Spawn enemies
    public void SpawnEnemy()
    {
        enemyShips.Add(enemyID, new EnemyShip(enemyID));
        Debug.Log($@"Added new Enemy: ID {enemyID}");
        enemyID++;
    }

    // Spawn projectiles
    public void SpawnProjectile(Vector3 _position, Quaternion _rotation, float _speed, float _lifetime)
    {
        projectiles.Add(projectileID, new Projectile(projectileID, _position, _rotation, _speed, _lifetime));
        Debug.Log($@"Added new Projectile: ID {projectileID}");
        projectileID++;
    }
}
