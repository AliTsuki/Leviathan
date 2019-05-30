using System.Collections.Generic;

using UnityEngine;

// TODO: Come up with a solution for hard edges between zones
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
    public static EnemyAndSpawnRate[] StarterZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Standard, 85),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Ramming, 5),
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 10)
    };
    public static EnemyAndSpawnRate[] RedZoneEnemies = new EnemyAndSpawnRate[]
    {
        new EnemyAndSpawnRate(EnemyShip.EnemyShipType.Broadside, 100)
    };

    // Zone types
    public enum ZoneType
    {
        DefaultZone,
        StarterZone,
        RedZone
    }
    public ZoneType Type;

    // Zones
    public static Zone DefaultZone = new Zone(ZoneType.DefaultZone, DefaultZoneEnemies);
    public static Zone StarterZone = new Zone(ZoneType.StarterZone, StarterZoneEnemies);
    public static Zone RedZone = new Zone(ZoneType.RedZone, RedZoneEnemies);

    // Dictionary of zones
    public static Dictionary<ZoneType, Zone> Zones = new Dictionary<ZoneType, Zone>
    {
        {DefaultZone.Type, DefaultZone},
        {StarterZone.Type, StarterZone},
        {RedZone.Type, RedZone}
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
    public static ZoneType GetZoneAtPosition(Vector2Int _pos)
    {
        Color color = Background.Tilemap.GetPixel(_pos.x + (Background.Tilemap.width / 2), _pos.y + (Background.Tilemap.height / 2));
        return GetZoneIDByColor(color);
    }

    // Get zone type at position
    public static ZoneType GetZoneAtPosition(Vector3 _pos)
    {
        Vector2Int position = Background.WorldCoordsToTileCoords(_pos);
        Color color = Background.Tilemap.GetPixel(position.x + (Background.Tilemap.width / 2), position.y + (Background.Tilemap.height / 2));
        return GetZoneIDByColor(color);
    }

    // Get zone ID by color value
    private static ZoneType GetZoneIDByColor(Color color)
    {
        // TODO: Color Tilemap file and add corresponding colors here and background prefabs with the byte number suffix
        if(color == Color.black)
        {
            return StarterZone.Type;
        }
        else if(color == Color.red)
        {
            return ZoneType.RedZone;
        }
        else
        {
            return ZoneType.DefaultZone;
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
        EnemyShip.EnemyShipType EnemyType = GetRandomEnemyTypeInZone(GetZoneAtPosition(_startingPosition));
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
