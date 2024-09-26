using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public enum BulletType
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

    public float speed;         // 포탄 고유의 속성 SPD, 포탄이 날아가는 속도
    private float Damage;       // 주포에서 받아오는 DMG
    private float ReloadTime;   // 주포에서 받아오는 RLD

    private Rigidbody rigidbody;

    public void InitializeBullet(Gear gear, BulletType type)
    {
        if(gear != null)
        {
            Damage = gear.stats.DMG;        // 장비의 공격력 그대로 받아오기
            ReloadTime = gear.stats.RLD;    // 장비의 재장전 그대로 받아오기
        }
        else
        {
            Debug.LogWarning($"Gear data is null.");
        }

        bulletType = type;
        speed = BulletSpeed(bulletType);

        rigidbody = GetComponent<Rigidbody>();

        ShootBullet(transform);
    }

    private void ShootBullet(Transform target)  // 포탄이 날아가는 방향
    {
        Vector2 direction = (target.position - transform.position).normalized;

        rigidbody.velocity = direction * speed;
    }

    private float BulletSpeed(BulletType type)  // 포탄이 날아가는 속도
    {
        switch(type)
        {
            case BulletType.DD_Gun:
                return 10f;
            case BulletType.CLCA_Gun:
                return 10f;
            case BulletType.CLCA_SubGun:
                return 10f;
            case BulletType.BB_Gun:
                return 7f;
            case BulletType.BB_SubGun:
                return 10f;
            case BulletType.Torpedo:
                return 10f;
            case BulletType.Bomb:
                return 15f;
            default:
                return 1f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 적에게 데미지를 주는 로직 작성
        if(collision.collider.CompareTag("Enemy"))
        {
            
            gameObject.SetActive(false);
        }

    }
}
