using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;

public class IDleState : MonoBehaviour, IState
{
    public SkeletonAnimation skeletonAnimation;
    public Animator animator;
    public string currentState;

    private NavMeshAgent navMeshAgent;

    private CapsuleCollider CharacterSight, min_BattleRange, MAX_BattleRange;

    private LayerMask enemyLayer;   

    public void EnterState()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();

        navMeshAgent = GetComponent<NavMeshAgent>();

        CharacterSight = GetComponentInChildren<CapsuleCollider>();
        min_BattleRange = GetComponentInChildren<CapsuleCollider>();    // �̰� �ΰ��� ���� ���� ��ũ��Ʈ���� �Ȱ��� ������ ��
        MAX_BattleRange = GetComponentInChildren<CapsuleCollider>();    // �̰� �ΰ��� ���� ���� ��ũ��Ʈ���� �Ȱ��� ������ ��

        animator.SetBool("normal", true);
    }

    public void UpdateState()
    {
        Collider[] enemyInSight = 
            Physics.OverlapCapsule(
                CharacterSight.bounds.center, 
                CharacterSight.bounds.center + CharacterSight.height * CharacterSight.transform.up, 
                CharacterSight.radius, 
                enemyLayer);

        if(enemyInSight.Length == 0)    // ���� �þ� ���� ���� ���� ���
        {
            RandomPositioning();        // �þ� ���� ���� ������ ��ġ �̵�
        }
    }

    private void RandomPositioning()    // �þ� ���� ���� ������ ��ġ �̵� �޼���
    {
        Vector3 randomDirection = Random.insideUnitSphere * CharacterSight.radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomDirection, out hit, CharacterSight.radius, 1))
        {
            Vector3 finalPosition = hit.position;

            navMeshAgent.SetDestination(finalPosition);

            Vector3 direction = finalPosition - transform.position;     // �̵� ���⿡ ���� ��������Ʈ ������

            if(direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);            // �̵� ���⿡ ���� ��������Ʈ ������
            }
            else if(direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);           // �̵� ���⿡ ���� ��������Ʈ ������
            }

            animator.SetBool("move", true); // Move �ִϸ��̼� ���
        }
    }

    public void ExitState()
    {
        animator.SetBool("normal", false);
    }
}
