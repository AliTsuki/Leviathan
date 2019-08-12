using UnityEngine;

// Rotates model ships
public class ShipModelRotate : MonoBehaviour
{
    // Editor fields
    [SerializeField, Range(0, 10)]
    public float RotationSpeed = 1;


    // Fixed Update is called a fixed number of times per second
    private void FixedUpdate()
    {
        this.gameObject.transform.Rotate(this.RotationSpeed, this.RotationSpeed, this.RotationSpeed);
    }
}
