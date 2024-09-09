using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleState : MonoBehaviour, IState
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private CapsuleCollider CharaSight, min_BattleRange, MAX_BattleRange;

    private LayerMask enemyLayer;

    public void EnterState()
    {

    }

    // 매 프레임마다 호출
    // 상태가 활성 중일 때 실행되는 로직
    public void UpdateState()
    {

    }

    // 상태가 종료될 때 호출
    // 상태를 빠져나올 때 필요한 정리 작업을 수행
    public void ExitState()
    {

    }
}
