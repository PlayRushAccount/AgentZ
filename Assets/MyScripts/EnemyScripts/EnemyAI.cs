using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    [Header("State")]
    public State currentState;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;
    public Transform[] patrolPoints;

    [Header("Ranges")]
    public float detectionRange = 10f;
    public float attackRange = 2f;

    [Header("Timers")]
    public float idleTime = 2f;
    public float attackCooldown = 1.5f;

    private int patrolIndex;
    private float idleTimer;
    private float attackTimer;

    // ---------------- UNITY ----------------

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // IMPORTANT: prevent NavMesh from rotating the enemy
        agent.updateRotation = false;
        agent.stoppingDistance = attackRange - 0.1f;

        currentState = State.Idle;
        ResetAnimations();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                IdleState(distanceToPlayer);
                break;

            case State.Patrol:
                PatrolState(distanceToPlayer);
                break;

            case State.Chase:
                ChaseState(distanceToPlayer);
                break;

            case State.Attack:
                AttackState(distanceToPlayer);
                break;
        }
    }

    // ---------------- STATES ----------------

    void IdleState(float distance)
    {
        StopMovement();
        ResetAnimations();

        idleTimer += Time.deltaTime;

        if (distance <= detectionRange)
        {
            ChangeState(State.Chase);
            return;
        }

        if (idleTimer >= idleTime)
        {
            idleTimer = 0f;
            ChangeState(State.Patrol);
        }
    }

    void PatrolState(float distance)
    {
        if (distance <= detectionRange)
        {
            ChangeState(State.Chase);
            return;
        }

        agent.isStopped = false;
        agent.speed = 1f;
        agent.SetDestination(patrolPoints[patrolIndex].position);
        RotateTowards(agent.desiredVelocity);

        animator.SetBool("IsWalking", true);
        animator.SetBool("IsRunning", false);

        if (!agent.pathPending && agent.remainingDistance <= 0.3f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            ChangeState(State.Idle);
        }
    }

    void ChaseState(float distance)
    {
        if (distance > detectionRange)
        {
            ChangeState(State.Patrol);
            return;
        }

        if (distance <= attackRange)
        {
            ChangeState(State.Attack);
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);

        RotateTowards(player.position - transform.position);

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", true);
    }

    void AttackState(float distance)
    {
        StopMovement();
        RotateTowards(player.position - transform.position);

        ResetAnimations();

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            animator.SetTrigger("Attack");

            // DAMAGE LOGIC HERE
            Debug.Log("Enemy attacks player");
        }

        if (distance > attackRange)
        {
            ChangeState(State.Chase);
        }
    }

    // ---------------- HELPERS ----------------

    void ChangeState(State newState)
    {
        currentState = newState;
    }

    void StopMovement()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    void ResetAnimations()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
    }

    void RotateTowards(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * 8f
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
