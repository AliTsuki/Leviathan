using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject projectile;
    private Rigidbody projectileRigidbody;
    private float projectileLifeTime = 2.5f;
    private float projectileSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Fixed Update is called a fixed number of times per second
    private void FixedUpdate()
    {
        projectileRigidbody.velocity = projectile.transform.forward * projectileSpeed;
        Destroy(projectile, projectileLifeTime);
    }
}
