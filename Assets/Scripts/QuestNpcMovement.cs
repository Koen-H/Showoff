using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class QuestNpcMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private QuestNPC questNpc;
    private Transform target;


    [SerializeField] bool canWander = true;
    [SerializeField] float wanderDistance = 5f;
    [SerializeField] Range wanderDelay = new Range(0, 1);

    //Nockback things
    [SerializeField] private bool canBeNockedBack = true;

    private bool isKnockedBack = false;
    private Vector3 knockbackDirection;
    private float knockbackForce;
    private float knockbackDuration;
    private float knockbackTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        questNpc = GetComponent<QuestNPC>();
        questNpc.OnTargetChange += OnTargetChange;
        if (canWander) StartCoroutine(Wander());
    }
    private void OnDisable()
    {
        questNpc.OnTargetChange -= OnTargetChange;
    }

    void OnTargetChange(Transform newTarget)
    {
        target = newTarget;
    }


    public void SetSpeed(float speed) { if (speed >= 0) agent.speed = speed; }

    #region Nockback
    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        if (!canBeNockedBack) return;
        ApplyNockbackServerRpc(direction, force, duration);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplyNockbackServerRpc(Vector3 direction, float force, float duration)
    {
        knockbackDirection = direction;
        knockbackForce = force;
        knockbackDuration = duration;
        knockbackTimer = 0f;
        isKnockedBack = true;

    }

    private void Nockback()
    {
        // Apply knockback force
        agent.velocity = knockbackDirection.normalized * knockbackForce;

        // Update the timer and check if knockback duration is over
        knockbackTimer += Time.deltaTime;
        if (knockbackTimer >= knockbackDuration)
        {
            // Knockback duration is over, resume normal movement
            isKnockedBack = false;
            agent.velocity = Vector3.zero;
        }
    }

    #endregion

    private void Update()
    {
        if (isKnockedBack) Nockback();
        else if (questNpc.enemyState != QuestNPCState.WAITING) Move();
    }

    protected virtual void Move()
    {
        //Simple walk towards target
        if (target == null) return;
        agent.SetDestination(target.position);
    }

    /// <summary>
    /// Wanders a random direction and distance.
    /// </summary>
    IEnumerator Wander()
    {
        if (questNpc.enemyState == QuestNPCState.BORED)
        {
            // Generate a random direction
            Vector3 randomDirection = Random.insideUnitSphere;

            // Set the Y component to 0 to ensure the agent walks on a flat surface
            randomDirection.y = 0f;

            // Normalize the direction to maintain consistent movement speed
            randomDirection.Normalize();

            // Set the destination based on the random direction
            Vector3 targetPosition = transform.position + randomDirection * wanderDistance;

            // Set the NavMeshAgent destination
            agent.SetDestination(targetPosition);
        }

        // Resume wandering after a short delay
        yield return new WaitForSeconds(wanderDelay.GetRandomValue());

        // Start wandering again
        StartCoroutine(Wander());
    }

}
