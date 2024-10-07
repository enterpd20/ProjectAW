using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    // ������Ʈ Ǯ���� ���� ����
    //public static BulletManager Instance
    //{
    //    get
    //    {
    //        if (FindObjectOfType<BulletManager>() == null)
    //        {
    //            Debug.LogError("No BulletManager found in the scene.");
    //        }
    //        return FindObjectOfType<BulletManager>();
    //    }
    //}

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

    //public GameObject GetPooledBullet(BulletController.BulletType type)
    //{
    //    Debug.Log($"[GetPooledBullet] Requested bullet type: {type}"); // ����� �α� �߰�
    //
    //    foreach (var bullet in bulletPool)
    //    {
    //        BulletController bulletController = bullet.GetComponent<BulletController>();
    //        if (!bullet.activeInHierarchy && bulletController != null && bulletController.bulletType == type)
    //        {
    //            Debug.Log($"[GetPooledBullet] Reusing bullet of type: {type}");
    //            return bullet;
    //        }
    //    }
    //
    //    // Ǯ�� ��� ������ �Ѿ��� ���ٸ� ���ο� �Ѿ��� ����
    //    int bulletIndex = (int)type;
    //    if (bulletIndex >= 0 && bulletIndex < bulletPrefabs.Length)
    //    {
    //        GameObject newBullet = Instantiate(bulletPrefabs[bulletIndex]);
    //
    //        newBullet.GetComponent<BulletController>().bulletType = type;
    //
    //        bulletPool.Add(newBullet);
    //
    //        Debug.Log($"[GetPooledBullet] New bullet created of type: {type}");
    //        return newBullet;
    //    }
    //
    //    Debug.LogError($"No bullet prefab found for type: {type}");
    //    return null;
    //}

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

        // Ǯ�� ��� ������ �Ѿ��� ���ٸ� ���ο� �Ѿ��� ����
        int bulletIndex = (int)bulletType;
        if (bulletIndex >= 0 && bulletIndex < bulletPrefabs.Length)
        {
            GameObject newBullet = Instantiate(bulletPrefabs[bulletIndex]);
            BulletController bulletController = newBullet.GetComponent<BulletController>();
            bulletController.bulletType = bulletType;
            bulletController.InitializeBullet(gear, ownerTeamManager, targetTransform, minBounds, maxBounds); // �ʱ�ȭ
            bulletPool.Add(newBullet);
            return newBullet;
        }

        Debug.LogError($"No bullet prefab found for type: {bulletType}");
        return null;
    }
}
