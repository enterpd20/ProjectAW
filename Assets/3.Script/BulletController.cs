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

    public void InitializeBullet(Gear gear, BulletType type, TeamManager ownerTeamManager, Transform targetTransform)
    {
        if (gear != null)
        {
            Damage = gear.stats.DMG;        // 장비의 공격력 그대로 받아오기
            ReloadTime = gear.stats.RLD;    // 장비의 재장전 그대로 받아오기
            //Debug.Log($"Bullet Initialized: Damage = {Damage}, ReloadTime = {ReloadTime}");
        }
        else
        {
            Debug.LogWarning($"Gear data is null.");
        }
    
        rb = GetComponent<Rigidbody>();
        teamManager = ownerTeamManager;
    
        bulletType = type;
        bulletSpeed = BulletSpeed(bulletType);
        Debug.Log($"Bullet Type: {bulletType}, Speed: {bulletSpeed}");
        //rb.isKinematic = true;
    
        // 타겟 방향 설정
        if (targetTransform != null)
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            rb.velocity = direction * bulletSpeed;
        }
    }

    private float BulletSpeed(BulletType type)  // 포탄이 날아가는 속도
    {
        switch (type)
        {
            case BulletType.DD_Gun:      return 50f;
            case BulletType.CLCA_Gun:    return 40f;
            case BulletType.CLCA_SubGun: return 50f;
            case BulletType.BB_Gun:      return 35f;
            case BulletType.BB_SubGun:   return 50f;
            case BulletType.Torpedo:     return 40f;
            case BulletType.Bomb:        return 50f;
            default:                     return 1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 TeamManager 컴포넌트를 가져옴
        TeamManager targetTeamManager = other.GetComponent<TeamManager>();

        if (targetTeamManager != null)
        {
            if ((teamManager.team == TeamManager.Team.Ally && targetTeamManager.team == TeamManager.Team.Enemy) ||
                (teamManager.team == TeamManager.Team.Enemy && targetTeamManager.team == TeamManager.Team.Ally))
            {
                // 적에게 데미지를 주는 로직
                BattleAI enemyAI = other.GetComponent<BattleAI>();
                if (enemyAI != null)
                {
                    enemyAI.TakeDamage(Damage);
                }

                // 총알을 비활성화
                gameObject.SetActive(false);
            }
        }

        // 물리적 상호작용 차단
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
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
}
