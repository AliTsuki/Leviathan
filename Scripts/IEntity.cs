using UnityEngine;

// Interface for all entities
public interface IEntity
{
    GameObject ShipObject { get; set; }
    uint ID { get; set; }
    GameController.IFF IFF { get; set; }
    bool Alive { get; set; }
    

    // Start is called before the first frame update
    void Start();

    // Update is called once per frame
    void Update();

    // Fixed Update is called a fixed number of times per second
    void FixedUpdate();

    // Called when receiving collision
    void ReceivedCollision(float _damage);
}
