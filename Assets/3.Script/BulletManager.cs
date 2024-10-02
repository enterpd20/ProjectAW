using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    // 오브젝트 풀링을 위한 관리
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

    // BulletType에 해당하는 프리팹 배열 (인스펙터에서 연결)
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

        // 풀에 사용 가능한 총알이 없다면 새로운 총알을 생성
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
