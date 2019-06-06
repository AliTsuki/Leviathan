using System;
using System.Collections.Generic;

using UnityEngine;

// Controls the background tiles
public static class Background
{
    // Lists and Dicts
    public static Dictionary<Vector2Int, BackgroundTile> Tilemap = new Dictionary<Vector2Int, BackgroundTile>();
    private static Dictionary<string, GameObject> BackgroundPrefabs = new Dictionary<string, GameObject>();
    private static Dictionary<Vector2Int, GameObject> Backgrounds = new Dictionary<Vector2Int, GameObject>();
    private static List<Vector2Int> BackgroundsToRemove = new List<Vector2Int>();

    // Next background fields
    private static Vector2Int NextBackgroundKey;
    private static Vector3 NextBackgroundPosition;
    private static BackgroundTile NextBackgroundTile;

    // Player position
    private static Vector3 PlayerPosition;

    // Tilemap
    public static Texture2D TilemapTexture;
    private static bool BackgroundDependenciesLoaded = false;

    // Constants
    private const int BackgroundInitializationTileAmount = 2;
    private const int BackgroundGenerationTileAmount = 2;
    private const uint BackgroundTileSize = 200;
    private const uint BackgroundMaxDistanceFromPlayer = 500;


    // Initialize
    public static void Initialize()
    {
        // Clear lists
        Backgrounds.Clear();
        BackgroundsToRemove.Clear();
        // If tilemap and prefabs are not loaded
        if(BackgroundDependenciesLoaded == false)
        {
            // Load tilemap
            TilemapTexture = Resources.Load<Texture2D>(GameController.TilemapName);
            // Read tilemap
            StoreTilemapInDict();
            // Loop through zone types
            foreach(Zone.ZoneType ZoneType in (Zone.ZoneType[])Enum.GetValues(typeof(Zone.ZoneType)))
            {
                // Loop through tile types
                foreach(BackgroundTile.TileTypeEnum TileType in (BackgroundTile.TileTypeEnum[])Enum.GetValues(typeof(BackgroundTile.TileTypeEnum)))
                {
                    // Loop through tile rotations
                    foreach(BackgroundTile.TileDirectionEnum TileRotation in (BackgroundTile.TileDirectionEnum[])Enum.GetValues(typeof(BackgroundTile.TileDirectionEnum)))
                    {
                        try
                        {
                            // Load background prefab by zone type, tile type, and rotation
                            BackgroundPrefabs.Add($@"{ZoneType}:{TileType}:{TileRotation}", Resources.Load<GameObject>(GameController.BackgroundPrefabName + $@"/{ZoneType}/{TileType} {TileRotation}"));
                        }
                        catch(Exception e)
                        {
                            Debug.Log(e);
                            Debug.Log($@"Failed to find background prefab");
                            Logger.Log(e);
                            Logger.Log($@"Failed to find background prefab");
                        }
                    }
                }
            }
            // Acknowledge tilemaps and prefabs loaded
            BackgroundDependenciesLoaded = true;
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

    // Convert tilemap pixel coords to tile coords
    public static Vector2Int TilemapPixelCoordsToTileCoords(Vector2Int _pixelCoords)
    {
        return new Vector2Int(_pixelCoords.x - (TilemapTexture.width / 2), _pixelCoords.y - (TilemapTexture.height / 2));
    }

    // Convert world coords to tile coords
    public static Vector2Int WorldCoordsToTileCoords(Vector3 _worldCoords)
    {
        return new Vector2Int(Mathf.RoundToInt(_worldCoords.x / BackgroundTileSize), Mathf.RoundToInt(_worldCoords.z / BackgroundTileSize));
    }

    // Convert tile coords to world coords
    public static Vector3 TileCoordsToWorldCoords(Vector2Int _tileCoords)
    {
        return new Vector3(_tileCoords.x * BackgroundTileSize, 0, _tileCoords.y * BackgroundTileSize);
    }

    // Store tilemap in Dict
    private static void StoreTilemapInDict()
    {
        try
        {
            // Loop through width of tilemap
            for(int x = 0; x < TilemapTexture.width; x++)
            {
                // Loop through height of tilemap
                for(int z = 0; z < TilemapTexture.height; z++)
                {
                    // Set pixel coords
                    Vector2Int PixelCoords = new Vector2Int(x, z);
                    // Get zone at pixel coords
                    Zone.ZoneType ZoneType = Zone.GetZoneAtPixelCoords(new Vector2Int(x, z));
                    // Reset tile type and rotation
                    byte TileTypeAndRotation = 0;
                    // Get tile type and rotation by checking nearby tiles and adding a byte value for each case
                    if(Zone.GetZoneAtPixelCoords(new Vector2Int(x + 1, z - 1)) == ZoneType)
                    {
                        TileTypeAndRotation += 8;
                    }
                    if(Zone.GetZoneAtPixelCoords(new Vector2Int(x - 1, z - 1)) == ZoneType)
                    {
                        TileTypeAndRotation += 4;
                    }
                    if(Zone.GetZoneAtPixelCoords(new Vector2Int(x + 1, z + 1)) == ZoneType)
                    {
                        TileTypeAndRotation += 2;
                    }
                    if(Zone.GetZoneAtPixelCoords(new Vector2Int(x - 1, z + 1)) == ZoneType)
                    {
                        TileTypeAndRotation += 1;
                    }
                    // Convert pixel coords to tile coords
                    Vector2Int TileCoords = TilemapPixelCoordsToTileCoords(PixelCoords);
                    // Add a new tile at tile coords of zone type with specified tile type and rotation
                    Tilemap.Add(TileCoords, new BackgroundTile(ZoneType, TileTypeAndRotation));
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Debug.Log($@"Failed to parse tilemap image");
            Logger.Log(e);
            Logger.Log($@"Failed to parse tilemap image");
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
                NextBackgroundPosition = TileCoordsToWorldCoords(NextBackgroundKey);
                // Get what type of background tile to add at this position
                NextBackgroundTile = Tilemap[NextBackgroundKey];
                // Add a new background prefab at the next background position and add it to backgrounds list
                Backgrounds.Add(NextBackgroundKey, GameObject.Instantiate(BackgroundPrefabs[$@"{NextBackgroundTile.ZoneType}:{NextBackgroundTile.TileType}:{NextBackgroundTile.Rotation}"], NextBackgroundPosition, Quaternion.identity));
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
                    NextBackgroundTile = Tilemap[NextBackgroundKey];
                    // Add a new background prefab at the next background position and add it to backgrounds list
                    Backgrounds.Add(NextBackgroundKey, GameObject.Instantiate(BackgroundPrefabs[$@"{NextBackgroundTile.ZoneType}:{NextBackgroundTile.TileType}:{NextBackgroundTile.Rotation}"], NextBackgroundPosition, Quaternion.identity));
                    // Set name of GameObject in Unity Editor to the key
                    Backgrounds[NextBackgroundKey].name = $@"Background: {NextBackgroundKey.x}, {NextBackgroundKey.y}";
                }
            }
        }
    }

    // Restart
    public static void Restart()
    {
        // Loop through all backgrounds
        foreach(KeyValuePair<Vector2Int, GameObject> background in Backgrounds)
        {
            // Destroy backgrounds
            GameObject.Destroy(background.Value);
        }
    }
}

// Stores specific background tiles including zone type, tile type, and rotation
public struct BackgroundTile
{
    // Fields
    public enum TileTypeEnum
    {
        Full,
        Half,
        Corner,
    }
    public enum TileDirectionEnum
    {
        North,
        East,
        South,
        West,
        NorthEast,
        SouthEast,
        SouthWest,
        NorthWest
    }
    public Zone.ZoneType ZoneType;
    public TileTypeEnum TileType;
    public TileDirectionEnum Rotation;

    // Background tile constructor
    public BackgroundTile(Zone.ZoneType _zoneType, byte _tileTypeAndRotation)
    {
        // Assign zone type to zone type passed in
        this.ZoneType = _zoneType;
        // Set tile type and rotation to byte value passed in
        if(_tileTypeAndRotation == 0)
        {
            this.TileType = TileTypeEnum.Full;
            this.Rotation = TileDirectionEnum.North;
        }
        else if(_tileTypeAndRotation == 1)
        {
            this.TileType = TileTypeEnum.Corner;
            this.Rotation = TileDirectionEnum.SouthEast;
        }
        else if(_tileTypeAndRotation == 2)
        {
            this.TileType = TileTypeEnum.Corner;
            this.Rotation = TileDirectionEnum.SouthWest;
        }
        else if(_tileTypeAndRotation == 3)
        {
            this.TileType = TileTypeEnum.Half;
            this.Rotation = TileDirectionEnum.South;
        }
        else if(_tileTypeAndRotation == 4)
        {
            this.TileType = TileTypeEnum.Corner;
            this.Rotation = TileDirectionEnum.NorthEast;
        }
        else if(_tileTypeAndRotation == 5)
        {
            this.TileType = TileTypeEnum.Half;
            this.Rotation = TileDirectionEnum.East;
        }
        else if(_tileTypeAndRotation == 6)
        {
            this.TileType = TileTypeEnum.Full;
            this.Rotation = TileDirectionEnum.North;
        }
        else if(_tileTypeAndRotation == 7)
        {
            this.TileType = TileTypeEnum.Full;
            this.Rotation = TileDirectionEnum.North;
        }
        else if(_tileTypeAndRotation == 8)
        {
            this.TileType = TileTypeEnum.Corner;
            this.Rotation = TileDirectionEnum.NorthWest;
        }
        else if(_tileTypeAndRotation == 9)
        {
            this.TileType = TileTypeEnum.Full;
            this.Rotation = TileDirectionEnum.North;
        }
        else if(_tileTypeAndRotation == 10)
        {
            this.TileType = TileTypeEnum.Half;
            this.Rotation = TileDirectionEnum.West;
        }
        else if(_tileTypeAndRotation == 11)
        {
            this.TileType = TileTypeEnum.Full;
            this.Rotation = TileDirectionEnum.North;
        }
        else if(_tileTypeAndRotation == 12)
        {
            this.TileType = TileTypeEnum.Half;
            this.Rotation = TileDirectionEnum.North;
        }
        else if(_tileTypeAndRotation == 13)
        {
            this.TileType = TileTypeEnum.Full;
            this.Rotation = TileDirectionEnum.North;
        }
        else if(_tileTypeAndRotation == 14)
        {
            this.TileType = TileTypeEnum.Full;
            this.Rotation = TileDirectionEnum.North;
        }
        else if(_tileTypeAndRotation == 15)
        {
            this.TileType = TileTypeEnum.Full;
            this.Rotation = TileDirectionEnum.North;
        }
        else
        {
            this.TileType = TileTypeEnum.Full;
            this.Rotation = TileDirectionEnum.North;
        }
    }
}
