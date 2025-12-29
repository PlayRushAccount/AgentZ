using UnityEngine;

public class ZombieWander : MonoBehaviour
{
   
    [SerializeField] private float wanderSpeed = 1f;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float waitTimeMin = 2f;
    [SerializeField] private float waitTimeMax = 5f;
    
    private Vector3 targetPosition;
    private float waitTimer;
    private bool isWaiting = true;
    
    private void Start()
    {
        SetRandomWaitTime();
    }
    
    private void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                ChooseNewTarget();
                isWaiting = false;
            }
        }
        else
        {
            WanderToTarget();
        }
    }
    
    private void ChooseNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }
    
    private void WanderToTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * wanderSpeed * Time.deltaTime;
        
        if (direction != Vector3.zero)
        {
            transform.LookAt(targetPosition);
        }
        
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            isWaiting = true;
            SetRandomWaitTime();
        }
    }
    
    private void SetRandomWaitTime()
    {
        waitTimer = Random.Range(waitTimeMin, waitTimeMax);
    }
}
