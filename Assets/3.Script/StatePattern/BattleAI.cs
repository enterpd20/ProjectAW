using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleAI : MonoBehaviour
{
    // ĳ������ ���� ����
    enum State
    {
        Patrol,       // ��� ����
        Move,       // �̵� ����
        Engage,     // ���� ����
        Retreat     // ���� ����
    }

    private State currentState = State.Patrol;   // �ʱ� ���´� ��� ���·� ����

    public Transform target;           // Ÿ��(��) ��ġ
    public float engageRange;                   // ������ ���� ���� (�����ص� �ݶ��̴� ����ͼ� ����)
    public float retreatHealthThreshold = 30f;  // ������ ü�� �Ӱ谪

    private float currentHealth;    // ���� ü�� (Dock UI���� ���� ������ �޾ƿ� ��)
    private float maxHealth;        // �ִ� ü�� (Dock UI���� ���� ������ �޾ƿ� ��)
    public float moveSpeed;         // �̵� �ӵ� (Dock UI���� ���� ������ �޾ƿ� ��)
    
    public float attackCooldown = 2f; // ���� ��Ÿ��
    private float attackCooldownTimer = 0f; // ���� Ÿ�̸�

    // ��� ���¿��� ���� �ð� ������ Idle ���·� ���ư����� ����
    private float patrolTimer = 0f;
    private float patrolTimeLimit = 5f;

    // ĳ���� �þ� ����, �ּ� ��������, �ִ� ��������
    public SphereCollider characterSight, min_EngageRange, MAX_EngageRange;
    private NavMeshAgent AutoBattleAgent;

    void Start()
    {
        currentState = State.Patrol;  // �ʱ� ���� ����
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

    // ��� ���� ó��
    void HandlePatrolState()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer > patrolTimeLimit)   // ���� �ð� �� �̵�
        {
            RandomPositioning();
            patrolTimer = 0f;
        }

        // ������ �Ÿ� Ȯ�� -> [[[[[[[[[[[[[[[[[[[[[[[[[�̰� �ʿ���?? Ȯ����]]]]]]]]]]]]]]]]]]]]]
        if (target != null && Vector3.Distance(transform.position, target.position) < engageRange)
        {
            currentState = State.Engage; // ���� ������ ���� ���·� ��ȯ
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

    // �̵� ���� ó��
    void HandleMoveState()
    {
        if (target == null)
        {
            currentState = State.Patrol;
            return;
        }

        // ������ �Ÿ��� Ȯ���ϰ� ������� ������ �̵�
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > engageRange)
        {
            MoveTowardsTarget();
        }
        else
        {
            currentState = State.Engage; // ���� ��������� ���� ���·� ��ȯ
        }
    }

    // ���� ���� ó��
    void HandleEngageState()
    {
        attackCooldownTimer += Time.deltaTime;

        // ���� ��Ÿ���� �������� ����
        if (attackCooldownTimer >= attackCooldown)
        {
            Attack();
            attackCooldownTimer = 0f;  // ��Ÿ�� �ʱ�ȭ
        }

        // ü���� ���� ���Ϸ� �������� ���� ���·� ��ȯ
        if (currentHealth <= retreatHealthThreshold)
        {
            currentState = State.Retreat;
        }
    }

    // ���� ���� ó��
    void HandleRetreatState()
    {
        // ���� �ݴ� �������� �̵�
        RetreatFromTarget();

        // ü���� ȸ���Ǹ� �ٽ� �̵� ���·� ��ȯ
        if (currentHealth > retreatHealthThreshold)
        {
            currentState = State.Move;
        }
    }

    // �����κ��� �����ϴ� �Լ�
    void RetreatFromTarget()
    {
        Vector3 direction = (transform.position - target.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    // ���� ���� �̵�
    void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
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
}