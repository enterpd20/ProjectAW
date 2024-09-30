using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/*
ĳ���Ͱ� �������ϴ� ����
ü�� - ���⼭ ��
�̵��ӵ� - ���⼭ ��
���ݷ� - BulletControl���� ��� ������ �޾ƿͼ� ��
������ - BulletControl���� ��� ������ �޾ƿͼ� ��
*/

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
    private float attackCooldown; // ���� ��Ÿ��

    private float attackCooldownTimer = 0f; // ���� Ÿ�̸�

    // ĳ���� �þ� ����, �ּ� ��������, �ִ� ��������
    public SphereCollider characterSight, min_EngageRange, MAX_EngageRange;
    private bool isMoving = false;
    //private NavMeshAgent AutoBattleAgent;

    public GameObject mapObject;
    private Vector3 mapMinBounds;
    private Vector3 mapMaxBounds;

    void Start()
    {
        currentState = State.Patrol;  // �ʱ� ���� ����
        //AutoBattleAgent = GetComponent<NavMeshAgent>();

        mapObject = FindObjectOfType<MeshCollider>().gameObject;    // �� ������Ʈ ��� �� ������

        // �� ������Ʈ�� ��� ���� ������
        if (mapObject != null)
        {
            Renderer mapRenderer = mapObject.GetComponent<Renderer>();
            if (mapRenderer != null)
            {
                mapMinBounds = mapRenderer.bounds.min;
                mapMaxBounds = mapRenderer.bounds.max;
            }
            else
            {
                Debug.LogError("Map object does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogError("Map object is not assigned.");
        }

        Debug.Log("Trying to get finalCharacterStats from Player.Instance...");

        if (Player.Instance == null)
        {
            Debug.LogError("Player.Instance is null!");
            return;
        }

        int selectedIndex = Player.Instance.selectedCharacterIndex;
        if(selectedIndex >= 0 && selectedIndex < Player.Instance.ownedCharacter.Count)
        {
            CharacterStats battleStats = Player.Instance.ownedCharacter[selectedIndex].stats;

            maxHealth = battleStats.HP;
            moveSpeed = battleStats.SPD;
            Debug.Log($"Character Stats: HP = {battleStats.HP}, SPD = {battleStats.SPD}");
        }
        else
        {
            Debug.LogWarning("Selected character index is out of range or invalid.");
        }

    }

    public void InitializeCharacterStats(CharacterStats stats)
    {       
        maxHealth = stats.HP;
        moveSpeed = stats.SPD * 0.5f;
        Debug.Log($"Initialized Character Stats: HP = {stats.HP}, SPD = {stats.SPD}");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                HandlePatrolState();
                //Debug.Log($"Patrol State!");
                break;
            case State.Engage:
                HandleEngageState();
                //Debug.Log($"Engage State!");
                break;
            case State.Retreat:
                HandleRetreatState();
                //Debug.Log($"Retreat State!");
                break;
        }
    }

    // ���� ���� ó��
    void HandlePatrolState()    // [[[[[�þ߹��� ���� ���� �ִٸ�, ���� �ִ� �������� ���� ���� ������ �߰� -> �������ִ��� Ȯ��]]]]]
    {
        ScanEnemy();            // �þ� ���� ���� �� ��ĵ �޼���

        if (target == null && !isMoving)     // �̵� ���� �ƴ� ���� ���� ��ġ ����
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
        randomDirection.y = transform.position.y;   // Y ���� ����

        Vector3 finalPosition = randomDirection;        
        Vector3 direction = (finalPosition - transform.position).normalized;    // ���� ��ġ���� ��ǥ ��ġ�� �̵�

        //transform.position += direction * moveSpeed * Time.deltaTime;
        // Rigidbody�� ���� �̵�
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Y ���� �̵��� �����ϱ� ���� Y ���� 0���� ����
            direction.y = 0;
            //rb.velocity = direction * moveSpeed;
            //rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            isMoving = true;  // �̵� ����
            StartCoroutine(MoveToPosition(rb, finalPosition));  // �̵��� �ڷ�ƾ���� ����
        }

        // �̵� ���⿡ ���� ��������Ʈ ������
        if (direction.x > 0)
        {
            transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
        }
    }

    private IEnumerator MoveToPosition(Rigidbody rb, Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0;

            Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            //rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

            // �̵� ���� ����: ���� ��� ���� ����Ͽ� ����
            nextPosition.x = Mathf.Clamp(nextPosition.x, mapMinBounds.x, mapMaxBounds.x);
            nextPosition.z = Mathf.Clamp(nextPosition.z, mapMinBounds.z, mapMaxBounds.z);
            rb.MovePosition(nextPosition);
            yield return null;  // ���� �����ӱ��� ���
        }

        isMoving = false;  // �̵� �Ϸ�
    }

    // �� �����ڸ��� ����� �� ������ ��ġ �ٽ� ����
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mapObject)
        {
            // �̵� ���� �ƴ϶�� ������ ��ġ �ٽ� ����
            if (!isMoving)
            {
                RandomPositioning();
                Debug.Log($"�̵����� �缳��!");
            }
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

        //NavMeshHit hit;
        //if (NavMesh.SamplePosition(randomDirection, out hit, MAX_EngageRange.radius, 1))
        //{
        //    Vector3 finalPosition = hit.position;
        //    AutoBattleAgent.SetDestination(finalPosition);
        //}

        // Y ���� ����
        randomDirection.y = transform.position.y;

        Vector3 finalPosition = randomDirection;

        // ���� ��ġ���� ��ǥ ��ġ�� �̵�
        Vector3 direction = (finalPosition - transform.position).normalized;

        //transform.position += direction * moveSpeed * Time.deltaTime;
        // Rigidbody�� ���� �̵�
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Y ���� �̵��� �����ϱ� ���� Y ���� 0���� ����
            direction.y = 0;
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            //rb.velocity = direction * moveSpeed;
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

        // Y ���� �̵��� �����ϱ� ���� Y ���� 0���� ����
        direction.y = 0;

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