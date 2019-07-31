using UnityEngine;

// Rotates model ships
public class ShipModelRotate : MonoBehaviour
{
    // Editor fields
    [SerializeField, Range(0, 10)]
    public float RotationSpeed = 1;


    // Update is called once per frame
    private void Update()
    {
        this.gameObject.transform.Rotate(this.RotationSpeed, this.RotationSpeed, this.RotationSpeed);
    }
}
