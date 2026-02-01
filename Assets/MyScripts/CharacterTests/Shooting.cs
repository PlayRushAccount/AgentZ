using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Shooting Origin")]
    [Tooltip("Point on the player where the ray originates (e.g., gun muzzle, chest, head)")]
    [SerializeField] private Transform firePoint;

    [Header("Ray Settings")]
    [Tooltip("Maximum range of the ray")]
    [SerializeField] private float rayDistance = 100f;
    [Tooltip("Layer mask for what the ray can hit")]
    [SerializeField] private LayerMask hitLayers;

    [Header("References")]

    public MuzzleFlash muzzleFlashController; 



    private void Update()
    {
        // Draw the ray each frame (for debugging)
        DrawShootingRay();

        // Example trigger: left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            ShootRay();
            muzzleFlashController.PlayMuzzleFlash();
        }
    }

    private void DrawShootingRay()
    {
        if (firePoint == null) return;

        // Draws a red ray in the Scene view to visualize shooting direction
        Debug.DrawRay(firePoint.position, firePoint.forward * rayDistance, Color.red);
    }

    private void ShootRay()
    {
        if (firePoint == null) return;

        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, hitLayers))
        {
            Debug.Log($"Hit {hit.collider.name} at distance {hit.distance}");

            // 🩸 Apply damage if the hit object has EnemyHealth
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(50f); // change this value or make it configurable
            }

            // 💥 Apply impact force if it has a Rigidbody
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(-hit.normal * 1000f, ForceMode.Impulse);
            }
        }
            else
            {
                Debug.Log("No hit detected.");
            }
    }
}
