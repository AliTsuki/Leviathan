using Cinemachine;

using UnityEngine;

// Holds all the GameObjects used by other classes, this is the only class that interacts with the Unity Editor directly
public class GameManager : MonoBehaviour
{
    public GameObject FollowCamera;
    public GameObject BackgroundParent;
    public GameObject BackgroundPrefab;
    public GameObject PlayerParent;
    public GameObject PlayerPrefab;
    public GameObject EnemyParent;
    public GameObject EnemyPrefab;
    public GameObject ProjectileParent;
    public GameObject ProjectilePrefab;

    public static CinemachineVirtualCamera FollowCameraStatic;
    public static GameObject BackgroundParentStatic;
    public static GameObject BackgroundPrefabStatic;
    public static GameObject PlayerParentStatic;
    public static GameObject PlayerPrefabStatic;
    public static GameObject EnemyParentStatic;
    public static GameObject EnemyPrefabStatic;
    public static GameObject ProjectileParentStatic;
    public static GameObject ProjectilePrefabStatic;

    public static GameController instance = new GameController();

    // Start is called before the first frame update
    public void Start()
    {
        FollowCameraStatic = FollowCamera.GetComponent<CinemachineVirtualCamera>();
        BackgroundParentStatic = BackgroundParent;
        BackgroundPrefabStatic = BackgroundPrefab;
        PlayerParentStatic = PlayerParent;
        PlayerPrefabStatic = PlayerPrefab;
        EnemyParentStatic = EnemyParent;
        EnemyPrefabStatic = EnemyPrefab;
        ProjectileParentStatic = ProjectileParent;
        ProjectilePrefabStatic = ProjectilePrefab;

        instance.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        instance.Update();
    }

    // Fixed Update is called a fixed number of times per second
    public void FixedUpdate()
    {
        instance.FixedUpdate();
    }
}
