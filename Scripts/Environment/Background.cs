﻿using System.Collections.Generic;

using UnityEngine;

// Controls the background tiles
public static class Background
{
    // GameObjects
    private static GameObject[] BackgroundPrefabs;
    
    // Tilemap
    private static Texture2D Tilemap;
   
    // Lists and Dicts
    private static readonly Dictionary<Vector2Int, GameObject> Backgrounds = new Dictionary<Vector2Int, GameObject>();
    private static List<Vector2Int> BackgroundsToRemove = new List<Vector2Int>();

    // Next background fields
    private static Vector2Int NextBackgroundKey;
    private static Vector3 NextBackgroundPosition;
    private static byte NextBackgroundType;

    // Player position
    private static Vector3 PlayerPosition;

    // Constants
    private const int BackgroundPrefabsAmount = 1;
    private const int BackgroundInitializationTileAmount = 2;
    private const int BackgroundGenerationTileAmount = 2;
    private const int BackgroundTileSize = 200;
    private const int BackgroundMaxDistanceFromPlayer = 500;


    // Initialize
    public static void Initialize()
    {
        // Clear lists
        Backgrounds.Clear();
        BackgroundsToRemove.Clear();
        // Load tilemap
        Tilemap = Resources.Load<Texture2D>(GameController.TilemapName);
        // Load background prefabs
        BackgroundPrefabs = new GameObject[BackgroundPrefabsAmount];
        for(int i = 0; i < BackgroundPrefabsAmount; i++)
        {
            BackgroundPrefabs[i] = Resources.Load<GameObject>(GameController.BackgroundPrefabName + $@" {i}");
        }
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

    // Get background tile type at position
    private static byte GetBackgroundTypeAtPosition(Vector2Int _pos)
    {
        Color color = Tilemap.GetPixel(_pos.x + (Tilemap.width / 2), _pos.y + (Tilemap.height / 2));
        // TODO: Color Tilemap file and add corresponding colors here and background prefabs with the byte number suffix
        if(color == Color.black)
        {
            return 0;
        }
        else if(color == Color.white)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    // Initializes the background at beginning of game
    private static void InitializeBackground()
    {
        // Loop through x and z coords from negative tile amount to positive
        for(int x = -BackgroundInitializationTileAmount; x < BackgroundInitializationTileAmount; x++)
        {
            for(int z = -BackgroundInitializationTileAmount; z < BackgroundInitializationTileAmount; z++)
            {
                // Set the next background key to be current x and z
                NextBackgroundKey = new Vector2Int(x, z);
                // Set the next background position to x and z multiplied by the size of the tiles
                NextBackgroundPosition = new Vector3(NextBackgroundKey.x * BackgroundTileSize, 0, NextBackgroundKey.y * BackgroundTileSize);
                // Get what type of background tile to add at this position
                NextBackgroundType = GetBackgroundTypeAtPosition(NextBackgroundKey);
                // Add a new background prefab at the next background position and add it to backgrounds list
                Backgrounds.Add(NextBackgroundKey, GameObject.Instantiate(BackgroundPrefabs[NextBackgroundType], NextBackgroundPosition, Quaternion.identity));
                // Set name of GameObject in Unity Editor to the key
                Backgrounds[NextBackgroundKey].name = $@"Background: {NextBackgroundKey.x}, {NextBackgroundKey.y}";
            }
        }
    }

    // Checks for backgrounds distance from player and removes if they are too distant
    private static void RemoveDistantBackgrounds()
    {
        // Get player position from GameController
        PlayerPosition = GameController.Player.ShipObject.transform.position;
        // Loop through all backgrounds currently in game
        foreach(KeyValuePair<Vector2Int, GameObject> bg in Backgrounds)
        {
            // Check if background is further away than max distance allowed
            if(Vector3.Distance(bg.Value.transform.position, PlayerPosition) > BackgroundMaxDistanceFromPlayer)
            {
                // Add background to removal list
                BackgroundsToRemove.Add(bg.Key);
            }
        }
        // Loop through all backgrounds marked for removal
        foreach(Vector2Int pos in BackgroundsToRemove)
        {
            // Destroy the background game object
            GameObject.Destroy(Backgrounds[pos]);
            // Remove destroyed background from backgrounds list
            Backgrounds.Remove(pos);
        }
        // Clear the background marked for removal list
        BackgroundsToRemove.Clear();
    }

    // Adds new backgrounds around player as you explore
    private static void AddNewBackgrounds()
    {
        // Get player position from GameController
        PlayerPosition = GameController.Player.ShipObject.transform.position;
        // Loop through x and z coords from negative tile amount to positive
        for(int x = -BackgroundGenerationTileAmount; x < BackgroundGenerationTileAmount; x++)
        {
            for(int z = -BackgroundGenerationTileAmount; z < BackgroundGenerationTileAmount; z++)
            {
                // Set next background key while taking into account player position
                NextBackgroundKey = new Vector2Int(Mathf.RoundToInt(PlayerPosition.x / BackgroundTileSize) + x, Mathf.RoundToInt(PlayerPosition.z / BackgroundTileSize) + z);
                // Set next background position taking into account tile size
                NextBackgroundPosition = new Vector3(NextBackgroundKey.x * BackgroundTileSize, 0, NextBackgroundKey.y * BackgroundTileSize);
                // Check if next background already exists
                if(Backgrounds.ContainsKey(NextBackgroundKey) == false)
                {
                    // Get what type of background tile to add at this position
                    NextBackgroundType = GetBackgroundTypeAtPosition(NextBackgroundKey);
                    // If background doesn't already exist, add new background into game and into backgrounds list
                    Backgrounds.Add(NextBackgroundKey, GameObject.Instantiate(BackgroundPrefabs[NextBackgroundType], NextBackgroundPosition, Quaternion.identity));
                    // Set name of GameObject in Unity Editor to the key
                    Backgrounds[NextBackgroundKey].name = $@"Background: {NextBackgroundKey.x}, {NextBackgroundKey.y}";
                }
            }
        }
    }

    // Restart
    public static void Restart()
    {
        foreach(KeyValuePair<Vector2Int, GameObject> background in Backgrounds)
        {
            GameObject.Destroy(background.Value);
        }
    }
}
