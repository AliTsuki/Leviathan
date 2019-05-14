using System.Collections.Generic;

using UnityEngine;

// Controls the background tiles
public static class Background
{
    // GameObjects
    private static GameObject BackgroundPrefab;
   
    // Fields
    private static readonly Dictionary<Vector2Int, GameObject> backgrounds = new Dictionary<Vector2Int, GameObject>();
    private static List<Vector2Int> backgroundsToRemove = new List<Vector2Int>();
    private static Vector2Int nextBackgroundKey;
    private static Vector3 nextBackgroundPosition;
    private static Vector3 playerPosition;

    // Constants
    private const int backgroundInitializationTileAmount = 2;
    private const int backgroundGenerationTileAmount = 2;
    private const int backgroundTileSize = 200;
    private const int backgroundMaxDistance = 500;


    // Start is called before the first frame update
    public static void Start()
    {
        BackgroundPrefab = Resources.Load(GameController.BackgroundPrefabName, typeof(GameObject)) as GameObject;
        InitializeBackground();
    }

    // Update is called once per frame
    public static void Update()
    {
        RemoveDistantBackgrounds();
        AddNewBackgrounds();
    }

    // Initializes the background at beginning of game
    private static void InitializeBackground()
    {
        for(int x = -backgroundInitializationTileAmount; x < backgroundInitializationTileAmount; x++)
        {
            for(int z = -backgroundInitializationTileAmount; z < backgroundInitializationTileAmount; z++)
            {
                nextBackgroundKey = new Vector2Int(x, z);
                nextBackgroundPosition = new Vector3(nextBackgroundKey.x * backgroundTileSize, 0, nextBackgroundKey.y * backgroundTileSize);
                backgrounds.Add(nextBackgroundKey, GameObject.Instantiate(BackgroundPrefab, nextBackgroundPosition, Quaternion.identity));
                backgrounds[nextBackgroundKey].name = $@"Background: {nextBackgroundKey.x}, {nextBackgroundKey.y}";
            }
        }
    }

    // Checks for backgrounds distance from player and removes if they are too distant
    private static void RemoveDistantBackgrounds()
    {
        playerPosition = GameController.Player.ShipObject.transform.position;
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

    // Adds new backgrounds around player as you explore
    private static void AddNewBackgrounds()
    {
        playerPosition = GameController.Player.ShipObject.transform.position;
        for(int x = -backgroundGenerationTileAmount; x < backgroundGenerationTileAmount; x++)
        {
            for(int z = -backgroundGenerationTileAmount; z < backgroundGenerationTileAmount; z++)
            {
                nextBackgroundKey = new Vector2Int(Mathf.RoundToInt(playerPosition.x / backgroundTileSize) + x, Mathf.RoundToInt(playerPosition.z / backgroundTileSize) + z);
                nextBackgroundPosition = new Vector3(nextBackgroundKey.x * backgroundTileSize, 0, nextBackgroundKey.y * backgroundTileSize);
                if(!backgrounds.ContainsKey(nextBackgroundKey))
                {
                    backgrounds.Add(nextBackgroundKey, GameObject.Instantiate(BackgroundPrefab, nextBackgroundPosition, Quaternion.identity));
                    backgrounds[nextBackgroundKey].name = $@"Background: {nextBackgroundKey.x}, {nextBackgroundKey.y}";
                }
            }
        }
    }
}
