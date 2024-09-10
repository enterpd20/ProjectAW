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
        min_BattleRange = GetComponentInChildren<CapsuleCollider>();    // 이거 두개는 교전 상태 스크립트에도 똑같이 적용할 것
        MAX_BattleRange = GetComponentInChildren<CapsuleCollider>();    // 이거 두개는 교전 상태 스크립트에도 똑같이 적용할 것

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

        if(enemyInSight.Length == 0)    // 적이 시야 범위 내에 없을 경우
        {
            RandomPositioning();        // 시야 범위 내의 무작위 위치 이동
        }
    }

    private void RandomPositioning()    // 시야 범위 내의 무작위 위치 이동 메서드
    {
        Vector3 randomDirection = Random.insideUnitSphere * CharaSight.radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomDirection, out hit, CharaSight.radius, 1))
        {
            Vector3 finalPosition = hit.position;

            navMeshAgent.SetDestination(finalPosition);

            Vector3 direction = finalPosition - transform.position;     // 이동 방향에 따라 스프라이트 뒤집기

            if(direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if(direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            animator.SetBool("move", true); // Move 애니메이션 재생
        }
    }

    public void ExitState()
    {
        animator.SetBool("normal", false);
    }
}
