using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
        GameController.Collide(this.gameObject, collider.gameObject);
    }
}
