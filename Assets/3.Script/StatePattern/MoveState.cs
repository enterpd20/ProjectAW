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

    // 매 프레임마다 호출
    // 상태가 활성 중일 때 실행되는 로직
    public void UpdateState()
    {
        Collider[] enemyInSight =
            Physics.OverlapCapsule(
                CharaSight.bounds.center,
                CharaSight.bounds.center + CharaSight.height * CharaSight.transform.up,
                CharaSight.radius,
                enemyLayer);

        if (enemyInSight.Length > 0)                    // 적이 시야 범위 내에 있을 경우
        {
            currentEnemy = enemyInSight[0].transform;   // 가장 가까운 적을 타겟으로 설정
            RandomPositioning_Battle();                 // 교전 거리 범위 내의 무작위 위치 이동
        }
    }

    private void RandomPositioning_Battle()
    {
        Vector3 randomPosition;

        do
        {
            // MAX_BattleRange 안쪽 범위에서 무작위 위치 지정
            Vector3 randomDirection = Random.insideUnitSphere * MAX_BattleRange.radius; 
            randomDirection += transform.position;

            NavMeshHit navHit;  // NavMesh 상에서 유효한 위치인지 확인
            if (NavMesh.SamplePosition(randomDirection, out navHit, MAX_BattleRange.radius, NavMesh.AllAreas))
            {
                randomPosition = navHit.position;

                // 무작위 위치가 min_BattleRange 바깥쪽에 있는지 확인
                if (Vector3.Distance(transform.position, randomPosition) >= min_BattleRange.radius)
                {
                    MoveAnimation(randomPosition);
                    navMeshAgent.SetDestination(randomPosition);    // 이동 지점 설정
                    break;
                }
            }
        } while (true);
    }

    private void MoveAnimation(Vector3 targetPosition)
    {
        if(currentEnemy != null)
        {
            Vector3 directionToPosition = targetPosition - transform.position;      // 이동할 위치의 방향 계산
            Vector3 directionToEnemy = currentEnemy.position - transform.position;  // 적이 있는 방향 계산

            float angle = Vector3.Angle(directionToPosition, directionToEnemy);     // 두 벡터 사이의 각도 계산

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

    // 상태가 종료될 때 호출
    // 상태를 빠져나올 때 필요한 정리 작업을 수행
    public void ExitState()
    {

    }
}
