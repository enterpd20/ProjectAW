using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance { get; private set; } // �ν��Ͻ� ���� �� ���� ��� �� ������Ƽ���� ó��

    // BulletType�� �ش��ϴ� ������ �迭 (�ν����Ϳ��� ����)
    public GameObject[] bulletPrefabs;

    private List<GameObject> bulletPool = new List<GameObject>();

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple BulletManager instances found! Destroying duplicate.");
            Destroy(gameObject); // �ߺ��� �ν��Ͻ��� ���� ��� �ı�
        }
    }

    public GameObject GetPooledBullet(Gear gear, TeamManager ownerTeamManager, Transform targetTransform, Vector3 minBounds, Vector3 maxBounds)
    {
        BulletController.BulletType bulletType = BulletController.DetermineBulletType(gear);

        foreach (var bullet in bulletPool)
        {
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (!bullet.activeInHierarchy && bulletController != null && bulletController.bulletType == bulletType)
            {
                bulletController.InitializeBullet(gear, ownerTeamManager, targetTransform, minBounds, maxBounds); // �ʱ�ȭ
                return bullet;
            }
        }

        // ��Ȱ��ȭ�� ��ź�� ������ ���ο� ��ź�� ����
        return CreateNewBullet(bulletType, gear, ownerTeamManager, targetTransform, minBounds, maxBounds);
    }

    // ���ο� ��ź�� �����ϴ� �Լ�
    private GameObject CreateNewBullet(BulletController.BulletType bulletType, Gear gear, TeamManager ownerTeamManager, Transform targetTransform, Vector3 minBounds, Vector3 maxBounds)
    {
        int bulletIndex = (int)bulletType;

        // bulletPrefabs �迭 ���� �ش� Ÿ���� ��ź�� �ִ��� Ȯ��
        if (bulletIndex >= 0 && bulletIndex < bulletPrefabs.Length)
        {
            // ���ο� ��ź ���� �� �ʱ�ȭ
            GameObject newBullet = Instantiate(bulletPrefabs[bulletIndex]);
            BulletController bulletController = newBullet.GetComponent<BulletController>();
            bulletController.bulletType = bulletType;
            bulletController.InitializeBullet(gear, ownerTeamManager, targetTransform, minBounds, maxBounds); // �ʱ�ȭ

            // Ǯ�� ���� ������ ��ź�� �߰�
            bulletPool.Add(newBullet);
            return newBullet;
        }

        Debug.LogError($"No bullet prefab found for type: {bulletType}");
        return null;
    }
}
