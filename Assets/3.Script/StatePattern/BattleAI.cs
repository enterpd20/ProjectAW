using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleAI : MonoBehaviour
{
    // ĳ������ ���� ����
    enum State
    {
        Patrol,       // ��� ����
        Engage,     // ���� ����
        Retreat     // ���� ����
    }

    private State currentState = State.Patrol;   // �ʱ� ���´� ��� ���·� ����

    private Transform target;                    // Ÿ��(��) ��ġ
    public float retreatHealthThreshold = 30f;  // ������ ü�� �Ӱ谪

    private float currentHealth;    // ���� ü�� (Dock UI���� ���� ������ �޾ƿ� ��)
    private float maxHealth;        // �ִ� ü�� (Dock UI���� ���� ������ �޾ƿ� ��)
    private float moveSpeed;         // �̵� �ӵ� (Dock UI���� ���� ������ �޾ƿ� ��)

    public float attackCooldown = 2f; // ���� ��Ÿ��
    private float attackCooldownTimer = 0f; // ���� Ÿ�̸�

    // ĳ���� �þ� ����, �ּ� ��������, �ִ� ��������
    public SphereCollider characterSight, min_EngageRange, MAX_EngageRange;
    private NavMeshAgent AutoBattleAgent;

    void Start()
    {
        currentState = State.Patrol;  // �ʱ� ���� ����
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

    // ���� ���� ó��
    void HandlePatrolState()    // [[[[[�þ߹��� ���� ���� �ִٸ�, ���� �ִ� �������� ���� ���� ������ �߰� -> �������ִ��� Ȯ��]]]]]
    {
        ScanEnemy();            // �þ� ���� ���� �� ��ĵ �޼���

        if (target == null)     // ���� �ð� �� �̵�
        {
            RandomPositioning();
        }
        else
        {
            currentState = State.Engage;
        }
    }

    void ScanEnemy()    // �þ� ���� ���� �� ��ĵ �޼���
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

            Vector3 direction = finalPosition - transform.position;     // �̵� ���⿡ ���� ��������Ʈ ������

            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);            // �̵� ���⿡ ���� ��������Ʈ ������
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);           // �̵� ���⿡ ���� ��������Ʈ ������
            }

            //animator.SetBool("move", true);  - �ִϸ��̼� ���߿�
        }
    }

    // ���� ���� ó��
    void HandleEngageState()
    {
        attackCooldownTimer += Time.deltaTime;          // [[[[[���� ��Ÿ�� -> ĳ���Ͱ� ������ ����� RLD �޾ƿ� ��]]]]]

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > MAX_EngageRange.radius)
            {
                currentState = State.Patrol;    // �ִ� ���� ������ ����� ���� ���·� ��ȯ
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

    void MovingInEngageRange()  // ���� ���� �� ������ ��ġ�� �̵�
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

    // ���� ���� ó��
    void HandleRetreatState()
    {
        if(target != null)
        {
            // ���� �ݴ� �������� �̵�
            RetreatFromTarget();

            // ���� ������ ������� ���� ���·� ��ȯ
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

    // �����κ��� �����ϴ� �Լ�
    void RetreatFromTarget()
    {
        Vector3 direction = (transform.position - target.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    bool RetreatConditions()
    {
        if (target == null) return false;

        // 1. ü���� 35% �̸��� ���
        if (currentHealth < maxHealth * 0.35f) return true;

        // 2. ü���� 40% �̸��̰�, ��ü �Ʊ��� ���� ��ü ������ ������ 2�� �̻� ���� ���
        if (currentHealth < maxHealth * 0.4f /*&& GetAllyCount() + 2 < GetEnemyCount()*/) return true;

        // 3. ü���� 50% �̸��̰� ���� ���� �󼺿� �ִ� ���
        // 4. ĳ���� �ڽ��� ������ �װ�����(CV)�� ���

        return false;
    }

    // ���� �����ϴ� �Լ�
    void Attack()
    {
        Debug.Log("Attack target!");
        // ���� ���� ���� ���� �ʿ�
    }

    // ü�� ���� ó�� (���� �޾��� ��)
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // ��� ó�� ���� �߰� �ʿ�
        }
    }

    // �̵� ���� ó��
    //void HandleMoveState()
    //{
    //    if (target == null)
    //    {
    //        currentState = State.Patrol;
    //        return;
    //    }
    //
    //    // ������ �Ÿ��� Ȯ���ϰ� ������� ������ �̵�
    //    float distance = Vector3.Distance(transform.position, target.position);
    //    if (distance > engageRange)
    //    {
    //        MoveTowardsTarget();
    //    }
    //    else
    //    {
    //        currentState = State.Engage; // ���� ��������� ���� ���·� ��ȯ
    //    }
    //}

    // ���� ���� �̵�
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