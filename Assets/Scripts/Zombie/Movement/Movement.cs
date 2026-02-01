using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private string targetTag = "Player";
    
    private Transform target;
    
    private void Update()
    {
        if (target != null)
        {
            ChaseTarget();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && target == null)
        {
            target = other.transform;
        }
    }

        private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag) && target == other.transform)
        {
            target = null;
        }
    }
    
    private void ChaseTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;
        transform.LookAt(target);
    }
}
