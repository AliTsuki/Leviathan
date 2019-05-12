using System.Collections.Generic;

using UnityEngine;

// Controls the background tiles
public class Background
{
    private GameController instance = GameManager.instance;

    private Dictionary<Vector2Int, GameObject> backgrounds = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> backgroundsToRemove = new List<Vector2Int>();
    private Vector2Int nextBackgroundKey;
    private Vector3 nextBackgroundPosition;
    private Vector3 playerPosition;

    public const int backgroundInitializationTileAmount = 2;
    public const int backgroundGenerationTileAmount = 2;
    public const int backgroundTileSize = 200;
    public const int backgroundMaxDistance = 500;


    // Start is called before the first frame update
    public void Start()
    {
        InitializeBackground();
    }

    // Update is called once per frame
    public void Update()
    {
        RemoveDistantBackgrounds();
        AddNewBackgrounds();
    }

    // Initializes the background at beginning of game
    private void InitializeBackground()
    {
        foreach(KeyValuePair<int, PlayerShip> player in instance.playerShips)
        {
            for(int x = -backgroundInitializationTileAmount; x < backgroundInitializationTileAmount; x++)
            {
                for(int z = -backgroundInitializationTileAmount; z < backgroundInitializationTileAmount; z++)
                {
                    nextBackgroundKey = new Vector2Int(x, z);
                    nextBackgroundPosition = new Vector3((nextBackgroundKey.x * backgroundTileSize), 0, (nextBackgroundKey.y * backgroundTileSize));
                    backgrounds.Add(nextBackgroundKey, GameObject.Instantiate(GameManager.BackgroundPrefabStatic, nextBackgroundPosition, Quaternion.identity));
                    backgrounds[nextBackgroundKey].name = $@"Background: {nextBackgroundKey.x}, {nextBackgroundKey.y}";
                    backgrounds[nextBackgroundKey].transform.parent = GameManager.BackgroundParentStatic.transform;
                }
            }
        }
    }

    // Checks for backgrounds distance from player and removes if they are too distant
    private void RemoveDistantBackgrounds()
    {
        if(instance.playerShips.Count > 0)
        {
            foreach(KeyValuePair<int, PlayerShip> player in instance.playerShips)
            {
                playerPosition = player.Value.ship.transform.position;
                foreach(KeyValuePair<Vector2Int, GameObject> bg in backgrounds)
                {
                    if(Vector3.Distance(bg.Value.transform.position, playerPosition) > backgroundMaxDistance)
                    {
                        backgroundsToRemove.Add(bg.Key);
                    }
                }
                foreach(Vector2Int pos in backgroundsToRemove)
                {
                    GameObject.Destroy(backgrounds[pos]);
                    backgrounds.Remove(pos);
                }
                backgroundsToRemove.Clear();
            }
        }
    }

    // Adds new backgrounds around player as you explore
    private void AddNewBackgrounds()
    {
        foreach(KeyValuePair<int, PlayerShip> player in instance.playerShips)
        {
            playerPosition = player.Value.ship.transform.position;
            for(int x = -backgroundGenerationTileAmount; x < backgroundGenerationTileAmount; x++)
            {
                for(int z = -backgroundGenerationTileAmount; z < backgroundGenerationTileAmount; z++)
                {
                    nextBackgroundKey = new Vector2Int(Mathf.RoundToInt(playerPosition.x / backgroundTileSize) + x, Mathf.RoundToInt(playerPosition.z / backgroundTileSize) + z);
                    nextBackgroundPosition = new Vector3(nextBackgroundKey.x * backgroundTileSize, 0, nextBackgroundKey.y * backgroundTileSize);
                    if(!backgrounds.ContainsKey(nextBackgroundKey))
                    {
                        backgrounds.Add(nextBackgroundKey, GameObject.Instantiate(GameManager.BackgroundPrefabStatic, nextBackgroundPosition, Quaternion.identity));
                        backgrounds[nextBackgroundKey].name = $@"Background: {nextBackgroundKey.x}, {nextBackgroundKey.y}";
                        backgrounds[nextBackgroundKey].transform.parent = GameManager.BackgroundParentStatic.transform;
                    }
                }
            }
        }
    }
}
