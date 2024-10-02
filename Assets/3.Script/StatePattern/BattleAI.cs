using System.Collections;
using UnityEngine;

/*
캐릭터가 가져야하는 스탯
체력 - 여기서 씀
이동속도 - 여기서 씀
공격력 - BulletControl에서 장비 데이터 받아와서 씀
재장전 - BulletControl에서 장비 데이터 받아와서 씀
*/

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
    private float moveSpeed;        // 이동 속도 (Dock UI에서 최종 스탯을 받아올 것)

    // 캐릭터 시야 범위, 최소 교전범위, 최대 교전범위
    public SphereCollider characterSight, min_EngageRange, MAX_EngageRange;
    private bool isMoving = false;

    private float attackCooldownTimer = 0f;

    public GameObject mapObject;
    private Vector3 mapMinBounds;
    private Vector3 mapMaxBounds;

    private TeamManager teamManager;

    void Start()
    {
        currentState = State.Patrol;  // 초기 상태 설정
        mapObject = FindObjectOfType<MeshCollider>().gameObject;    // 맵 오브젝트 경계 값 가져옴

        // 맵 오브젝트의 경계 값을 가져옴
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

        // 장비 데이터 로드 확인
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

    // 캐릭터 스탯 불러오기
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


    // 정찰 상태 처리
    void HandlePatrolState()    // [[[[[시야범위 내에 적이 있다면, 적이 최대 교전범위 내에 들어올 때까지 추격 -> 구현돼있는지 확인]]]]]
    {
        ScanEnemy();            // 시야 범위 내의 적 스캔 메서드

        if (target == null && !isMoving)     // 이동 중이 아닐 때만 랜덤 위치 지정
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

            // 자기 자신이나 팀 정보가 없으면 스킵
            if (otherTeamManager == null || otherTeamManager == teamManager) continue;

            // 적대 관계 식별
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

        target = closestTarget; // 가장 가까운 적을 타겟으로 설정
    }

    // 이동 지점 무작위로 지정
    private void RandomPositioning()
    {
        Vector3 randomDirection = Random.insideUnitSphere * characterSight.radius;
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
    }

    private IEnumerator MoveToPosition(Rigidbody rb, Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0;

            Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            //rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

            // 이동 범위 제한: 맵의 경계 값을 사용하여 제한
            nextPosition.x = Mathf.Clamp(nextPosition.x, mapMinBounds.x, mapMaxBounds.x);
            nextPosition.z = Mathf.Clamp(nextPosition.z, mapMinBounds.z, mapMaxBounds.z);
            rb.MovePosition(nextPosition);
            yield return null;  // 다음 프레임까지 대기
        }

        isMoving = false;  // 이동 완료
    }

    // 전투 상태 처리
    void HandleEngageState()
    {
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

            // 공격 로직 - BulletManager의 ReloadTime 사용
            if (attackCooldownTimer <= 0)
            {
                Attack();
            }
            else
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            // 일정 체력 이하일 경우, 퇴각 상태로 변경
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

        // Y 값을 고정
        randomDirection.y = transform.position.y;

        Vector3 finalPosition = randomDirection;

        // 현재 위치에서 목표 위치로 이동
        Vector3 direction = (finalPosition - transform.position).normalized;

        //transform.position += direction * moveSpeed * Time.deltaTime;
        // Rigidbody를 통해 이동
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Y 방향 이동을 방지하기 위해 Y 값을 0으로 설정
            direction.y = 0;
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            //rb.velocity = direction * moveSpeed;
        }
    }

    // 후퇴 상태 처리
    void HandleRetreatState()
    {
        if (target != null)
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

        // Y 방향 이동을 방지하기 위해 Y 값을 0으로 설정
        direction.y = 0;

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

    // 적을 공격하는 함수
    void Attack()
    {
        Debug.Log("Attack target!");

        // selectedCharacterIndices 배열에서 유효한 캐릭터 인덱스를 찾음
        int currentCharacterIndex = -1;
        foreach (int index in Player.Instance.selectedCharacterIndices)
        {
            if (index >= 0 && index < Player.Instance.ownedCharacter.Count)
            {
                currentCharacterIndex = index;
                break;
            }
        }

        // 유효한 캐릭터 인덱스가 없을 경우 경고 메시지 출력 후 반환
        if (currentCharacterIndex == -1)
        {
            Debug.LogWarning("No valid character index found in selectedCharacterIndices.");
            return;
        }

        Character currentCharacter = Player.Instance.ownedCharacter[currentCharacterIndex];

        // 장착된 장비 가져오기 (equippedGears 리스트의 첫 번째 장비 사용)
        Gear equippedGear = null;
        if (currentCharacter.eqiuppedGears != null && currentCharacter.eqiuppedGears.Count > 0)
        {
            string equippedGearName = currentCharacter.eqiuppedGears[0]; // 첫 번째 장착된 장비의 이름 가져오기
            Debug.Log($"Equipped gear name: {equippedGearName}");

            // foreach를 사용해 gears 리스트에서 장비 검색
            //foreach (Gear gear in Player.Instance.gears)
            //{
            //    if (gear.name == equippedGearName)
            //    {
            //        equippedGear = gear;
            //        break;
            //    }
            //}

            // GearDataLoader를 통해 장비 찾기
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

        // 총알 타입 결정
        BulletController.BulletType bulletType = BulletController.DetermineBulletType(equippedGear);

        // 오브젝트 풀에서 총알 가져오기
        GameObject bullet = BulletManager.Instance.GetPooledBullet(bulletType);
        if (bullet != null)
        {
            Vector3 bulletPosition = transform.position + transform.forward * 1f;
            bulletPosition.y = 1.0f; // 원하는 y축 높이로 고정
            bullet.transform.position = bulletPosition; // 발사 위치 설정
            bullet.SetActive(true);

            // 총알 초기화
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (bulletController != null)
            {
                bulletController.InitializeBullet(equippedGear, bulletType, teamManager, target);
                attackCooldownTimer = bulletController.ReloadTime; // ReloadTime을 BulletController에서 가져옴
            }
        }
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

public class TeamManager : MonoBehaviour
{
    public enum Team
    {
        Ally,
        Enemy
    }

    public Team team;
}