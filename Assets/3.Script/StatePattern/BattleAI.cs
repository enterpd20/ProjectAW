using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleAI : MonoBehaviour
{
    // 캐릭터의 상태 정의
    enum State
    {
        Patrol,       // 대기 상태
        Engage,     // 전투 상태
        Retreat     // 후퇴 상태
    }

    private State currentState = State.Patrol;   // 초기 상태는 대기 상태로 설정

    private Transform target;                    // 타겟(적) 위치
    public float retreatHealthThreshold = 30f;  // 후퇴할 체력 임계값

    private float currentHealth;    // 현재 체력 (Dock UI에서 최종 스탯을 받아올 것)
    private float maxHealth;        // 최대 체력 (Dock UI에서 최종 스탯을 받아올 것)
    private float moveSpeed;         // 이동 속도 (Dock UI에서 최종 스탯을 받아올 것)

    public float attackCooldown = 2f; // 공격 쿨타임
    private float attackCooldownTimer = 0f; // 공격 타이머

    // 캐릭터 시야 범위, 최소 교전범위, 최대 교전범위
    public SphereCollider characterSight, min_EngageRange, MAX_EngageRange;
    private NavMeshAgent AutoBattleAgent;

    void Start()
    {
        currentState = State.Patrol;  // 초기 상태 설정
        AutoBattleAgent = GetComponent<NavMeshAgent>();

        CharacterStats battleStats = Player.Instance.finalCharacterStats;

        if(battleStats != null)
        {
            currentHealth = battleStats.HP;
            moveSpeed = battleStats.SPD;
        }
        else
        {
            Debug.LogWarning("Final characters stats not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                HandlePatrolState();
                break;
            case State.Engage:
                HandleEngageState();
                break;
            case State.Retreat:
                HandleRetreatState();
                break;
        }
    }

    // 정찰 상태 처리
    void HandlePatrolState()    // [[[[[시야범위 내에 적이 있다면, 적이 최대 교전범위 내에 들어올 때까지 추격 -> 구현돼있는지 확인]]]]]
    {
        ScanEnemy();            // 시야 범위 내의 적 스캔 메서드

        if (target == null)     // 일정 시간 후 이동
        {
            RandomPositioning();
        }
        else
        {
            currentState = State.Engage;
        }
    }

    void ScanEnemy()    // 시야 범위 내의 적 스캔 메서드
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, characterSight.radius);
        foreach (var hitCol in hitColliders)
        {
            TeamManager charaTeam = hitCol.GetComponent<TeamManager>();
            if (charaTeam != null && charaTeam.team == TeamManager.Team.Enemy)
            {
                target = hitCol.transform;
                break;
            }
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

    // 전투 상태 처리
    void HandleEngageState()
    {
        attackCooldownTimer += Time.deltaTime;          // [[[[[공격 쿨타임 -> 캐릭터가 장착한 장비의 RLD 받아올 것]]]]]

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > MAX_EngageRange.radius)
            {
                currentState = State.Patrol;    // 최대 교전 범위를 벗어나면 정찰 상태로 전환
                return;
            }
            else if (distanceToTarget < MAX_EngageRange.radius || distanceToTarget > min_EngageRange.radius)
            {
                MovingInEngageRange();
            }

            if (currentHealth <= retreatHealthThreshold)
            {
                currentState = State.Retreat;
            }
        }
        else
        {
            currentState = State.Patrol;
        }
    }

    void MovingInEngageRange()  // 교전 범위 내 무작위 위치로 이동
    {
        Vector3 randomDirection = Random.insideUnitSphere * MAX_EngageRange.radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, MAX_EngageRange.radius, 1))
        {
            Vector3 finalPosition = hit.position;
            AutoBattleAgent.SetDestination(finalPosition);
        }
    }

    // 후퇴 상태 처리
    void HandleRetreatState()
    {
        if(target != null)
        {
            // 적과 반대 방향으로 이동
            RetreatFromTarget();

            // 후퇴 조건이 사라지면 정찰 상태로 전환
            if (!RetreatConditions())
            {
                currentState = State.Patrol;
            }
        }
        else
        {
            currentState = State.Patrol;
        }
    }

    // 적으로부터 후퇴하는 함수
    void RetreatFromTarget()
    {
        Vector3 direction = (transform.position - target.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    bool RetreatConditions()
    {
        if (target == null) return false;

        // 1. 체력이 35% 미만인 경우
        if (currentHealth < maxHealth * 0.35f) return true;

        // 2. 체력이 40% 미만이고, 전체 아군의 수가 전체 적군의 수보다 2명 이상 적은 경우
        if (currentHealth < maxHealth * 0.4f /*&& GetAllyCount() + 2 < GetEnemyCount()*/) return true;

        // 3. 체력이 50% 미만이고 적이 우위 상성에 있는 경우
        // 4. 캐릭터 자신의 함종이 항공모함(CV)인 경우

        return false;
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

    // 이동 상태 처리
    //void HandleMoveState()
    //{
    //    if (target == null)
    //    {
    //        currentState = State.Patrol;
    //        return;
    //    }
    //
    //    // 적과의 거리를 확인하고 가까워질 때까지 이동
    //    float distance = Vector3.Distance(transform.position, target.position);
    //    if (distance > engageRange)
    //    {
    //        MoveTowardsTarget();
    //    }
    //    else
    //    {
    //        currentState = State.Engage; // 적과 가까워지면 전투 상태로 전환
    //    }
    //}

    // 적을 향해 이동
    //void MoveTowardsTarget()
    //{
    //    Vector3 direction = (target.position - transform.position).normalized;
    //    transform.position += direction * moveSpeed * Time.deltaTime;
    //}    
}

public class TeamManager : MonoBehaviour
{
    public enum Team
    {
        Ally,
        Enemy
    }

    public Team team;
}