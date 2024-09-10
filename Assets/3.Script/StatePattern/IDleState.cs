using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;

public class IDleState : MonoBehaviour, IState
{
    private SkeletonAnimation skeletonAnimation;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private CapsuleCollider CharaSight, min_BattleRange, MAX_BattleRange;

    private LayerMask enemyLayer;   

    public void EnterState()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();

        navMeshAgent = GetComponent<NavMeshAgent>();

        CharaSight = GetComponentInChildren<CapsuleCollider>();
        min_BattleRange = GetComponentInChildren<CapsuleCollider>();    // �̰� �ΰ��� ���� ���� ��ũ��Ʈ���� �Ȱ��� ������ ��
        MAX_BattleRange = GetComponentInChildren<CapsuleCollider>();    // �̰� �ΰ��� ���� ���� ��ũ��Ʈ���� �Ȱ��� ������ ��

        animator.SetBool("normal", true);
    }

    public void UpdateState()
    {
        Collider[] enemyInSight = 
            Physics.OverlapCapsule(
                CharaSight.bounds.center, 
                CharaSight.bounds.center + CharaSight.height * CharaSight.transform.up, 
                CharaSight.radius, 
                enemyLayer);

        if(enemyInSight.Length == 0)    // ���� �þ� ���� ���� ���� ���
        {
            RandomPositioning();        // �þ� ���� ���� ������ ��ġ �̵�
        }
    }

    private void RandomPositioning()    // �þ� ���� ���� ������ ��ġ �̵� �޼���
    {
        Vector3 randomDirection = Random.insideUnitSphere * CharaSight.radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomDirection, out hit, CharaSight.radius, 1))
        {
            Vector3 finalPosition = hit.position;

            navMeshAgent.SetDestination(finalPosition);

            Vector3 direction = finalPosition - transform.position;     // �̵� ���⿡ ���� ��������Ʈ ������

            if(direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if(direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            animator.SetBool("move", true); // Move �ִϸ��̼� ���
        }
    }

    public void ExitState()
    {
        animator.SetBool("normal", false);
    }
}
