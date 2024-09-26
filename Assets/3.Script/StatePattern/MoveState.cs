using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;

// 240910 이동과 전투의 FSM 스크립트 하나로 합치는게 훨씬 나을수도?
// 사유: 애들이 거의 대부분 이동과 교전이 동시에 이뤄지기 때문...

public class MoveState : MonoBehaviour, IState
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private CapsuleCollider CharacterSight, min_BattleRange, MAX_BattleRange;

    private LayerMask enemyLayer;
    private Transform enemyTransform;

    // 체력이 40% 이하일 경우 퇴각하는 메서드를 위한 HP 변수
    private float MAXHP;
    private float CurrentHP;

    public void EnterState()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        CharacterSight = GetComponentInChildren<CapsuleCollider>();
        min_BattleRange = GetComponentInChildren<CapsuleCollider>();
        MAX_BattleRange = GetComponentInChildren<CapsuleCollider>();
    }

    // 매 프레임마다 호출
    // 상태가 활성 중일 때 실행되는 로직
    public void UpdateState()
    {
        Collider[] enemyInSight =
            Physics.OverlapCapsule(
                CharacterSight.bounds.center,
                CharacterSight.bounds.center + CharacterSight.height * CharacterSight.transform.up,
                CharacterSight.radius,
                enemyLayer);

        if (enemyInSight.Length > 0)                    // 적이 시야 범위 내에 있을 경우
        {
            enemyTransform = enemyInSight[0].transform;   // 가장 가까운 적을 타겟으로 설정
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
        if(enemyTransform != null)
        {
            Vector3 directionToPosition = targetPosition - transform.position;      // 이동할 위치의 방향 계산
            Vector3 directionToEnemy = enemyTransform.position - transform.position;  // 적이 있는 방향 계산

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
