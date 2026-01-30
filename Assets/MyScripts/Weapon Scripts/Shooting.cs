using UnityEngine;
using UnityEngine.UI;

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

    [Header("Damage Settings")]
    [SerializeField] private float damagePerShot = 25f;

    [Header("Aim Icon (Crosshair)")]
    [Tooltip("UI Image that follows the raycast hit point on screen.")]
    [SerializeField] public Image aimIcon;
    [SerializeField] private Camera mainCamera;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (aimIcon != null)
            aimIcon.enabled = true;
    }

    private void Update()
    {
        DrawShootingRay();
        //UpdateAimIcon();

        if (Input.GetMouseButtonDown(0))
        {
            ShootRay();
        }
    }

    private void DrawShootingRay()
    {
        if (firePoint == null) return;
        Debug.DrawRay(firePoint.position, firePoint.forward * rayDistance, Color.red);
    }

    private void ShootRay()
    {
        if (firePoint == null)
        {
            Debug.LogWarning("FirePoint not assigned!");
            return;
        }

        Ray ray = new Ray(firePoint.position, firePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, hitLayers))
        {
            Debug.Log($"Hit {hit.collider.name} at distance {hit.distance}");

            // Apply damage if target has health
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(damagePerShot);
        }
        else
        {
            Debug.Log("No hit detected.");
        }
    }

   /* private void UpdateAimIcon()
{
    if (aimIcon == null || mainCamera == null || firePoint == null)
        return;

    Ray ray = new Ray(firePoint.position, firePoint.forward);
    Vector3 worldPoint;

    // Try raycast first
    if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, hitLayers))
    {
        worldPoint = hit.point;
    }
    else
    {
        // Default to a point directly forward in space (so it always shows)
        worldPoint = firePoint.position + firePoint.forward * rayDistance;
    }

    // Convert to screen position
    Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPoint);

    // If it's behind the camera, project it anyway (don't disable)
    if (screenPos.z < 0)
    {
        // Flip behind camera to avoid invisibility
        screenPos *= -1f;
    }

    aimIcon.enabled = true;

    // Smooth movement for polish
    aimIcon.rectTransform.position = Vector3.Lerp(
        aimIcon.rectTransform.position,
        screenPos,
        Time.deltaTime * 15f
    );

    // Change color when aiming at enemy
if (Physics.Raycast(ray, out RaycastHit hitInfo, rayDistance, hitLayers))
{
    if (hitInfo.collider.GetComponent<EnemyHealth>() != null)
        aimIcon.color = Color.red;
    else
        aimIcon.color = Color.white;
}
else
{
    aimIcon.color = Color.white;
}

}
*/

}
