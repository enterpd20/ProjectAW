using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleAI : MonoBehaviour
{
    // 캐릭터의 상태 정의
    enum State
    {
        Patrol,       // 대기 상태
        Move,       // 이동 상태
        Engage,     // 전투 상태
        Retreat     // 후퇴 상태
    }

    private State currentState = State.Patrol;   // 초기 상태는 대기 상태로 설정

    public Transform target;           // 타겟(적) 위치
    public float engageRange;                   // 적과의 교전 범위 (설정해둔 콜라이더 갖고와서 적용)
    public float retreatHealthThreshold = 30f;  // 후퇴할 체력 임계값

    private float currentHealth;    // 현재 체력 (Dock UI에서 최종 스탯을 받아올 것)
    private float maxHealth;        // 최대 체력 (Dock UI에서 최종 스탯을 받아올 것)
    public float moveSpeed;         // 이동 속도 (Dock UI에서 최종 스탯을 받아올 것)
    
    public float attackCooldown = 2f; // 공격 쿨타임
    private float attackCooldownTimer = 0f; // 공격 타이머

    // 대기 상태에서 일정 시간 지나면 Idle 상태로 돌아가도록 설정
    private float patrolTimer = 0f;
    private float patrolTimeLimit = 5f;

    // 캐릭터 시야 범위, 최소 교전범위, 최대 교전범위
    public SphereCollider characterSight, min_EngageRange, MAX_EngageRange;
    private NavMeshAgent AutoBattleAgent;

    void Start()
    {
        currentState = State.Patrol;  // 초기 상태 설정
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                HandlePatrolState();
                break;
            case State.Move:
                HandleMoveState();
                break;
            case State.Engage:
                HandleEngageState();
                break;
            case State.Retreat:
                HandleRetreatState();
                break;
        }
    }

    // 대기 상태 처리
    void HandlePatrolState()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer > patrolTimeLimit)   // 일정 시간 후 이동
        {
            RandomPositioning();
            patrolTimer = 0f;
        }

        // 적과의 거리 확인 -> [[[[[[[[[[[[[[[[[[[[[[[[[이거 필요함?? 확인좀]]]]]]]]]]]]]]]]]]]]]
        if (target != null && Vector3.Distance(transform.position, target.position) < engageRange)
        {
            currentState = State.Engage; // 적이 있으면 전투 상태로 전환
        }
    }

    private void RandomPositioning()
    {
        Vector3 randomDirection = Random.insideUnitSphere * characterSight.radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, characterSight.radius, 1))
        {
            Vector3 finalPosition = hit.position;

            AutoBattleAgent.SetDestination(finalPosition);

            Vector3 direction = finalPosition - transform.position;     // 이동 방향에 따라 스프라이트 뒤집기

            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);            // 이동 방향에 따라 스프라이트 뒤집기
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);           // 이동 방향에 따라 스프라이트 뒤집기
            }

            //animator.SetBool("move", true);  - 애니메이션 나중에
        }
    }

    // 이동 상태 처리
    void HandleMoveState()
    {
        if (target == null)
        {
            currentState = State.Patrol;
            return;
        }

        // 적과의 거리를 확인하고 가까워질 때까지 이동
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > engageRange)
        {
            MoveTowardsTarget();
        }
        else
        {
            currentState = State.Engage; // 적과 가까워지면 전투 상태로 전환
        }
    }

    // 전투 상태 처리
    void HandleEngageState()
    {
        attackCooldownTimer += Time.deltaTime;

        // 공격 쿨타임이 지났으면 공격
        if (attackCooldownTimer >= attackCooldown)
        {
            Attack();
            attackCooldownTimer = 0f;  // 쿨타임 초기화
        }

        // 체력이 일정 이하로 떨어지면 후퇴 상태로 전환
        if (currentHealth <= retreatHealthThreshold)
        {
            currentState = State.Retreat;
        }
    }

    // 후퇴 상태 처리
    void HandleRetreatState()
    {
        // 적과 반대 방향으로 이동
        RetreatFromTarget();

        // 체력이 회복되면 다시 이동 상태로 전환
        if (currentHealth > retreatHealthThreshold)
        {
            currentState = State.Move;
        }
    }

    // 적으로부터 후퇴하는 함수
    void RetreatFromTarget()
    {
        Vector3 direction = (transform.position - target.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    // 적을 향해 이동
    void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    // 적을 공격하는 함수
    void Attack()
    {
        Debug.Log("Attack target!");
        // 실제 공격 로직 구현 필요
    }

    // 체력 감소 처리 (공격 받았을 때)
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // 사망 처리 로직 추가 필요
        }
    }
}