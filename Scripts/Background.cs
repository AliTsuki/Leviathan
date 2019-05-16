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
        // Load background prefab
        BackgroundPrefab = Resources.Load(GameController.BackgroundPrefabName, typeof(GameObject)) as GameObject;
        // Initialize background
        InitializeBackground();
    }

    // Update is called once per frame
    public static void Update()
    {
        // Remove distant backgrounds
        RemoveDistantBackgrounds();
        // Add new backgrounds
        AddNewBackgrounds();
    }

    // Initializes the background at beginning of game
    private static void InitializeBackground()
    {
        // Loop through x and z coords from negative tile amount to positive
        for(int x = -backgroundInitializationTileAmount; x < backgroundInitializationTileAmount; x++)
        {
            for(int z = -backgroundInitializationTileAmount; z < backgroundInitializationTileAmount; z++)
            {
                // Set the next background key to be current x and z
                nextBackgroundKey = new Vector2Int(x, z);
                // Set the next background position to x and z multiplied by the size of the tiles
                nextBackgroundPosition = new Vector3(nextBackgroundKey.x * backgroundTileSize, 0, nextBackgroundKey.y * backgroundTileSize);
                // Add a new background prefab at the next background position and add it to backgrounds list
                backgrounds.Add(nextBackgroundKey, GameObject.Instantiate(BackgroundPrefab, nextBackgroundPosition, Quaternion.identity));
                // Set name of GameObject in Unity Editor to the key
                backgrounds[nextBackgroundKey].name = $@"Background: {nextBackgroundKey.x}, {nextBackgroundKey.y}";
            }
        }
    }

    // Checks for backgrounds distance from player and removes if they are too distant
    private static void RemoveDistantBackgrounds()
    {
        // Get player position from GameController
        playerPosition = GameController.Player.ShipObject.transform.position;
        // Loop through all backgrounds currently in game
        foreach(KeyValuePair<Vector2Int, GameObject> bg in backgrounds)
        {
            // Check if background is further away than max distance allowed
            if(Vector3.Distance(bg.Value.transform.position, playerPosition) > backgroundMaxDistance)
            {
                // Add background to removal list
                backgroundsToRemove.Add(bg.Key);
            }
        }
        // Loop through all backgrounds marked for removal
        foreach(Vector2Int pos in backgroundsToRemove)
        {
            // Destroy the background game object
            GameObject.Destroy(backgrounds[pos]);
            // Remove destroyed background from backgrounds list
            backgrounds.Remove(pos);
        }
        // Clear the background marked for removal list
        backgroundsToRemove.Clear();
    }

    // Adds new backgrounds around player as you explore
    private static void AddNewBackgrounds()
    {
        // Get player position from GameController
        playerPosition = GameController.Player.ShipObject.transform.position;
        // Loop through x and z coords from negative tile amount to positive
        for(int x = -backgroundGenerationTileAmount; x < backgroundGenerationTileAmount; x++)
        {
            for(int z = -backgroundGenerationTileAmount; z < backgroundGenerationTileAmount; z++)
            {
                // Set next background key while taking into account player position
                nextBackgroundKey = new Vector2Int(Mathf.RoundToInt(playerPosition.x / backgroundTileSize) + x, Mathf.RoundToInt(playerPosition.z / backgroundTileSize) + z);
                // Set next background position taking into account tile size
                nextBackgroundPosition = new Vector3(nextBackgroundKey.x * backgroundTileSize, 0, nextBackgroundKey.y * backgroundTileSize);
                // Check if next background already exists
                if(backgrounds.ContainsKey(nextBackgroundKey) == false)
                {
                    // If background doesn't already exist, add new background into game and into backgrounds list
                    backgrounds.Add(nextBackgroundKey, GameObject.Instantiate(BackgroundPrefab, nextBackgroundPosition, Quaternion.identity));
                    // Set name of GameObject in Unity Editor to the key
                    backgrounds[nextBackgroundKey].name = $@"Background: {nextBackgroundKey.x}, {nextBackgroundKey.y}";
                }
            }
        }
    }
}
