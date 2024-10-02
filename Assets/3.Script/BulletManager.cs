using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
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

    // BulletType�� �ش��ϴ� ������ �迭 (�ν����Ϳ��� ����)
    public GameObject[] bulletPrefabs;

    private List<GameObject> bulletPool = new List<GameObject>();

    public GameObject GetPooledBullet(BulletController.BulletType type)
    {
        foreach (var bullet in bulletPool)
        {
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (!bullet.activeInHierarchy && bulletController != null && bulletController.bulletType == type)
            {
                return bullet;
            }
        }

        // Ǯ�� ��� ������ �Ѿ��� ���ٸ� ���ο� �Ѿ��� ����
        int bulletIndex = (int)type;
        if (bulletIndex >= 0 && bulletIndex < bulletPrefabs.Length)
        {
            GameObject newBullet = Instantiate(bulletPrefabs[bulletIndex]);
            bulletPool.Add(newBullet);

            return newBullet;
        }

        Debug.LogError($"No bullet prefab found for type: {type}");
        return null;
    }
}
