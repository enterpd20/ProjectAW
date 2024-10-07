using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{    
    public enum BulletType  // 총 7가지
    {
        DD_Gun,         // 구축함 주포
        CLCA_Gun,       // 순양함 주포
        CLCA_SubGun,    // 순양함 부포
        BB_Gun,         // 전함 주포
        BB_SubGun,      // 전함 부포
        Torpedo,        // 어뢰(구축함, 뇌격기)
        Bomb            // 폭탄(급강하폭격기)
    }

    public BulletType bulletType;

    public float bulletSpeed;  // 포탄 고유의 속성 SPD, 포탄이 날아가는 속도
    public float Damage;       // 주포에서 받아오는 DMG
    public float ReloadTime;   // 주포에서 받아오는 RLD

    private Rigidbody rb;
    private TeamManager teamManager;

    private Vector3 mapMinBounds;
    private Vector3 mapMaxBounds;

    private void Update()
    {
        // 총알이 맵 경계를 벗어났는지 체크
        if (transform.position.x < mapMinBounds.x || transform.position.x > mapMaxBounds.x ||
            transform.position.z < mapMinBounds.z || transform.position.z > mapMaxBounds.z)
        {
            DeactivateBullet();
        }
    }

    public void InitializeBullet(Gear gear/*, BulletType type*/, TeamManager ownerTeamManager, Transform targetTransform, Vector3 minBounds, Vector3 maxBounds)
    {
        if (gear != null)
        {
            Damage = gear.stats.DMG;        // 장비의 공격력 그대로 받아오기
            ReloadTime = gear.stats.RLD;    // 장비의 재장전 그대로 받아오기

            bulletType = DetermineBulletType(gear);

            //Debug.Log($"Bullet Initialized: Damage = {Damage}, ReloadTime = {ReloadTime}");
        }
        else
        {
            Debug.LogWarning($"Gear data is null.");
        }
    
        rb = GetComponent<Rigidbody>();
        teamManager = ownerTeamManager;
    
        //bulletType = type;
        bulletSpeed = BulletSpeed(bulletType);
        Debug.Log($"Bullet Initialized: Type = {bulletType}, Damage = {Damage}, Speed = {bulletSpeed}");

        // 타겟 방향 설정
        if (targetTransform != null)
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;

            // 회전 설정: 포탄이 날아가는 방향으로 회전
            //transform.rotation = Quaternion.LookRotation(direction);

            // 포탄이 날아가는 방향에 따라 회전하도록 설정
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Z축 회전을 적용

            rb.velocity = direction * bulletSpeed;
        }

        // 맵 경계 설정
        mapMinBounds = minBounds;
        mapMaxBounds = maxBounds;
    }
    public static BulletType DetermineBulletType(Gear gear)
    {
        switch (gear.gearType)
        {
            case "DD Gun":
                return BulletType.DD_Gun;
            case "CLCA Gun":
                return BulletType.CLCA_Gun;
            case "CLCA Sub Gun":
                return BulletType.CLCA_SubGun;
            case "BB Gun":
                return BulletType.BB_Gun;
            case "BB Sub Gun":
                return BulletType.BB_SubGun;
            case "Torpedo":
                return BulletType.Torpedo;
            case "Bomb":
                return BulletType.Bomb;
            default:
                Debug.LogWarning($"Unknown gear type: {gear.gearType}");
                return BulletType.DD_Gun;
        }
    }

    private float BulletSpeed(BulletType type)  // 포탄이 날아가는 속도
    {
        switch (type)
        {
            case BulletType.DD_Gun:      return 30f;
            case BulletType.CLCA_Gun:    return 30f;
            case BulletType.CLCA_SubGun: return 30f;
            case BulletType.BB_Gun:      return 25f;
            case BulletType.BB_SubGun:   return 30f;
            case BulletType.Torpedo:     return 30f;
            case BulletType.Bomb:        return 50f;
            default:                     return 1f;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // 충돌한 오브젝트의 TeamManager 컴포넌트를 가져옴
        TeamManager targetTeamManager = collision.GetComponent<TeamManager>();

        if (targetTeamManager != null)
        {
            if ((teamManager.team == TeamManager.Team.Ally && targetTeamManager.team == TeamManager.Team.Enemy) ||
                (teamManager.team == TeamManager.Team.Enemy && targetTeamManager.team == TeamManager.Team.Ally))
            {
                // 데미지를 주는 로직
                BattleAI enemyAI = collision.GetComponent<BattleAI>();
                if (enemyAI != null)
                {
                    enemyAI.TakeDamage(Damage);
                    DeactivateBullet();
                }                
            }
        }
    }

    private void DeactivateBullet()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        gameObject.SetActive(false); // 총알을 비활성화
    }
}
