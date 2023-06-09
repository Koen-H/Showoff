using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PunchAttack : EnemyAttackBehaviour
{

    [SerializeField, Tooltip("From what distance should we try the attack?")] 
    protected float attackRange = 2f;
    private Animator attackAnim;

    [SerializeField] private List<AttackCollider> attackColliders;

    private List<Transform> hits = new List<Transform>();

    protected override void Awake()
    {
        base.Awake();
        
        foreach (AttackCollider atCol in attackColliders)
        {
            atCol.OnTriggerEnterEvent += OnAttackColliderEnter;
        }
        ToggleColliders(false);
    }
    private void OnDisable()
    {
        foreach (AttackCollider atCol in attackColliders)
        {
            atCol.OnTriggerEnterEvent -= OnAttackColliderEnter;
        }
    }

    public override void TryAttack()
    {
        PlayerCharacterController currentTarget = enemy.CurrentTarget;
        if (currentTarget == null) return;
        Vector3 toTarget = currentTarget.transform.position - transform.position;
        float dotProduct = Vector3.Dot(toTarget.normalized, transform.forward);
        if (dotProduct < 0) return;
        if ((currentTarget.transform.position - transform.position).magnitude < attackRange) Attack();
        return;
    }

    protected override void Attack()
    {
        Attacked();
        ToggleColliders(true);

        //Play the punch attack animation
        if (enemy.IsOwner) enemy.enemyAnimationState.Value = EnemyAnimationState.SWORDSLASH;

        /*  attackAnim = GetComponentInChildren<Animator>();
          attackAnim.SetTrigger("Animate");*/

        float attackAnimDuration = 0.917f;//Set the duration of the animation manually as we can't get it from the animator.
        StartCoroutine(AfterAttackAnim(attackAnimDuration));
    }

    void OnAttackColliderEnter(Transform enteredTransform)
    {
        if (hits.Contains(enteredTransform)) return;
        hits.Add(enteredTransform);
        if (enteredTransform.TryGetComponent(out PlayerCharacterController player))
        {
            player.TakeDamage(damage);
            enemy.soundManager.enemyAttack.Play();
        }
    }

    protected override void AfterAttack()
    {
        base.AfterAttack();
        ToggleColliders(false);
        hits.Clear();
    }

    void ToggleColliders(bool enabled)
    {
        foreach (AttackCollider atCol in attackColliders)
        {
           Collider[] colliders = atCol.GetComponents<Collider>();
            foreach(Collider collider in colliders) collider.enabled = enabled;
        }
    }
}
