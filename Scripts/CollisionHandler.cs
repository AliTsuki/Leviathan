using UnityEngine;

// Handles Collision information, this should be placed on all entity prefabs in Unity Editor
public class CollisionHandler : MonoBehaviour
{
    // Called on collision and reports to GameController
    public void OnTriggerEnter(Collider collider)
    {
        GameController.Collide(this.gameObject, collider.gameObject);
    }
}
