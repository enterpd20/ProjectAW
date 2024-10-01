using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
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

    public float bulletSpeed;         // ��ź ������ �Ӽ� SPD, ��ź�� ���ư��� �ӵ�
    public float Damage;       // �������� �޾ƿ��� DMG
    public float ReloadTime;   // �������� �޾ƿ��� RLD

    private Rigidbody rigidbody;

    // ������Ʈ Ǯ���� ���� ����
    public static BulletManager Instance
    {
        get
        {
            if (FindObjectOfType<BulletManager>() == null)
            {
                Debug.LogError("No BulletManager found in the scene.");
            }
            return FindObjectOfType<BulletManager>();
        }
    }

    private List<GameObject> bulletPool = new List<GameObject>();

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
        bulletSpeed = BulletSpeed(bulletType);

        rigidbody = GetComponent<Rigidbody>();

        ShootBullet();
    }

    private void ShootBullet()
    {
        // Ÿ�� ���� ���� (���Ƿ� target ���� ����)
        Transform target = transform;  // ���� Ÿ���� �޾ƿ;� ��
        Vector2 direction = (target.position - transform.position).normalized;
        rigidbody.velocity = direction * bulletSpeed;
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

    public GameObject GetPooledBullet()
    {
        foreach (var bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }
        return null;
    }
}
