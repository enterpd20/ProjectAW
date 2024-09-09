using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;

public class MoveState : MonoBehaviour, IState
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private CapsuleCollider CharaSight, min_BattleRange, MAX_BattleRange;

    private LayerMask enemyLayer;
    private Transform currentEnemy;

    public void EnterState()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        CharaSight = GetComponentInChildren<CapsuleCollider>();
        min_BattleRange = GetComponentInChildren<CapsuleCollider>();
        MAX_BattleRange = GetComponentInChildren<CapsuleCollider>();
    }

    // �� �����Ӹ��� ȣ��
    // ���°� Ȱ�� ���� �� ����Ǵ� ����
    public void UpdateState()
    {
        Collider[] enemyInSight =
            Physics.OverlapCapsule(
                CharaSight.bounds.center,
                CharaSight.bounds.center + CharaSight.height * CharaSight.transform.up,
                CharaSight.radius,
                enemyLayer);

        if (enemyInSight.Length > 0)                    // ���� �þ� ���� ���� ���� ���
        {
            currentEnemy = enemyInSight[0].transform;   // ���� ����� ���� Ÿ������ ����
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
        if(currentEnemy != null)
        {
            Vector3 directionToPosition = targetPosition - transform.position;      // �̵��� ��ġ�� ���� ���
            Vector3 directionToEnemy = currentEnemy.position - transform.position;  // ���� �ִ� ���� ���

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
