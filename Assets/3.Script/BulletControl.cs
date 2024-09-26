using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public enum BulletType
    {
        DD_Gun,         // ������ ����
        CLCA_Gun,       // ������ ����
        CLCA_SubGun,    // ������ ����
        BB_Gun,         // ���� ����
        BB_SubGun,      // ���� ����
        Torpedo,        // ���(������, ���ݱ�)
        Bomb            // ��ź(�ް������ݱ�)
    }

    public BulletType bulletType;

    public float speed;         // ��ź ������ �Ӽ� SPD, ��ź�� ���ư��� �ӵ�
    private float Damage;       // �������� �޾ƿ��� DMG
    private float ReloadTime;   // �������� �޾ƿ��� RLD

    private Rigidbody rigidbody;

    public void InitializeBullet(Gear gear, BulletType type)
    {
        if(gear != null)
        {
            Damage = gear.stats.DMG;        // ����� ���ݷ� �״�� �޾ƿ���
            ReloadTime = gear.stats.RLD;    // ����� ������ �״�� �޾ƿ���
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

    private void ShootBullet(Transform target)  // ��ź�� ���ư��� ����
    {
        Vector2 direction = (target.position - transform.position).normalized;

        rigidbody.velocity = direction * speed;
    }

    private float BulletSpeed(BulletType type)  // ��ź�� ���ư��� �ӵ�
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
        // ������ �������� �ִ� ���� �ۼ�
        if(collision.collider.CompareTag("Enemy"))
        {
            
            gameObject.SetActive(false);
        }

    }
}
