using Spine.Unity;
using System.Collections;
using UnityEngine;

public class BattleAI : MonoBehaviour
{
    // 캐릭터의 상태 정의
    enum State
    {
        Patrol,     // 정찰 상태
        Engage,     // 교전 상태
        Retreat,    // 후퇴 상태
        Dead        // 사망 상태
    }

    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset normal, move, attack, dead;

    private State currentState = State.Patrol;   // 초기 상태는 대기 상태로 설정

    private Transform target;                    // 타겟(적) 위치

    private int damageIgnoreChance = 0;
    private float attackCooldownTimer = 0f;
    public float moveSpeed;        // 이동 속도 (Dock UI에서 최종 스탯을 받아올 것)    
    public float maxHealth;        // 최대 체력 (Dock UI에서 최종 스탯을 받아올 것)
    public float currentHealth;    // 현재 체력 (Dock UI에서 최종 스탯을 받아올 것)

    // 캐릭터 시야 범위, 최소 교전범위, 최대 교전범위
    public SphereCollider characterSight, min_EngageRange, MAX_EngageRange;
    private Rigidbody rb;

    private float patrolMoveCooldown = 1f; // 이동 재개까지 대기 시간
    private float patrolMoveTimer = 0f;
    private bool isMoving = false;

    public GameObject mapObject;
    private Vector3 mapMinBounds;
    private Vector3 mapMaxBounds;

    private TeamManager teamManager;

    private Gear equippedGear; // 캐릭터에 장착된 장비 캐싱

    void Start()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        rb = GetComponent<Rigidbody>();

        currentState = State.Patrol;  // 초기 상태 설정
        mapObject = FindObjectOfType<MeshCollider>().gameObject;    // 맵 오브젝트 경계 값 가져옴

        // 초기 애니메이션 상태 설정
        SetCharacterState("Idle");

        // 맵 오브젝트의 경계 값을 가져옴
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
        if (currentHealth <= 0 && currentState != State.Dead)   // 체력이 0 이하면 더 이상 Update 실행하지 않음 (사망 상태)
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

        // 이동하지 않을 때 타이머를 작동시켜 `patrolMoveCooldown`이 지난 후 이동 재개
        if (!isMoving)
        {
            patrolMoveTimer += Time.deltaTime;
            if (patrolMoveTimer >= patrolMoveCooldown)
            {
                RandomPositioning();
                patrolMoveTimer = 0f; // 타이머 초기화
            }
        }

        // 공격 재장전 타이머 감소 로직을 항상 수행
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

            //maxHealth = battleStats.HP;
            //currentHealth = maxHealth;
            //moveSpeed = battleStats.SPD * 0.3f;
            //Debug.Log($"{gameObject.name} - 체력 초기화: {currentHealth}/{maxHealth}");

            maxHealth = stats.HP;
            currentHealth = maxHealth;
            moveSpeed = stats.SPD * 0.3f;

            // 장비 정보 캐싱
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
        // 같은 상태로 전환되지 않도록 현재 상태와 비교
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

    // 정찰 상태 처리
    void HandlePatrolState()
    {
        if (!isMoving) SetCharacterState("Idle");
        // 이동 시작 시 애니메이션 변경

        ScanEnemy();    // 시야 범위 내의 적 스캔 메서드

        if (target == null && !isMoving)     // 이동 중이 아닐 때만 랜덤 위치 지정
        {
            RandomPositioning();
            SetCharacterState("Move");
        }
        else if (target != null)
        {
            currentState = State.Engage;
        }
        else if (currentHealth <= maxHealth * 0.35f) // 체력이 낮으면 Retreat 상태로 전환
        {
            currentState = State.Retreat;
        }

        //Debug.Log($"{gameObject.name} - Patrol State!");
    }

    void ScanEnemy()    // 시야 범위 내의 적 스캔 메서드
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, characterSight.radius);
        float closestDistance = float.MaxValue;
        Transform closestTarget = null;

        foreach (var hitCol in hitColliders)
        {
            TeamManager otherTeamManager = hitCol.GetComponent<TeamManager>();

            // 자기 자신이나 팀 정보가 없으면 스킵
            if (otherTeamManager == null || otherTeamManager == teamManager) continue;

            // 적대 관계 식별
            if ((teamManager.team == TeamManager.Team.Ally && otherTeamManager.team == TeamManager.Team.Enemy) ||
                (teamManager.team == TeamManager.Team.Enemy && otherTeamManager.team == TeamManager.Team.Ally))
            {
                // 적 캐릭터의 BattleAI 컴포넌트를 가져옴
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

        target = closestTarget; // 가장 가까운 적을 타겟으로 설정
    }

    // 이동 지점 무작위로 지정
    private void RandomPositioning()
    {
        Vector3 randomDirection =
            new Vector3(Random.Range(mapMinBounds.x, mapMaxBounds.x),
                        transform.position.y,
                        Random.Range(mapMinBounds.z, mapMaxBounds.z));
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;   // Y 값을 고정

        Vector3 finalPosition = randomDirection;

        finalPosition.x = Mathf.Clamp(finalPosition.x, mapMinBounds.x, mapMaxBounds.x);
        finalPosition.z = Mathf.Clamp(finalPosition.z, mapMinBounds.z, mapMaxBounds.z);

        Vector3 direction = (finalPosition - transform.position).normalized;    // 현재 위치에서 목표 위치로 이동

        // Rigidbody를 통해 이동
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Y 방향 이동을 방지하기 위해 Y 값을 0으로 설정
            direction.y = 0;
            isMoving = true;  // 이동 시작            
            StartCoroutine(MoveToPosition(rb, finalPosition));  // 이동을 코루틴으로 실행
        }

        // 이동 방향에 따라 스프라이트 뒤집기
        if (direction.x > 0)
        {
            transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
        }

        //Debug.Log($"Next move to position: {finalPosition}"); // 다음 이동 경로를 로그로 표시
    }

    private IEnumerator MoveToPosition(Rigidbody rb, Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.2f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            // 이동 범위 제한: 맵의 경계 값을 사용하여 제한
            nextPosition.x = Mathf.Clamp(nextPosition.x, mapMinBounds.x, mapMaxBounds.x);
            nextPosition.z = Mathf.Clamp(nextPosition.z, mapMinBounds.z, mapMaxBounds.z);
            rb.MovePosition(nextPosition);
            yield return null;  // 다음 프레임까지 대기
        }

        transform.position = targetPosition; // 정확한 위치로 이동 후에 고정
        isMoving = false;  // 이동 완료
    }

    // 전투 상태 처리
    void HandleEngageState()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // 적이 최대 교전범위를 벗어나고, 시야 범위도 벗어난다면
            if (distanceToTarget > MAX_EngageRange.radius && distanceToTarget > characterSight.radius)
            {
                currentState = State.Patrol;    // 정찰 상태로 전환
                return;
            }
            // 적이 시야 범위 내에 있고 최대 교전범위 밖에 있다면
            else if (distanceToTarget > MAX_EngageRange.radius && distanceToTarget < characterSight.radius)
            {
                ChaseTarget(); // 추격 로직 실행
            }
            // 적이 최대 교전범위 안, 최소 교전범위 밖에 있다면
            else if (distanceToTarget < MAX_EngageRange.radius && distanceToTarget > min_EngageRange.radius)
            {
                MovingInEngageRange(); // 전투 이동 로직 실행
            }
            else if (distanceToTarget < min_EngageRange.radius)
            {
                MaintainDistanceFromTarget(); // 적과 거리를 유지하는 로직
            }

            // 공격 로직 - BulletManager의 ReloadTime 사용
            if (attackCooldownTimer <= 0)
            {
                SetCharacterState("Attack");
                Attack();
            }
            else
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            // 일정 체력 이하일 경우, 퇴각 상태로 변경
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

    void MovingInEngageRange()  // 교전 범위 내 무작위 위치로 이동
    {
        Vector3 randomDirection = Random.insideUnitSphere * MAX_EngageRange.radius;
        randomDirection += transform.position;

        // Y 값을 고정
        randomDirection.y = transform.position.y;

        Vector3 finalPosition = randomDirection;

        // 현재 위치에서 목표 위치로 이동
        Vector3 direction = (finalPosition - transform.position).normalized;

        // Rigidbody를 통해 이동
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Y 방향 이동을 방지하기 위해 Y 값을 0으로 설정
            direction.y = 0;
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }

        //Debug.Log($"Moving in Engage range to: {finalPosition}");
    }

    void MaintainDistanceFromTarget()
    {
        Vector3 direction = (transform.position - target.position).normalized; // 적과 반대 방향으로 이동
        direction.y = 1.3f; // Y축 고정

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            // 이동 범위 제한: 맵의 경계 값을 사용하여 제한
            nextPosition.x = Mathf.Clamp(nextPosition.x, mapMinBounds.x, mapMaxBounds.x);
            nextPosition.z = Mathf.Clamp(nextPosition.z, mapMinBounds.z, mapMaxBounds.z);

            rb.MovePosition(nextPosition);
        }

        // 이동 방향에 따라 스프라이트 뒤집기
        if (direction.x > 0)
        {
            transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
        }

        //Debug.Log($"{gameObject.name} - Maintaining distance from target!");

        // 거리를 최대 교전 범위 내, 최소 교전 범위 밖으로 조절했는지 확인
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > min_EngageRange.radius && distanceToTarget < MAX_EngageRange.radius)
        {
            // 전투 이동 로직으로 전환
            currentState = State.Engage;
        }
    }

    void ChaseTarget()  // 적을 추격하는 함수
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Y축 고정

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            // 이동 범위 제한: 맵의 경계 값을 사용하여 제한
            nextPosition.x = Mathf.Clamp(nextPosition.x, mapMinBounds.x, mapMaxBounds.x);
            nextPosition.z = Mathf.Clamp(nextPosition.z, mapMinBounds.z, mapMaxBounds.z);
            rb.MovePosition(nextPosition);
        }

        // 이동 방향에 따라 스프라이트 뒤집기
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
        // 현재 캐릭터의 장착된 장비 가져오기
        Gear equippedGear = null;
        Character currentCharacter = null;

        if (teamManager != null && teamManager.team == TeamManager.Team.Ally) // 아군의 경우만
        {
            // BattleAI에 적용된 캐릭터의 장비를 가져옴
            int characterIndex = Player.Instance.ownedCharacter.FindIndex(character => character.name == this.gameObject.name);
            if (characterIndex >= 0)
            {
                currentCharacter = Player.Instance.ownedCharacter[characterIndex];
                if (currentCharacter.eqiuppedGears != null && currentCharacter.eqiuppedGears.Count > 0)
                {
                    string equippedGearName = currentCharacter.eqiuppedGears[0]; // 첫 번째 장착된 장비의 이름 가져오기
                    equippedGear = GearDataLoader.GetGearByName(equippedGearName);
                }
            }
        }
        else if (teamManager.team == TeamManager.Team.Enemy) // 적군의 경우
        {
            int enemyIndex = Player.Instance.enemyCharacter.FindIndex(character => character.name == this.gameObject.name);
            if (enemyIndex >= 0)
            {
                Character enemyCharacter = Player.Instance.enemyCharacter[enemyIndex];
                if (enemyCharacter.eqiuppedGears != null && enemyCharacter.eqiuppedGears.Count > 0)
                {
                    string equippedGearName = enemyCharacter.eqiuppedGears[0]; // 첫 번째 장착된 장비의 이름 가져오기
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
            SetCharacterState("Attack"); // 포탄을 발사할 때만 공격 애니메이션 재생
                // 총알 생성
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

    // 후퇴 상태 처리
    void HandleRetreatState()
    {
        SetCharacterState("Move");

        if (target != null)
        {
            // 적과 반대 방향으로 이동
            RetreatFromTarget();

            // 교전 범위 내에 적이 있다면 공격 가능
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget < MAX_EngageRange.radius && distanceToTarget > min_EngageRange.radius)
            {
                Attack(); // 교전 범위 내에서 공격
            }

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

        //Debug.Log($"{gameObject.name} - Retreat State!");
    }

    // 적으로부터 후퇴하는 함수
    void RetreatFromTarget()
    {
        Vector3 direction = (transform.position - target.position).normalized;

        // Y 방향 이동을 방지하기 위해 Y 값을 0으로 설정
        direction.y = 0;

        Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

        // 맵 경계를 벗어나지 않도록 위치 제한
        nextPosition.x = Mathf.Clamp(nextPosition.x, mapMinBounds.x, mapMaxBounds.x);
        nextPosition.z = Mathf.Clamp(nextPosition.z, mapMinBounds.z, mapMaxBounds.z);

        // 만약 캐릭터가 맵 경계에 도달했다면 정찰 상태로 전환
        if (nextPosition.x == mapMinBounds.x || nextPosition.x == mapMaxBounds.x ||
            nextPosition.z == mapMinBounds.z || nextPosition.z == mapMaxBounds.z)
        {
            currentState = State.Patrol;
            //Debug.Log("Reached map boundary, switching to Patrol state.");

            // 경계에 도달한 후 랜덤한 위치로 이동하도록 함
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

        // 1. 체력이 25% 미만인 경우
        if (currentHealth < maxHealth * 0.25f) return true;

        // 2. 체력이 40% 미만이고, 시야 범위 내의 적이 2명 이상인 경우
        if (currentHealth < maxHealth * 0.4f && CountEnemyInSight() >= 2) return true;

        return false;
    }

    private int CountEnemyInSight()
    {
        int enemyCount = 0;

        // 시야 범위 콜라이더를 이용하여 상대팀을 탐지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, characterSight.radius);

        foreach (var hitCol in hitColliders)
        {
            TeamManager otherTeamManager = hitCol.GetComponent<TeamManager>();

            if (otherTeamManager != null)
            {
                // 각 팀을 적으로 인식하도록
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
        int sfxIndex = -1; // 기본값은 재생되지 않도록 설정
        float volume = 1f; // 기본 볼륨

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
        //Debug.Log($"{gameObject.name} collided with: {other.gameObject.name}"); // 충돌 디버그

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
        // damageIgnoreChance 변수는 BattleAI 클래스의 멤버 변수로 선언되어 있어야 합니다.
        damageIgnoreChance = Mathf.Clamp(chance, 0, 100);
    }

    // 체력 감소 처리 (공격 받았을 때)
    public void TakeDamage(float damage)
    {
        // 30% 확률로 대미지를 무시하는 로직
        if (Random.Range(0, 100) < damageIgnoreChance)
        {
            Debug.Log($"{gameObject.name}이(가) 대미지를 무시했습니다!");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            OnDie(); // 사망 처리
        }
    }

    // 캐릭터 사망 처리
    private void OnDie()
    {
        if (currentState == State.Dead) return;

        Debug.Log($"{gameObject.name} has died.");
        SetCharacterState("Dead");
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

        // 사망 애니메이션이 끝난 후 캐릭터를 비활성화
        skeletonAnimation.state.Complete += OnAnimationComplete;
    }

    // 사망 애니메이션이 끝난 후 호출되는 이벤트 핸들러
    private void OnAnimationComplete(Spine.TrackEntry trackEntry)
    {
        // 사망 애니메이션이 끝났을 때만 비활성화
        if (trackEntry.Animation.Name == dead.name)
        {
            gameObject.SetActive(false); // 캐릭터 비활성화 (사망)            
            Destroy(GetComponent<TeamManager>()); // TeamManager 컴포넌트 제거

            // 애니메이션 완료 이벤트를 더 이상 감지하지 않도록 핸들러 제거
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