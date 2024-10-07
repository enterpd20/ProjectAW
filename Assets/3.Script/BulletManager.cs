using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    // 오브젝트 풀링을 위한 관리
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

    public static BulletManager Instance { get; private set; } // 인스턴스 저장 및 접근 모두 이 프로퍼티에서 처리

    // BulletType에 해당하는 프리팹 배열 (인스펙터에서 연결)
    public GameObject[] bulletPrefabs;

    private List<GameObject> bulletPool = new List<GameObject>();

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple BulletManager instances found! Destroying duplicate.");
            Destroy(gameObject); // 중복된 인스턴스가 있을 경우 파괴
        }
    }

    //public GameObject GetPooledBullet(BulletController.BulletType type)
    //{
    //    Debug.Log($"[GetPooledBullet] Requested bullet type: {type}"); // 디버그 로그 추가
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
    //    // 풀에 사용 가능한 총알이 없다면 새로운 총알을 생성
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
                bulletController.InitializeBullet(gear, ownerTeamManager, targetTransform, minBounds, maxBounds); // 초기화
                return bullet;
            }
        }

        // 풀에 사용 가능한 총알이 없다면 새로운 총알을 생성
        int bulletIndex = (int)bulletType;
        if (bulletIndex >= 0 && bulletIndex < bulletPrefabs.Length)
        {
            GameObject newBullet = Instantiate(bulletPrefabs[bulletIndex]);
            BulletController bulletController = newBullet.GetComponent<BulletController>();
            bulletController.bulletType = bulletType;
            bulletController.InitializeBullet(gear, ownerTeamManager, targetTransform, minBounds, maxBounds); // 초기화
            bulletPool.Add(newBullet);
            return newBullet;
        }

        Debug.LogError($"No bullet prefab found for type: {bulletType}");
        return null;
    }
}
