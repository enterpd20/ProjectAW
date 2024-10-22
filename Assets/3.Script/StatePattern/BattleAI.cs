using Spine.Unity;
using System.Collections;
using UnityEngine;

public class BattleAI : MonoBehaviour
{
    // ĳ������ ���� ����
    enum State
    {
        Patrol,     // ���� ����
        Engage,     // ���� ����
        Retreat,    // ���� ����
        Dead        // ��� ����
    }

    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset normal, move, attack, dead;

    private State currentState = State.Patrol;   // �ʱ� ���´� ��� ���·� ����

    private Transform target;                    // Ÿ��(��) ��ġ

    private int damageIgnoreChance = 0;
    private float attackCooldownTimer = 0f;
    public float moveSpeed;        // �̵� �ӵ� (Dock UI���� ���� ������ �޾ƿ� ��)    
    public float maxHealth;        // �ִ� ü�� (Dock UI���� ���� ������ �޾ƿ� ��)
    public float currentHealth;    // ���� ü�� (Dock UI���� ���� ������ �޾ƿ� ��)

    // ĳ���� �þ� ����, �ּ� ��������, �ִ� ��������
    public SphereCollider characterSight, min_EngageRange, MAX_EngageRange;
    private Rigidbody rb;

    private float patrolMoveCooldown = 1f; // �̵� �簳���� ��� �ð�
    private float patrolMoveTimer = 0f;
    private bool isMoving = false;

    public GameObject mapObject;
    private Vector3 mapMinBounds;
    private Vector3 mapMaxBounds;

    private TeamManager teamManager;

    private Gear equippedGear; // ĳ���Ϳ� ������ ��� ĳ��

    void Start()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        rb = GetComponent<Rigidbody>();

        currentState = State.Patrol;  // �ʱ� ���� ����
        mapObject = FindObjectOfType<MeshCollider>().gameObject;    // �� ������Ʈ ��� �� ������

        // �ʱ� �ִϸ��̼� ���� ����
        SetCharacterState("Idle");

        // �� ������Ʈ�� ��� ���� ������
        if (mapObject != null)
        {
            Renderer mapRenderer = mapObject.GetComponent<Renderer>();
            if (mapRenderer != null)
            {
                float offset = 7f;
                mapMinBounds = mapRenderer.bounds.min + new Vector3(offset, 0, offset);
                mapMaxBounds = mapRenderer.bounds.max - new Vector3(offset, 0, offset);
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
    }

    void Update()
    {
        if (currentHealth <= 0 && currentState != State.Dead)   // ü���� 0 ���ϸ� �� �̻� Update �������� ���� (��� ����)
        {
            SetCharacterState("Dead");
            currentState = State.Dead;
            return;
        }

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

        // �̵����� ���� �� Ÿ�̸Ӹ� �۵����� `patrolMoveCooldown`�� ���� �� �̵� �簳
        if (!isMoving)
        {
            patrolMoveTimer += Time.deltaTime;
            if (patrolMoveTimer >= patrolMoveCooldown)
            {
                RandomPositioning();
                patrolMoveTimer = 0f; // Ÿ�̸� �ʱ�ȭ
            }
        }

        // ���� ������ Ÿ�̸� ���� ������ �׻� ����
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    public void InitializeCharacterStats(CharacterStats stats)
    {
        int selectedIndex = Player.Instance.selectedCharacterIndex;
        if (selectedIndex >= 0 && selectedIndex < Player.Instance.ownedCharacter.Count)
        {
            CharacterStats battleStats = Player.Instance.ownedCharacter[selectedIndex].stats;

            maxHealth = battleStats.HP;
            currentHealth = maxHealth;
            moveSpeed = battleStats.SPD * 0.3f;
            //Debug.Log($"{gameObject.name} - ü�� �ʱ�ȭ: {currentHealth}/{maxHealth}");

            // ��� ���� ĳ��
            string equippedGearName = Player.Instance.ownedCharacter[selectedIndex].eqiuppedGears[0];
            equippedGear = GearDataLoader.GetGearByName(equippedGearName);
        }
        else
        {
            Debug.LogWarning("Selected character index is out of range or invalid.");
        }
    }

    public void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }

    public void SetCharacterState(string state)
    {
        // ���� ���·� ��ȯ���� �ʵ��� ���� ���¿� ��
        if (skeletonAnimation.AnimationName == state) return;

        switch (state)
        {
            case "Idle":
                SetAnimation(normal, true, 1f);
                break;
            case "Move":
                SetAnimation(move, true, 1f);
                break;
            case "Attack":
                SetAnimation(attack, false, 1f);
                break;
            case "Dead":
                SetAnimation(dead, false, 1f);
                break;
        }
    }

    // ���� ���� ó��
    void HandlePatrolState()
    {
        if (!isMoving) SetCharacterState("Idle");
        // �̵� ���� �� �ִϸ��̼� ����

        ScanEnemy();    // �þ� ���� ���� �� ��ĵ �޼���

        if (target == null && !isMoving)     // �̵� ���� �ƴ� ���� ���� ��ġ ����
        {
            RandomPositioning();
            SetCharacterState("Move");
        }
        else if (target != null)
        {
            currentState = State.Engage;
        }
        else if (currentHealth <= maxHealth * 0.35f) // ü���� ������ Retreat ���·� ��ȯ
        {
            currentState = State.Retreat;
        }

        //Debug.Log($"{gameObject.name} - Patrol State!");
    }

    void ScanEnemy()    // �þ� ���� ���� �� ��ĵ �޼���
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, characterSight.radius);
        float closestDistance = float.MaxValue;
        Transform closestTarget = null;

        foreach (var hitCol in hitColliders)
        {
            TeamManager otherTeamManager = hitCol.GetComponent<TeamManager>();

            // �ڱ� �ڽ��̳� �� ������ ������ ��ŵ
            if (otherTeamManager == null || otherTeamManager == teamManager) continue;

            // ���� ���� �ĺ�
            if ((teamManager.team == TeamManager.Team.Ally && otherTeamManager.team == TeamManager.Team.Enemy) ||
                (teamManager.team == TeamManager.Team.Enemy && otherTeamManager.team == TeamManager.Team.Ally))
            {
                // �� ĳ������ BattleAI ������Ʈ�� ������
                BattleAI potentialTarget = hitCol.GetComponent<BattleAI>();

                if (potentialTarget != null && potentialTarget.currentHealth > 0)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, hitCol.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = hitCol.transform;
                    }
                }
            }
        }

        target = closestTarget; // ���� ����� ���� Ÿ������ ����
    }

    // �̵� ���� �������� ����
    private void RandomPositioning()
    {
        Vector3 randomDirection =
            new Vector3(Random.Range(mapMinBounds.x, mapMaxBounds.x),
                        transform.position.y,
                        Random.Range(mapMinBounds.z, mapMaxBounds.z));
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

        //Debug.Log($"Next move to position: {finalPosition}"); // ���� �̵� ��θ� �α׷� ǥ��
    }

    private IEnumerator MoveToPosition(Rigidbody rb, Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.2f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            // �̵� ���� ����: ���� ��� ���� ����Ͽ� ����
            nextPosition.x = Mathf.Clamp(nextPosition.x, mapMinBounds.x, mapMaxBounds.x);
            nextPosition.z = Mathf.Clamp(nextPosition.z, mapMinBounds.z, mapMaxBounds.z);
            rb.MovePosition(nextPosition);
            yield return null;  // ���� �����ӱ��� ���
        }

        transform.position = targetPosition; // ��Ȯ�� ��ġ�� �̵� �Ŀ� ����
        isMoving = false;  // �̵� �Ϸ�
    }

    // ���� ���� ó��
    void HandleEngageState()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // ���� �ִ� ���������� �����, �þ� ������ ����ٸ�
            if (distanceToTarget > MAX_EngageRange.radius && distanceToTarget > characterSight.radius)
            {
                currentState = State.Patrol;    // ���� ���·� ��ȯ
                return;
            }
            // ���� �þ� ���� ���� �ְ� �ִ� �������� �ۿ� �ִٸ�
            else if (distanceToTarget > MAX_EngageRange.radius && distanceToTarget < characterSight.radius)
            {
                ChaseTarget(); // �߰� ���� ����
            }
            // ���� �ִ� �������� ��, �ּ� �������� �ۿ� �ִٸ�
            else if (distanceToTarget < MAX_EngageRange.radius && distanceToTarget > min_EngageRange.radius)
            {
                MovingInEngageRange(); // ���� �̵� ���� ����
            }
            else if (distanceToTarget < min_EngageRange.radius)
            {
                MaintainDistanceFromTarget(); // ���� �Ÿ��� �����ϴ� ����
            }

            // ���� ���� - BulletManager�� ReloadTime ���
            if (attackCooldownTimer <= 0)
            {
                SetCharacterState("Attack");
                Attack();
            }
            else
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            // ���� ü�� ������ ���, �� ���·� ����
            if (currentHealth <= maxHealth * 0.35f)
            {
                currentState = State.Retreat;
            }
        }
        else
        {
            currentState = State.Patrol;
        }

        //Debug.Log($"Current target position in EngageState: {target.position}");
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

        // Rigidbody�� ���� �̵�
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Y ���� �̵��� �����ϱ� ���� Y ���� 0���� ����
            direction.y = 0;
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }

        //Debug.Log($"Moving in Engage range to: {finalPosition}");
    }

    void MaintainDistanceFromTarget()
    {
        Vector3 direction = (transform.position - target.position).normalized; // ���� �ݴ� �������� �̵�
        direction.y = 1.3f; // Y�� ����

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            // �̵� ���� ����: ���� ��� ���� ����Ͽ� ����
            nextPosition.x = Mathf.Clamp(nextPosition.x, mapMinBounds.x, mapMaxBounds.x);
            nextPosition.z = Mathf.Clamp(nextPosition.z, mapMinBounds.z, mapMaxBounds.z);

            rb.MovePosition(nextPosition);
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

        //Debug.Log($"{gameObject.name} - Maintaining distance from target!");

        // �Ÿ��� �ִ� ���� ���� ��, �ּ� ���� ���� ������ �����ߴ��� Ȯ��
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > min_EngageRange.radius && distanceToTarget < MAX_EngageRange.radius)
        {
            // ���� �̵� �������� ��ȯ
            currentState = State.Engage;
        }
    }

    void ChaseTarget()  // ���� �߰��ϴ� �Լ�
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Y�� ����

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            // �̵� ���� ����: ���� ��� ���� ����Ͽ� ����
            nextPosition.x = Mathf.Clamp(nextPosition.x, mapMinBounds.x, mapMaxBounds.x);
            nextPosition.z = Mathf.Clamp(nextPosition.z, mapMinBounds.z, mapMaxBounds.z);
            rb.MovePosition(nextPosition);
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

        //Debug.Log($"{gameObject.name} - Chasing target!");
    }

    void Attack()
    {
        // ���� ĳ������ ������ ��� ��������
        Gear equippedGear = null;
        Character currentCharacter = null;

        if (teamManager != null && teamManager.team == TeamManager.Team.Ally) // �Ʊ��� ��츸
        {
            // BattleAI�� ����� ĳ������ ��� ������
            int characterIndex = Player.Instance.ownedCharacter.FindIndex(character => character.name == this.gameObject.name);
            if (characterIndex >= 0)
            {
                currentCharacter = Player.Instance.ownedCharacter[characterIndex];
                if (currentCharacter.eqiuppedGears != null && currentCharacter.eqiuppedGears.Count > 0)
                {
                    string equippedGearName = currentCharacter.eqiuppedGears[0]; // ù ��° ������ ����� �̸� ��������
                    equippedGear = GearDataLoader.GetGearByName(equippedGearName);
                }
            }
        }
        else if (teamManager.team == TeamManager.Team.Enemy) // ������ ���
        {
            int enemyIndex = Player.Instance.enemyCharacter.FindIndex(character => character.name == this.gameObject.name);
            if (enemyIndex >= 0)
            {
                Character enemyCharacter = Player.Instance.enemyCharacter[enemyIndex];
                if (enemyCharacter.eqiuppedGears != null && enemyCharacter.eqiuppedGears.Count > 0)
                {
                    string equippedGearName = enemyCharacter.eqiuppedGears[0]; // ù ��° ������ ����� �̸� ��������
                    equippedGear = GearDataLoader.GetGearByName(equippedGearName);
                }
            }
        }

        if (equippedGear == null)
        {
            Debug.LogWarning($"No valid gear found for character: {gameObject.name}");
            return;
        }

        if (attackCooldownTimer <= 0)
        {
            SetCharacterState("Attack"); // ��ź�� �߻��� ���� ���� �ִϸ��̼� ���
                // �Ѿ� ����
            GameObject bullet = BulletManager.Instance.GetPooledBullet(equippedGear, teamManager, target, mapMinBounds, mapMaxBounds);
            if (bullet != null)
            {
                Vector3 bulletPosition = transform.position + transform.forward * 1f;
                bulletPosition.y = 1.3f;
                bullet.transform.position = bulletPosition;
                bullet.SetActive(true);

                BulletController bulletController = bullet.GetComponent<BulletController>();
                if (bulletController != null)
                {
                    bulletController.InitializeBullet(equippedGear, teamManager, target, mapMinBounds, mapMaxBounds);
                    attackCooldownTimer = bulletController.ReloadTime;

                    ShipTypeSFX(currentCharacter.shipType);
                }

            }
        }
    }

    // ���� ���� ó��
    void HandleRetreatState()
    {
        SetCharacterState("Move");

        if (target != null)
        {
            // ���� �ݴ� �������� �̵�
            RetreatFromTarget();

            // ���� ���� ���� ���� �ִٸ� ���� ����
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget < MAX_EngageRange.radius && distanceToTarget > min_EngageRange.radius)
            {
                Attack(); // ���� ���� ������ ����
            }

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

        //Debug.Log($"{gameObject.name} - Retreat State!");
    }

    // �����κ��� �����ϴ� �Լ�
    void RetreatFromTarget()
    {
        Vector3 direction = (transform.position - target.position).normalized;

        // Y ���� �̵��� �����ϱ� ���� Y ���� 0���� ����
        direction.y = 0;

        Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

        // �� ��踦 ����� �ʵ��� ��ġ ����
        nextPosition.x = Mathf.Clamp(nextPosition.x, mapMinBounds.x, mapMaxBounds.x);
        nextPosition.z = Mathf.Clamp(nextPosition.z, mapMinBounds.z, mapMaxBounds.z);

        // ���� ĳ���Ͱ� �� ��迡 �����ߴٸ� ���� ���·� ��ȯ
        if (nextPosition.x == mapMinBounds.x || nextPosition.x == mapMaxBounds.x ||
            nextPosition.z == mapMinBounds.z || nextPosition.z == mapMaxBounds.z)
        {
            currentState = State.Patrol;
            //Debug.Log("Reached map boundary, switching to Patrol state.");

            // ��迡 ������ �� ������ ��ġ�� �̵��ϵ��� ��
            RandomPositioning();
        }
        else
        {
            transform.position = nextPosition;
        }
    }

    bool RetreatConditions()
    {
        if (target == null) return false;

        // 1. ü���� 25% �̸��� ���
        if (currentHealth < maxHealth * 0.25f) return true;

        // 2. ü���� 40% �̸��̰�, �þ� ���� ���� ���� 2�� �̻��� ���
        if (currentHealth < maxHealth * 0.4f && CountEnemyInSight() >= 2) return true;

        return false;
    }

    private int CountEnemyInSight()
    {
        int enemyCount = 0;

        // �þ� ���� �ݶ��̴��� �̿��Ͽ� ������� Ž��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, characterSight.radius);

        foreach (var hitCol in hitColliders)
        {
            TeamManager otherTeamManager = hitCol.GetComponent<TeamManager>();

            if (otherTeamManager != null)
            {
                // �� ���� ������ �ν��ϵ���
                if ((teamManager.team == TeamManager.Team.Ally && otherTeamManager.team == TeamManager.Team.Enemy) ||
                    (teamManager.team == TeamManager.Team.Enemy && otherTeamManager.team == TeamManager.Team.Ally))
                {
                    enemyCount++;
                }
            }
        }

        return enemyCount;
    }

    private void ShipTypeSFX(string shipType)
    {
        int sfxIndex = -1; // �⺻���� ������� �ʵ��� ����
        float volume = 1f; // �⺻ ����

        switch (shipType)
        {
            case "DD":
                sfxIndex = 3;
                volume = 0.4f;
                break;
            case "CLCA":
                sfxIndex = 4;
                volume = 0.4f;
                break;
            case "BB":
                sfxIndex = 5;
                volume = 0.4f;
                break;
            default:
                Debug.LogWarning($"Unknown ship type: {shipType}");
                break;
        }

        if(sfxIndex >= 0)
        {
            SoundManager.Instance.PlaySFX(sfxIndex, volume);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{gameObject.name} collided with: {other.gameObject.name}"); // �浹 �����

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

    public void DamageIgnore(int chance)
    {
        // damageIgnoreChance ������ BattleAI Ŭ������ ��� ������ ����Ǿ� �־�� �մϴ�.
        damageIgnoreChance = Mathf.Clamp(chance, 0, 100);
    }

    // ü�� ���� ó�� (���� �޾��� ��)
    public void TakeDamage(float damage)
    {
        // 30% Ȯ���� ������� �����ϴ� ����
        if (Random.Range(0, 100) < damageIgnoreChance)
        {
            Debug.Log($"{gameObject.name}��(��) ������� �����߽��ϴ�!");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            OnDie(); // ��� ó��
        }
    }

    // ĳ���� ��� ó��
    private void OnDie()
    {
        if (currentState == State.Dead) return;

        Debug.Log($"{gameObject.name} has died.");
        SetCharacterState("Dead");
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

        // ��� �ִϸ��̼��� ���� �� ĳ���͸� ��Ȱ��ȭ
        skeletonAnimation.state.Complete += OnAnimationComplete;
    }

    // ��� �ִϸ��̼��� ���� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
    private void OnAnimationComplete(Spine.TrackEntry trackEntry)
    {
        // ��� �ִϸ��̼��� ������ ���� ��Ȱ��ȭ
        if (trackEntry.Animation.Name == dead.name)
        {
            gameObject.SetActive(false); // ĳ���� ��Ȱ��ȭ (���)            
            Destroy(GetComponent<TeamManager>()); // TeamManager ������Ʈ ����

            // �ִϸ��̼� �Ϸ� �̺�Ʈ�� �� �̻� �������� �ʵ��� �ڵ鷯 ����
            skeletonAnimation.state.Complete -= OnAnimationComplete;
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