using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;

// 240910 �̵��� ������ FSM ��ũ��Ʈ �ϳ��� ��ġ�°� �ξ� ��������?
// ����: �ֵ��� ���� ��κ� �̵��� ������ ���ÿ� �̷����� ����...

public class MoveState : MonoBehaviour, IState
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private CapsuleCollider CharacterSight, min_BattleRange, MAX_BattleRange;

    private LayerMask enemyLayer;
    private Transform enemyTransform;

    // ü���� 40% ������ ��� ���ϴ� �޼��带 ���� HP ����
    private float MAXHP;
    private float CurrentHP;

    public void EnterState()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        CharacterSight = GetComponentInChildren<CapsuleCollider>();
        min_BattleRange = GetComponentInChildren<CapsuleCollider>();
        MAX_BattleRange = GetComponentInChildren<CapsuleCollider>();
    }

    // �� �����Ӹ��� ȣ��
    // ���°� Ȱ�� ���� �� ����Ǵ� ����
    public void UpdateState()
    {
        Collider[] enemyInSight =
            Physics.OverlapCapsule(
                CharacterSight.bounds.center,
                CharacterSight.bounds.center + CharacterSight.height * CharacterSight.transform.up,
                CharacterSight.radius,
                enemyLayer);

        if (enemyInSight.Length > 0)                    // ���� �þ� ���� ���� ���� ���
        {
            enemyTransform = enemyInSight[0].transform;   // ���� ����� ���� Ÿ������ ����
            RandomPositioning_Battle();                 // ���� �Ÿ� ���� ���� ������ ��ġ �̵�
        }
    }

    private void RandomPositioning_Battle()
    {
        Vector3 randomPosition;

        do
        {
            // MAX_BattleRange ���� �������� ������ ��ġ ����
            Vector3 randomDirection = Random.insideUnitSphere * MAX_BattleRange.radius; 
            randomDirection += transform.position;

            NavMeshHit navHit;  // NavMesh �󿡼� ��ȿ�� ��ġ���� Ȯ��
            if (NavMesh.SamplePosition(randomDirection, out navHit, MAX_BattleRange.radius, NavMesh.AllAreas))
            {
                randomPosition = navHit.position;

                // ������ ��ġ�� min_BattleRange �ٱ��ʿ� �ִ��� Ȯ��
                if (Vector3.Distance(transform.position, randomPosition) >= min_BattleRange.radius)
                {
                    MoveAnimation(randomPosition);
                    navMeshAgent.SetDestination(randomPosition);    // �̵� ���� ����
                    break;
                }
            }
        } while (true);
    }

    private void MoveAnimation(Vector3 targetPosition)
    {
        if(enemyTransform != null)
        {
            Vector3 directionToPosition = targetPosition - transform.position;      // �̵��� ��ġ�� ���� ���
            Vector3 directionToEnemy = enemyTransform.position - transform.position;  // ���� �ִ� ���� ���

            float angle = Vector3.Angle(directionToPosition, directionToEnemy);     // �� ���� ������ ���� ���

            if(angle < 90f)
            {
                animator.SetBool("move", true);
                animator.SetBool("move_left", false);
            }
            else
            {
                animator.SetBool("move", false);
                animator.SetBool("move_left", true);
            }
        }
    }

    // ���°� ����� �� ȣ��
    // ���¸� �������� �� �ʿ��� ���� �۾��� ����
    public void ExitState()
    {

    }
}
