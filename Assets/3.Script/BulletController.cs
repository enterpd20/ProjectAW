using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{    
    public enum BulletType  // �� 7����
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

    public float bulletSpeed;  // ��ź ������ �Ӽ� SPD, ��ź�� ���ư��� �ӵ�
    public float Damage;       // �������� �޾ƿ��� DMG
    public float ReloadTime;   // �������� �޾ƿ��� RLD

    private Rigidbody rb;
    private TeamManager teamManager;

    public void InitializeBullet(Gear gear, BulletType type, TeamManager ownerTeamManager, Transform targetTransform)
    {
        if (gear != null)
        {
            Damage = gear.stats.DMG;        // ����� ���ݷ� �״�� �޾ƿ���
            ReloadTime = gear.stats.RLD;    // ����� ������ �״�� �޾ƿ���
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
    
        // Ÿ�� ���� ����
        if (targetTransform != null)
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            rb.velocity = direction * bulletSpeed;
        }
    }

    private float BulletSpeed(BulletType type)  // ��ź�� ���ư��� �ӵ�
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
        // �浹�� ������Ʈ�� TeamManager ������Ʈ�� ������
        TeamManager targetTeamManager = other.GetComponent<TeamManager>();

        if (targetTeamManager != null)
        {
            if ((teamManager.team == TeamManager.Team.Ally && targetTeamManager.team == TeamManager.Team.Enemy) ||
                (teamManager.team == TeamManager.Team.Enemy && targetTeamManager.team == TeamManager.Team.Ally))
            {
                // ������ �������� �ִ� ����
                BattleAI enemyAI = other.GetComponent<BattleAI>();
                if (enemyAI != null)
                {
                    enemyAI.TakeDamage(Damage);
                }

                // �Ѿ��� ��Ȱ��ȭ
                gameObject.SetActive(false);
            }
        }

        // ������ ��ȣ�ۿ� ����
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
