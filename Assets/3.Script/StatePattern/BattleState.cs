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

    // �� �����Ӹ��� ȣ��
    // ���°� Ȱ�� ���� �� ����Ǵ� ����
    public void UpdateState()
    {

    }

    // ���°� ����� �� ȣ��
    // ���¸� �������� �� �ʿ��� ���� �۾��� ����
    public void ExitState()
    {

    }
}
