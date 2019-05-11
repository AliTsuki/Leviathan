using System.Collections.Generic;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject background;
    public Transform backgroundParent;
    private Dictionary<Vector2Int, GameObject> backgroundArray = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> backgroundsToRemove = new List<Vector2Int>();
    private Vector2Int nextBackgroundKey;
    private Vector3 nextBackgroundPosition;
    public Transform player;
    private Vector3 playerPosition;
    private const int backgroundInitializationTileAmount = 2;
    private const int backgroundGenerationTileAmount = 2;
    private const int backgroundTileSize = 200;
    private const int backgroundMaxDistance = 500;

    // Start is called before the first frame update
    void Start()
    {
        InitializeBackground();
    }

    // Update is called once per frame
    void Update()
    {
        RemoveDistantBackgrounds();
        AddNewBackgrounds();
    }

    // Fixed Update is called a fixed number of times per second
    private void FixedUpdate()
    {

    }

    // Initializes the background at beginning of game
    private void InitializeBackground()
    {
        playerPosition = player.position;
        for(int x = -backgroundInitializationTileAmount; x < backgroundInitializationTileAmount; x++)
        {
            for(int z = -backgroundInitializationTileAmount; z < backgroundInitializationTileAmount; z++)
            {
                nextBackgroundKey = new Vector2Int(x, z);
                nextBackgroundPosition = new Vector3((nextBackgroundKey.x * backgroundTileSize), 0, (nextBackgroundKey.y * backgroundTileSize));
                backgroundArray.Add(nextBackgroundKey, Instantiate(background, nextBackgroundPosition, Quaternion.identity));
                backgroundArray[nextBackgroundKey].name = $@"Background: {nextBackgroundKey.x}, {nextBackgroundKey.y}";
                backgroundArray[nextBackgroundKey].transform.parent = backgroundParent;
            }
        }
    }

    // Checks for backgrounds distance from player and removes is they are too distant
    private void RemoveDistantBackgrounds()
    {
        playerPosition = player.position;
        foreach(KeyValuePair<Vector2Int, GameObject> bg in backgroundArray)
        {
            if(Vector3.Distance(bg.Value.transform.position, playerPosition) > backgroundMaxDistance)
            {
                backgroundsToRemove.Add(bg.Key);
            }
        }
        foreach(Vector2Int pos in backgroundsToRemove)
        {
            Destroy(backgroundArray[pos]);
            backgroundArray.Remove(pos);
        }
        backgroundsToRemove.Clear();
    }

    // Adds new backgrounds around player as you explore
    private void AddNewBackgrounds()
    {
        playerPosition = player.position;
        for(int x = -backgroundGenerationTileAmount; x < backgroundGenerationTileAmount; x++)
        {
            for(int z = -backgroundGenerationTileAmount; z < backgroundGenerationTileAmount; z++)
            {
                nextBackgroundKey = new Vector2Int(Mathf.RoundToInt(playerPosition.x / backgroundTileSize) + x, Mathf.RoundToInt(playerPosition.z / backgroundTileSize) + z);
                nextBackgroundPosition = new Vector3(nextBackgroundKey.x * backgroundTileSize, 0, nextBackgroundKey.y * backgroundTileSize);
                if(!backgroundArray.ContainsKey(nextBackgroundKey))
                {
                    backgroundArray.Add(nextBackgroundKey, Instantiate(background, nextBackgroundPosition, Quaternion.identity));
                    backgroundArray[nextBackgroundKey].name = $@"Background: {nextBackgroundKey.x}, {nextBackgroundKey.y}";
                    backgroundArray[nextBackgroundKey].transform.parent = backgroundParent;
                }
            }
        }
    }
}
