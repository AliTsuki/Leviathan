using System.Collections.Generic;

using UnityEngine;

// Contains what type of enemies are located at which zones
public class Zone
{
    // Enemies in zone
    public static EnemyAndSpawnRate[] DefaultZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Standard, 85),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Ramming, 5),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 10)
    };
    public static EnemyAndSpawnRate[] GreenZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Standard, 85),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Ramming, 5),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 10)
    };
    public static EnemyAndSpawnRate[] RedZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 100)
    };
    public static EnemyAndSpawnRate[] OrangeZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Ramming, 100)
    };
    public static EnemyAndSpawnRate[] PurpleZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 100)
    };
    public static EnemyAndSpawnRate[] BlueZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Standard, 100)
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
    public ZoneType Type;

    // Zones
    public static Zone DefaultZone = new Zone(ZoneType.DefaultZone, DefaultZoneEnemies);
    public static Zone GreenZone = new Zone(ZoneType.GreenZone, GreenZoneEnemies);
    public static Zone RedZone = new Zone(ZoneType.RedZone, RedZoneEnemies);
    public static Zone OrangeZone = new Zone(ZoneType.OrangeZone, OrangeZoneEnemies);
    public static Zone PurpleZone = new Zone(ZoneType.PurpleZone, PurpleZoneEnemies);
    public static Zone BlueZone = new Zone(ZoneType.BlueZone, BlueZoneEnemies);

    // Dictionary of zones
    public static Dictionary<ZoneType, Zone> Zones = new Dictionary<ZoneType, Zone>
    {
        {DefaultZone.Type, DefaultZone},
        {GreenZone.Type, GreenZone},
        {RedZone.Type, RedZone},
        {OrangeZone.Type, OrangeZone},
        {PurpleZone.Type, PurpleZone},
        {BlueZone.Type, BlueZone}
    };

    // Constructor fields
    public EnemyAndSpawnRate[] ZoneEnemyTypeArray;
    public int TotalWeight;

    // Zone constructor
    public Zone(ZoneType _zoneType, EnemyAndSpawnRate[] _enemyArray)
    {
        this.Type = _zoneType;
        this.ZoneEnemyTypeArray = _enemyArray;
        for(int i = 0; i < this.ZoneEnemyTypeArray.Length; i++)
        {
            this.TotalWeight += this.ZoneEnemyTypeArray[i].SpawnRate;
        }
    }


    // Get zone type at position
    public static ZoneType GetZoneAtPixelCoords(Vector2Int _pixelCoords)
    {
        Color color = Background.TilemapTexture.GetPixel(_pixelCoords.x, _pixelCoords.y);
        return GetZoneIDByColor(color);
    }

    // Get zone type at position
    public static ZoneType GetZoneAtWorldCoords(Vector3 _worldCoords)
    {
        Vector2Int TileCoords = Background.WorldCoordsToTileCoords(_worldCoords);
        return Background.Tilemap[TileCoords].ZoneType;
    }

    // Get zone ID by color value
    private static ZoneType GetZoneIDByColor(Color color)
    {
        // TODO: Color Tilemap file and add corresponding colors here and background prefabs with the byte number suffix
        if(color == Color.black)
        {
            return DefaultZone.Type;
        }
        else if(color == Color.green)
        {
            return GreenZone.Type;
        }
        else if(color == Color.red)
        {
            return OrangeZone.Type;
        }
        else if(color == new Color(1, 0, 1, 1)) // Purple
        {
            return PurpleZone.Type;
        }
        else if(color == Color.blue)
        {
            return BlueZone.Type;
        }
        else
        {
            return DefaultZone.Type;
        }
    }

    // Random weighted select enemy from enemy list for zone
    public static EnemyShip.EnemyShipType GetRandomEnemyTypeInZone(ZoneType _zoneType)
    {
        // Default enemyID
        EnemyShip.EnemyShipType EnemyType = EnemyShip.EnemyShipType.Standard;
        // Get a random number between 0 and total weight for enemies in zone
        int randomNumber = GameController.r.Next(0, Zones[_zoneType].TotalWeight);
        // Get enemy array
        EnemyAndSpawnRate[] EnemyArray = Zones[_zoneType].ZoneEnemyTypeArray;
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
    public static EnemyShip SpawnEnemy(uint _shipID, Vector3 _startingPosition)
    {
        // Get enemy type
        EnemyShip.EnemyShipType EnemyType = GetRandomEnemyTypeInZone(GetZoneAtWorldCoords(_startingPosition));
        // Return enemy indicated by enemyID
        if(EnemyType == EnemyShip.EnemyShipType.Standard)
        {
            return new ESStandard(_shipID, _startingPosition);
        }
        else if(EnemyType == EnemyShip.EnemyShipType.Ramming)
        {
            return new ESRamming(_shipID, _startingPosition);
        }
        else if(EnemyType == EnemyShip.EnemyShipType.Broadside)
        {
            return new ESBroadside(_shipID, _startingPosition);
        }
        else
        {
            return null;
        }
    }
}

public struct EnemyAndSpawnRate
{
    public EnemyShip.EnemyShipType EnemyType;
    public int SpawnRate;

    public EnemyAndSpawnRate(EnemyShip.EnemyShipType _enemyType, int _spawnRate)
    {
        this.EnemyType = _enemyType;
        this.SpawnRate = _spawnRate;
    }
}
