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

    private Vector3 mapMinBounds;
    private Vector3 mapMaxBounds;

    private void Update()
    {
        // ��ź�� ���� ȸ�� ���� ����� �α�
        //Debug.Log($"Update - Bullet Rotation: {transform.rotation.eulerAngles}");

        float offset = 5f;

        // �Ѿ��� �� ��踦 ������� üũ
        if (transform.position.x < mapMinBounds.x - offset || transform.position.x > mapMaxBounds.x + offset ||
            transform.position.z < mapMinBounds.z - offset || transform.position.z > mapMaxBounds.z + offset)
        {
            DeactivateBullet();
        }
    }

    public void InitializeBullet(Gear gear, TeamManager ownerTeamManager, Transform targetTransform, Vector3 minBounds, Vector3 maxBounds)
    {
        if (gear != null)
        {
            Damage = gear.stats.DMG;        // ����� ���ݷ� �״�� �޾ƿ���
            ReloadTime = gear.stats.RLD;    // ����� ������ �״�� �޾ƿ���

            bulletType = DetermineBulletType(gear);

            //Debug.Log($"Bullet Initialized: Damage = {Damage}, ReloadTime = {ReloadTime}");
        }
        else
        {
            Debug.LogWarning($"Gear data is null.");
        }
    
        rb = GetComponent<Rigidbody>();
        teamManager = ownerTeamManager;

        bulletSpeed = BulletSpeed(bulletType);
        //Debug.Log($"Bullet Initialized: Type = {bulletType}, Damage = {Damage}, Speed = {bulletSpeed}");

        // Ÿ�� ���� ����
        if (targetTransform != null)
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;

            // ��ź�� ��������Ʈ�� ȸ���ϵ��� ����
            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                spriteRenderer.transform.rotation = Quaternion.Euler(57, angle - 90f, 0); // Z�� ȸ���� ����

                // InitializeBullet �޼��忡�� ȸ�� ���� ����� �α�
                //Debug.Log($"InitializeBullet - Set Bullet Rotation: {spriteRenderer.transform.rotation.eulerAngles}");
            }

            rb.velocity = direction * bulletSpeed;
        }

        // �� ��� ����
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

    private float BulletSpeed(BulletType type)  // ��ź�� ���ư��� �ӵ�
    {
        switch (type)
        {
            case BulletType.DD_Gun:      return 50f;
            case BulletType.CLCA_Gun:    return 45f;
            case BulletType.CLCA_SubGun: return 30f;
            case BulletType.BB_Gun:      return 35f;
            case BulletType.BB_SubGun:   return 30f;
            case BulletType.Torpedo:     return 30f;
            case BulletType.Bomb:        return 50f;
            default:                     return 1f;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // �浹�� ������Ʈ�� TeamManager ������Ʈ�� ������
        TeamManager targetTeamManager = collision.GetComponent<TeamManager>();

        if (targetTeamManager != null)
        {
            if ((teamManager.team == TeamManager.Team.Ally && targetTeamManager.team == TeamManager.Team.Enemy) ||
                (teamManager.team == TeamManager.Team.Enemy && targetTeamManager.team == TeamManager.Team.Ally))
            {
                // �������� �ִ� ����
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

        gameObject.SetActive(false); // �Ѿ��� ��Ȱ��ȭ
    }
}
