using System.Collections;
using UnityEngine;

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
    private float moveSpeed;        // �̵� �ӵ� (Dock UI���� ���� ������ �޾ƿ� ��)

    // ĳ���� �þ� ����, �ּ� ��������, �ִ� ��������
    public SphereCollider characterSight, min_EngageRange, MAX_EngageRange;
    private bool isMoving = false;

    private float attackCooldownTimer = 0f;

    public GameObject mapObject;
    private Vector3 mapMinBounds;
    private Vector3 mapMaxBounds;

    private TeamManager teamManager;

    void Start()
    {
        currentState = State.Patrol;  // �ʱ� ���� ����
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

        teamManager = GetComponent<TeamManager>();

        // ��� ������ �ε� Ȯ��
        Debug.Log($"Gears count in Player: {Player.Instance.gears.Count}");
        foreach (var gear in Player.Instance.gears)
        {
            Debug.Log($"Gear in Player: {gear.name}, Type: {gear.gearType}");
        }
    }

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

    // ĳ���� ���� �ҷ�����
    public void InitializeCharacterStats(CharacterStats stats)
    {
        int selectedIndex = Player.Instance.selectedCharacterIndex;
        if (selectedIndex >= 0 && selectedIndex < Player.Instance.ownedCharacter.Count)
        {
            CharacterStats battleStats = Player.Instance.ownedCharacter[selectedIndex].stats;

            maxHealth = battleStats.HP;
            moveSpeed = battleStats.SPD * 0.5f;
            Debug.Log($"Character Stats: HP = {battleStats.HP}, SPD = {battleStats.SPD}");
        }
        else
        {
            Debug.LogWarning("Selected character index is out of range or invalid.");
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
        float closestDistance = float.MaxValue;
        Transform closestTarget = null;

        foreach (var hitCol in hitColliders)
        {
            //TeamManager charaTeam = hitCol.GetComponent<TeamManager>();
            //if (charaTeam != null && charaTeam.team == TeamManager.Team.Enemy)
            //{
            //    target = hitCol.transform;
            //    break;
            //}
            
            TeamManager otherTeamManager = hitCol.GetComponent<TeamManager>();

            // �ڱ� �ڽ��̳� �� ������ ������ ��ŵ
            if (otherTeamManager == null || otherTeamManager == teamManager) continue;

            // ���� ���� �ĺ�
            if ((teamManager.team == TeamManager.Team.Ally && otherTeamManager.team == TeamManager.Team.Enemy) ||
                (teamManager.team == TeamManager.Team.Enemy && otherTeamManager.team == TeamManager.Team.Ally))
            {
                float distanceToTarget = Vector3.Distance(transform.position, hitCol.transform.position);
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = hitCol.transform;
                }
            }
        }

        target = closestTarget; // ���� ����� ���� Ÿ������ ����
    }

    // �̵� ���� �������� ����
    private void RandomPositioning()
    {
        Vector3 randomDirection = Random.insideUnitSphere * characterSight.radius;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;   // Y ���� ����

        Vector3 finalPosition = randomDirection;

        finalPosition.x = Mathf.Clamp(finalPosition.x, mapMinBounds.x, mapMaxBounds.x);
        finalPosition.z = Mathf.Clamp(finalPosition.z, mapMinBounds.z, mapMaxBounds.z);

        Vector3 direction = (finalPosition - transform.position).normalized;    // ���� ��ġ���� ��ǥ ��ġ�� �̵�

        // Rigidbody�� ���� �̵�
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Y ���� �̵��� �����ϱ� ���� Y ���� 0���� ����
            direction.y = 0;
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

    // ���� ���� ó��
    void HandleEngageState()
    {
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

            // ���� ���� - BulletManager�� ReloadTime ���
            if (attackCooldownTimer <= 0)
            {
                Attack();
            }
            else
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            // ���� ü�� ������ ���, �� ���·� ����
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
        if (target != null)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mapObject)
        {
            if (!isMoving)
            {
                RandomPositioning();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == mapObject)
        {
            if (!isMoving)
            {
                RandomPositioning();
            }
        }
    }

    // ���� �����ϴ� �Լ�
    void Attack()
    {
        Debug.Log("Attack target!");

        // selectedCharacterIndices �迭���� ��ȿ�� ĳ���� �ε����� ã��
        int currentCharacterIndex = -1;
        foreach (int index in Player.Instance.selectedCharacterIndices)
        {
            if (index >= 0 && index < Player.Instance.ownedCharacter.Count)
            {
                currentCharacterIndex = index;
                break;
            }
        }

        // ��ȿ�� ĳ���� �ε����� ���� ��� ��� �޽��� ��� �� ��ȯ
        if (currentCharacterIndex == -1)
        {
            Debug.LogWarning("No valid character index found in selectedCharacterIndices.");
            return;
        }

        Character currentCharacter = Player.Instance.ownedCharacter[currentCharacterIndex];

        // ������ ��� �������� (equippedGears ����Ʈ�� ù ��° ��� ���)
        Gear equippedGear = null;
        if (currentCharacter.eqiuppedGears != null && currentCharacter.eqiuppedGears.Count > 0)
        {
            string equippedGearName = currentCharacter.eqiuppedGears[0]; // ù ��° ������ ����� �̸� ��������
            Debug.Log($"Equipped gear name: {equippedGearName}");

            // foreach�� ����� gears ����Ʈ���� ��� �˻�
            //foreach (Gear gear in Player.Instance.gears)
            //{
            //    if (gear.name == equippedGearName)
            //    {
            //        equippedGear = gear;
            //        break;
            //    }
            //}

            // GearDataLoader�� ���� ��� ã��
            equippedGear = GearDataLoader.GetGearByName(equippedGearName);

            if (equippedGear == null)
            {
                Debug.LogWarning($"No gear found with the name: {equippedGearName}");
                return;
            }
        }
        else
        {
            Debug.LogWarning("No equipped gears found for this character.");
            return;
        }

        // �Ѿ� Ÿ�� ����
        BulletController.BulletType bulletType = BulletController.DetermineBulletType(equippedGear);

        // ������Ʈ Ǯ���� �Ѿ� ��������
        GameObject bullet = BulletManager.Instance.GetPooledBullet(bulletType);
        if (bullet != null)
        {
            Vector3 bulletPosition = transform.position + transform.forward * 1f;
            bulletPosition.y = 1.0f; // ���ϴ� y�� ���̷� ����
            bullet.transform.position = bulletPosition; // �߻� ��ġ ����
            bullet.SetActive(true);

            // �Ѿ� �ʱ�ȭ
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (bulletController != null)
            {
                bulletController.InitializeBullet(equippedGear, bulletType, teamManager, target);
                attackCooldownTimer = bulletController.ReloadTime; // ReloadTime�� BulletController���� ������
            }
        }
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

public class TeamManager : MonoBehaviour
{
    public enum Team
    {
        Ally,
        Enemy
    }

    public Team team;
}