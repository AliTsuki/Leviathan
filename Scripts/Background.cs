using System.Collections.Generic;

using UnityEngine;

// Controls the background tiles
public class Background
{
    private static GameController controller = GameManager.instance;

    private GameObject BackgroundPrefab;
   
    private Dictionary<Vector2Int, GameObject> backgrounds = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> backgroundsToRemove = new List<Vector2Int>();
    private Vector2Int nextBackgroundKey;
    private Vector3 nextBackgroundPosition;
    private Vector3 playerPosition;

    private const int backgroundInitializationTileAmount = 2;
    private const int backgroundGenerationTileAmount = 2;
    private const int backgroundTileSize = 200;
    private const int backgroundMaxDistance = 500;


    // Start is called before the first frame update
    public void Start()
    {
        BackgroundPrefab = Resources.Load<GameObject>(controller.BackgroundPrefabName);
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
        for(int x = -backgroundInitializationTileAmount; x < backgroundInitializationTileAmount; x++)
        {
            for(int z = -backgroundInitializationTileAmount; z < backgroundInitializationTileAmount; z++)
            {
                nextBackgroundKey = new Vector2Int(x, z);
                nextBackgroundPosition = new Vector3((nextBackgroundKey.x * backgroundTileSize), 0, (nextBackgroundKey.y * backgroundTileSize));
                backgrounds.Add(nextBackgroundKey, GameObject.Instantiate(BackgroundPrefab, nextBackgroundPosition, Quaternion.identity));
                backgrounds[nextBackgroundKey].name = $@"Background: {nextBackgroundKey.x}, {nextBackgroundKey.y}";
            }
        }
    }

    // Checks for backgrounds distance from player and removes if they are too distant
    private void RemoveDistantBackgrounds()
    {
        playerPosition = controller.playerShips[0].ship.transform.position;
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
    private void AddNewBackgrounds()
    {
        playerPosition = controller.playerShips[0].ship.transform.position;
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
