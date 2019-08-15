using System;
using System.Collections.Generic;

using UnityEngine;

// Contains what type of enemies are located at which zones
public class Zone
{
    // Enemies in zone
    private static readonly EnemyAndSpawnRate[] DefaultZoneEnemies = new EnemyAndSpawnRate[]
    {
        // NONE
    };
    private static readonly EnemyAndSpawnRate[] GreenZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Standard, 85),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Ramming, 2),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 13)
    };
    private static readonly EnemyAndSpawnRate[] RedZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 10),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Flanker, 90)
    };
    private static readonly EnemyAndSpawnRate[] OrangeZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 10),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Flanker, 90)
    };
    private static readonly EnemyAndSpawnRate[] PurpleZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Standard, 25),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 75)
    };
    private static readonly EnemyAndSpawnRate[] BlueZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Standard, 40),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Ramming, 5),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 15),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Flanker, 40)
    };

    // Zone types
    public enum ZoneType
    {
        DefaultZone,
        GreenZone,
        RedZone,
        OrangeZone,
        PurpleZone,
        BlueZone
    }
    public ZoneType Type { get; private set; }

    // Zones
    private static Zone DefaultZone = new Zone(ZoneType.DefaultZone, DefaultZoneEnemies);
    private static Zone GreenZone = new Zone(ZoneType.GreenZone, GreenZoneEnemies);
    private static Zone RedZone = new Zone(ZoneType.RedZone, RedZoneEnemies);
    private static Zone OrangeZone = new Zone(ZoneType.OrangeZone, OrangeZoneEnemies);
    private static Zone PurpleZone = new Zone(ZoneType.PurpleZone, PurpleZoneEnemies);
    private static Zone BlueZone = new Zone(ZoneType.BlueZone, BlueZoneEnemies);

    // Dictionary of zones
    private static readonly Dictionary<ZoneType, Zone> Zones = new Dictionary<ZoneType, Zone>
    {
        {DefaultZone.Type, DefaultZone},
        {GreenZone.Type, GreenZone},
        {RedZone.Type, RedZone},
        {OrangeZone.Type, OrangeZone},
        {PurpleZone.Type, PurpleZone},
        {BlueZone.Type, BlueZone}
    };
    
    // Color dictionaries
    private static readonly Dictionary<string, Vector3> TilemapColorsByName = new Dictionary<string, Vector3>
    {
        {"white", new Vector3(1, 1, 1)},
        {"gray", new Vector3(0.5f, 0.5f, 0.5f)},
        {"black", new Vector3(0, 0, 0)},
        {"red", new Vector3(1, 0, 0)},
        {"orange", new Vector3(1, 0.5f, 0)},
        {"yellow", new Vector3(1, 1, 0)},
        {"green", new Vector3(0, 1, 0)},
        {"blue", new Vector3(0, 0, 1)},
        {"indigo", new Vector3(0, 1, 1)},
        {"purple", new Vector3(1, 0, 1)},
    };
    private static readonly Dictionary<Vector3, string> TilemapColorsByValue = new Dictionary<Vector3, string>
    {
        {new Vector3(1, 1, 1), "white"},
        {new Vector3(0.5f, 0.5f, 0.5f), "gray"},
        {new Vector3(0, 0, 0), "black"},
        {new Vector3(1, 0, 0), "red"},
        {new Vector3(1, 0.5f, 0), "orange"},
        {new Vector3(1, 1, 0), "yellow"},
        {new Vector3(0, 1, 0), "green"},
        {new Vector3(0, 0, 1), "blue"},
        {new Vector3(0, 1, 1), "indigo"},
        {new Vector3(1, 0, 1), "purple"},
    };

    // Constructor fields
    private EnemyAndSpawnRate[] ZoneEnemyTypeArray;
    private int TotalWeight;

    // Zone constructor
    public Zone(ZoneType zoneType, EnemyAndSpawnRate[] enemyArray)
    {
        this.Type = zoneType;
        this.ZoneEnemyTypeArray = enemyArray;
        for(int i = 0; i < this.ZoneEnemyTypeArray.Length; i++)
        {
            this.TotalWeight += this.ZoneEnemyTypeArray[i].SpawnRate;
        }
    }


    // Get zone type at pixel coords
    public static ZoneType GetZoneAtPixelCoords(Vector2Int pixelCoords)
    {
        Color color = Background.TilemapTexture.GetPixel(pixelCoords.x, pixelCoords.y);
        return GetZoneIDByColor(color);
    }

    // Get zone type at world coords
    public static ZoneType GetZoneAtWorldCoords(Vector3 worldCoords)
    {
        Vector2Int TileCoords = Background.WorldCoordsToTileCoords(worldCoords);
        return Background.Tilemap[TileCoords].ZoneType;
    }

    // Get zone ID by color value
    private static ZoneType GetZoneIDByColor(Color color)
    {
        // TODO: Color Tilemap file and add corresponding colors here and background prefabs with the byte number suffix
        try
        {
            string TilemapColor = TilemapColorsByValue[new Vector3(Mathematics.RoundToNearestHalf(color.r), Mathematics.RoundToNearestHalf(color.g), Mathematics.RoundToNearestHalf(color.b))];
            switch(TilemapColor)
            {
                case "black":
                {
                    return DefaultZone.Type;
                }
                case "green":
                {
                    return GreenZone.Type;
                }
                case "orange":
                {
                    return OrangeZone.Type;
                }
                case "red":
                {
                    return RedZone.Type;
                }
                case "purple":
                {
                    return PurpleZone.Type;
                }
                case "blue":
                {
                    return BlueZone.Type;
                }
                default:
                {
                    return DefaultZone.Type;
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log($@"Tilemap Color Not found: {color.r}:{color.g}:{color.b}");
            Logger.Log(e);
            Logger.Log($@"Tilemap Color Not found: {color.r}:{color.g}:{color.b}");
            return DefaultZone.Type;
        }
    }

    // Random weighted select enemy from enemy list for zone
    public static EnemyShip.EnemyShipType GetRandomEnemyTypeInZone(ZoneType zoneType)
    {
        // Default enemyID
        EnemyShip.EnemyShipType EnemyType = EnemyShip.EnemyShipType.Standard;
        // Get a random number between 0 and total weight for enemies in zone
        int randomNumber = GameController.RandomNumGen.Next(0, Zones[zoneType].TotalWeight);
        // Get enemy array
        EnemyAndSpawnRate[] EnemyArray = Zones[zoneType].ZoneEnemyTypeArray;
        // Loop through enemy array
        for(int i = 0; i < EnemyArray.Length; i++)
        {
            // If random number is less than spawn rate of this enemy
            if(randomNumber < EnemyArray[i].SpawnRate)
            {
                // Set enemy ID
                EnemyType = EnemyArray[i].EnemyType;
                break;
            }
            // Subtract spawn rate from random number
            randomNumber -= EnemyArray[i].SpawnRate;
        }
        // Return the enemy ID landed on last
        return EnemyType;
    }

    // Spawn enemy
    public static EnemyShip SpawnEnemy(uint shipID, Vector3 startingPosition)
    {
        // Get enemy type
        EnemyShip.EnemyShipType EnemyType = GetRandomEnemyTypeInZone(GetZoneAtWorldCoords(startingPosition));
        // Return enemy indicated by enemyID
        switch(EnemyType)
        {
            case EnemyShip.EnemyShipType.Standard:
            {
                return new ESStandard(shipID, startingPosition);
            }
            case EnemyShip.EnemyShipType.Ramming:
            {
                return new ESRamming(shipID, startingPosition);
            }
            case EnemyShip.EnemyShipType.Broadside:
            {
                return new ESBroadside(shipID, startingPosition);
            }
            case EnemyShip.EnemyShipType.Flanker:
            {
                return new ESFlanker(shipID, startingPosition);
            }
            default:
            {
                return null;
            }
        }
    }
}

public struct EnemyAndSpawnRate
{
    public EnemyShip.EnemyShipType EnemyType { get; private set; }
    public int SpawnRate { get; private set; }

    public EnemyAndSpawnRate(EnemyShip.EnemyShipType enemyType, int spawnRate)
    {
        this.EnemyType = enemyType;
        this.SpawnRate = spawnRate;
    }
}
